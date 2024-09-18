<template>
  <div class="infinite-list-wrapper" style="overflow: auto">
    <ul
      v-infinite-scroll="search"
      class="list"
      :infinite-scroll-disabled="disabled"
    >
  
  </ul>
    <div class="comment" v-for="item in listEvaluateTarget" :key="item.id">
      <p class="comment-header">
        <strong>{{ item.createBy }}</strong> -
        <span class="timestamp">{{ formatDate(item.createOn) }}</span>
      </p>
      <p class="comment-body">{{ item.content }}</p>
    </div>
  </div>
  <div>
    <el-input v-model="content"/>
    <el-button @click="AddEvaluateTarget">Add</el-button>
  </div>
</template>
<script setup lang="ts">
import type { SearchRequest } from "@/components/maynghien/BaseModels/SearchRequest";
import { computed, onMounted, ref, watch } from "vue";
import * as handleSearch from "@/components/maynghien/Common/handleSearchFilter";
import { Filter } from "@/components/maynghien/BaseModels/Filter";
import { useRoute } from "vue-router";
import axios from "axios";
import { axiosInstance } from "@/Service/axiosConfig";
import type { SearchResponse } from "@/Models/SearchResponse";
import { EvaluateTarget } from "@/Models/EvaluateTarget";
import { ElMessage, ElMessageBox } from "element-plus";
import { formatDate, RecalculateTheDate } from "@/Service/formatDate";

const route = useRoute();
const loading = ref(false);
const noMore = ref(false);
const disabled = computed(() => loading.value || noMore.value);
const searchResponse = ref<SearchResponse<EvaluateTarget>>({
  data: [],
  totalRows: 0,
  totalPages: 0,
  currentPage: 1,
  rowsPerPage: 0,
});
const props = defineProps<{
  searchRequest: SearchRequest;
  targetType: string
}>();
const searchRequest = ref<SearchRequest>({
  PageIndex: 1,
  PageSize: 10,
  filters: props.searchRequest.filters ?? [],
  SortBy: undefined,
});
const listEvaluateTarget = ref<EvaluateTarget[]>([]);
const content = ref("");
const search = async () => {
  axiosInstance
    .post("EvaLuateTarget/search", searchRequest.value)
    .then((res) => {
      if (res.data.isSuccess) {
        searchResponse.value = res.data.data;
        searchResponse.value.data?.forEach((item) => {
          item.createOn = RecalculateTheDate(item.createOn);
        })
        if(searchResponse.value.data && searchResponse.value.data != null){
          searchRequest.value.PageIndex != undefined ? searchRequest.value.PageIndex  += 1 : searchRequest.value.PageIndex = 1;
          listEvaluateTarget.value.push(...searchResponse.value.data!);
        }
      } else {
        ElMessage({
          message: res.data.message,
          type: "error",
          plain: true,
        });
      }
    });
};

onMounted(() => {
  // var filter = new Filter();
  // filter.FieldName = "objectivesId";
  // filter.Value = prop;
  search();
});
watch(() => props.searchRequest.filters, () => {
  searchRequest.value = props.searchRequest;
  listEvaluateTarget.value = [];
  searchRequest.value.PageIndex = 1;
  search();
}, { deep: true })


const AddEvaluateTarget = async () => {
  // const evaluateTarget = new EvaluateTarget();
  const evaluateTarget = new EvaluateTarget();
  if(props.targetType == '0'){
    evaluateTarget.userObjectivesId = props.searchRequest.filters?.filter(x => x.FieldName == "objectivesId")[0].Value as string;
  }
  else{
    evaluateTarget.departmentObjectivesId = props.searchRequest.filters?.filter(x => x.FieldName == "objectivesId")[0].Value as string;
  }
  evaluateTarget.content = content.value;
  console.log(evaluateTarget)
  await axiosInstance.post("EvaLuateTarget", evaluateTarget).then((res) => {
    if(res.data.isSuccess){
      ElMessage({
        message: "Add success",
        type: "success",
        plain: true,
      });
      listEvaluateTarget.value = [];
      searchRequest.value.PageIndex = 1;
      search();
    }
    else{
      ElMessage({
        message: res.data.message,
        type: "error",
        plain: true,
      });
    }
  })

  console.log(props.searchRequest)
}
</script>
<style scoped>
.infinite-list-wrapper {
  max-height: calc(100vh - 200px);
  /* overflow: auto; */
  padding: 0 20px;
}

.comment {
  background-color: #f9f9f9;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  padding: 16px;
  margin-bottom: 12px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  transition: box-shadow 0.3s ease-in-out;
}

.comment:hover {
  box-shadow: 0 6px 12px rgba(0, 0, 0, 0.15);
}

.comment-header {
  color: #333;
  font-size: 14px;
  margin-bottom: 8px;
}

.timestamp {
  color: #888;
  font-size: 12px;
}

.comment-body {
  font-size: 14px;
  line-height: 1.6;
  color: #555;
  margin-top: 8px;
}
</style>
