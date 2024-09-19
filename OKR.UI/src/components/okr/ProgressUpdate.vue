<template>
  <div class="infinite-list-wrapper"  style="overflow: auto">
    <ul
      v-infinite-scroll="searchProgressUpdate"
      class="list"
      :infinite-scroll-disabled="disabled"
    >
      <li v-for="item in listProgressUpdate" :key="item.id" class="list-item">
        <el-card>
          <div class="point" v-if="isNumberOrNumericString(item.oldPoint) && isNumberOrNumericString(item.newPoint)">
            <strong>{{ item.oldPoint }}</strong> <el-icon><Right /></el-icon> <strong>{{ item.newPoint }}</strong> 
            <el-tag>{{
              item.unit == 0 ? "Percent" : item.unit == 1 ? "Value" : "Checked"
            }}</el-tag>
          </div>
          <div class="content">
            <p>Date: <strong>{{ formatDate(item.createOn) }}</strong></p>
            <p>{{ item.note }}</p>
          </div>
        </el-card>
      </li>
    </ul>
  </div>
  <p v-if="noMore">nomore</p>
</template>

<script lang="ts" setup>
import { computed, onMounted, ref, watch } from "vue";
import Cookie from "js-cookie";
import { axiosInstance } from "@/Service/axiosConfig";
import { SearchRequest } from "../../components/maynghien/BaseModels/SearchRequest";
import { ProgressUpdates } from "../../Models/ProgressUpdates";
import { SearchResponse } from "../maynghien/BaseModels/SearchResponse";
import { ElLoading, ElMessage } from "element-plus";
import { useRoute } from "vue-router";
import { Filter } from "@/components/maynghien/BaseModels/Filter";
import { Right } from "@element-plus/icons-vue";
import { formatDate, RecalculateTheDate } from "../../Service/formatDate";
import { isNumberOrNumericString} from '@/Service/Number'
import Cookies from "js-cookie";

const route = useRoute();
const count = ref(10);
const loading = ref(true);
const noMore = ref(false);
const disabled = ref(true);
const props = defineProps<{
  searchRequest: SearchRequest;
  // test?: string;
}>();
const searchRequest = ref<SearchRequest>({
  PageIndex: 1,
  PageSize: 10,
  filters: props.searchRequest.filters ?? [],
  SortBy: undefined,
});
const searchResponse = ref<SearchResponse<ProgressUpdates[]>>({
  data: undefined,
  totalRows: 0,
  totalPages: 0,
  currentPage: 1,
  rowsPerPage: 0,
});

const listProgressUpdate = ref<ProgressUpdates[]>([]);

const searchProgressUpdate = async () => {
  if (loading.value && noMore.value) return; 
  loading.value = true;
  try{
    // ElLoading.service({ lock: true, text: "Loading", background: "rgba(0, 0, 0, 0.7)" });
    await axiosInstance
    .post("ProgressUpdates/search", searchRequest.value)
    .then((response) => {
      if (!response.data.isSuccess) {
        console.log(response.data.message);
        ElMessage.error(response.data.message);
        noMore.value = true;
        disabled.value = true;
      } else {
        searchResponse.value = response.data.data;
        if(!searchResponse.value){
          searchResponse.value = new SearchResponse();
          searchResponse.value.data = [];
        }
        searchResponse.value.data?.forEach((item) => {
          item.createOn = RecalculateTheDate(item.createOn);
        })
        if(searchResponse.value.data && searchResponse.value.data != null && searchResponse.value.data.length > 0){
          searchRequest.value.PageIndex != undefined ? searchRequest.value.PageIndex  += 1 : searchRequest.value.PageIndex = 1;
          listProgressUpdate.value.push(...searchResponse.value.data!);
        }
        else{
          noMore.value = true;
          disabled.value = true;
        }
      }
    });
    // ElLoading.service().close();
  }
  catch(e){
    console.error(e);
  } finally {
    loading.value = false; 
    disabled.value = false;
    disabled.value = noMore.value;
  }
};
watch(() => props.searchRequest.filters, () => {
  searchRequest.value = props.searchRequest;
  listProgressUpdate.value = [];
  searchRequest.value.PageIndex = 1;
  searchProgressUpdate();
}, { deep: true })
onMounted(() => {
  // console.log(props.test);
  searchProgressUpdate();
})

</script>

<style scoped>
.infinite-list-wrapper {
  max-height: calc(100vh - 200px);
  /* overflow: auto; */
  padding: 0 20px;
}

.list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.list-item {
  margin-bottom: 20px;
}

.point {
  display: flex;
  align-items: center;
  margin-bottom: 10px;
}

.content {
  font-size: 14px;
}



</style>
