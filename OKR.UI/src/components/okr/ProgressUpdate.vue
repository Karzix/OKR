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
</template>

<script lang="ts" setup>
import { computed, onMounted, ref, watch } from "vue";
import Cookie from "js-cookie";
import { axiosInstance } from "@/Service/axiosConfig";
import { SearchRequest } from "../../components/maynghien/BaseModels/SearchRequest";
import { ProgressUpdates } from "../../Models/ProgressUpdates";
import type { SearchResponse } from "../maynghien/BaseModels/SearchResponse";
import { ElMessage } from "element-plus";
import { useRoute } from "vue-router";
import { Filter } from "@/components/maynghien/BaseModels/Filter";
import { Right } from "@element-plus/icons-vue";
import { formatDate, RecalculateTheDate } from "../../Service/formatDate";
import { isNumberOrNumericString} from '@/Service/Number'
import Cookies from "js-cookie";

const route = useRoute();
const count = ref(10);
const loading = ref(false);
const noMore = ref(false);
const disabled = computed(() => loading.value || noMore.value);
const props = defineProps<{
  searchRequest: SearchRequest;
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
  try{
    await axiosInstance
    .post("ProgressUpdates/search", searchRequest.value)
    .then((response) => {
      if (!response.data.isSuccess) {
        console.log(response.data.message);
        ElMessage.error(response.data.message);
      } else {
        searchResponse.value = response.data.data;
        searchResponse.value.data?.forEach((item) => {
          item.createOn = RecalculateTheDate(item.createOn);
        })
        if(searchResponse.value.data && searchResponse.value.data != null){
          searchRequest.value.PageIndex != undefined ? searchRequest.value.PageIndex  += 1 : searchRequest.value.PageIndex = 1;
          listProgressUpdate.value.push(...searchResponse.value.data!);
        }
      }
    });
  }
  catch(e){
    console.error(e);
  }
};
watch(() => props.searchRequest.filters, () => {
  searchRequest.value = props.searchRequest;
  listProgressUpdate.value = [];
  searchRequest.value.PageIndex = 1;
  searchProgressUpdate();
}, { deep: true })


</script>

<style scoped>
.infinite-list-wrapper {
  height: calc(100vh - 200px);
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
