<template>
  <div class="infinite-list-wrapper">
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
    <p v-if="loading" class="loading-text">Loading...</p>
    <p v-if="noMore" class="no-more-text">No more</p>
  </div>
</template>

<script lang="ts" setup>
import { computed, onMounted, ref } from "vue";
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

const route = useRoute();
const count = ref(10);
const loading = ref(false);
const noMore = ref(false);
const disabled = computed(() => loading.value || noMore.value);

const searchRequest = ref<SearchRequest>({
  PageIndex: 1,
  PageSize: 10,
  filters: [],
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
const searchProgressUpdate = () => {
  loading.value = true;
  try{
    axiosInstance
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
          noMore.value = false;
        }
        else{
          noMore.value = true
        }
      }
    });
  }
  catch(e){
    console.error(e);
  }
  loading.value = false;
};
onMounted(() => {
  var userName = route.params.userName;
  if (userName && userName.toString() != "") {
    var newFilter = new Filter();
    newFilter.FieldName = "createBy";
    newFilter.Value = userName.toString();
    searchRequest.value.filters?.push(newFilter);
  }
  searchProgressUpdate();
});

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

.loading-text, .no-more-text {
  text-align: center;
  color: #888;
  margin-top: 20px;
}

.el-card {
  padding: 10px;
}

.el-icon {
  margin: 0 5px;
}
</style>
