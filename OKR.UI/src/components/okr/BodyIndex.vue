<template>
  <div class="infinite-list-wrapper" style="overflow: auto">
    <ul
      v-infinite-scroll="Search"
      class="list"
      :infinite-scroll-disabled="disabled"
    >
      <li
        v-for="item in listObjectives"
        :key="item.id"
        class="list-item"
      >
        <el-card @click="handleDeatail(item)">
          <div style="">
            <h3>
              {{ item.name }}
              <el-icon
                @click="
                  () => {
                    if (isLogin) editObjective(item);
                  }
                "
                ><Edit
              /></el-icon>
            </h3>
            <el-progress :percentage="item.point" :color="customColorMethod" />
          </div>
          <el-tree :data="buildTree(item)" :props="defaultProps" />
        </el-card>
      </li>
    </ul>
  </div>
</template>
<script setup lang="ts">
import type { Objective } from "@/Models/Objective";
import { SearchResponse } from "../../components/maynghien/BaseModels/SearchResponse";
import Cookies from "js-cookie";
import { onMounted, ref } from "vue";
import { axiosInstance } from "@/Service/axiosConfig";
import type { SearchRequest } from "../maynghien/BaseModels/SearchRequest";
import { RecalculateTheDate } from "@/Service/formatDate";
import * as handleSearch from "@/components/maynghien/Common/handleSearchFilter";
import { ElMessage } from "element-plus";

const props = defineProps<{
  searchRequest: SearchRequest;
}>();
const emit = defineEmits<{
  (e: "onEditObjective", objective: Objective): void;
  (e: "onDeatail", objective: Objective): void;
}>();
const defaultProps = {
  children: "children",
  label: "label",
};
interface Tree {
  label: string;
  children?: Tree[];
}
const isLogin = ref(false);
const searchResponseObjectives = ref<SearchResponse<Objective[]>>({
  data: [] as Objective[],
  totalRows: 0,
  totalPages: 0,
  currentPage: 1,
  rowsPerPage: 0,
});
const loading = ref(true);
const noMore = ref(false);
const disabled = ref(true);
const listObjectives = ref<Objective[]>([]);
const customColorMethod = (percentage: number) => {
  if (percentage < 30) {
    return "#909399";
  }
  if (percentage < 70) {
    return "#e6a23c";
  }
  return "#67c23a";
};
const searchRequest = ref<SearchRequest>({
  PageIndex: 1,
  PageSize: 10,
  filters: props.searchRequest.filters ?? [],
  SortBy: undefined,
});
const editObjective = (objective: Objective) => {
  emit("onEditObjective", objective);
};
const handleDeatail = (objective: Objective) => {
  emit("onDeatail", objective);
};
const buildTree = (objective: Objective): Tree[] => {
  var dataTreeTemp = [] as Tree[];
  for (let i = 0; i < objective.listKeyResults?.length; i++) {
    var newTree = {
      label: "",
      children: [] as Tree[],
    } as Tree;
    newTree.label = objective.listKeyResults[i].description ?? "";
    for (
      let j = 0;
      j < (objective.listKeyResults[i]?.sidequests?.length ?? 0);
      j++
    ) {
      newTree.children?.push({
        label: objective.listKeyResults[i].sidequests[j].name ?? "",
        children: [] as Tree[],
      });
    }
    dataTreeTemp.push(newTree);
  }
  return dataTreeTemp;
};
onMounted(() => {
  if (!Cookies.get("accessToken")) {
  } else {
    isLogin.value = true;
  }
});
const Search = async () => {
  if (loading.value && noMore.value) return; 
  loading.value = true;
  try {
    await axiosInstance.post("Objectives/search",searchRequest.value)
    .then((response) => {
      if(!response.data.isSuccess){
        console.log(response.data.message);
        ElMessage.error(response.data.message);
        noMore.value = true;
        disabled.value = true;
      }else{
        searchResponseObjectives.value = response.data.data;
        if(!searchResponseObjectives.value){
          searchResponseObjectives.value = new SearchResponse();
          searchResponseObjectives.value.data = [];
        }
        searchResponseObjectives.value.data?.forEach((item) => {
          item.deadline = RecalculateTheDate(item.deadline);
          item.startDay = RecalculateTheDate(item.startDay);
          item.listKeyResults?.forEach((keyResult) => {
            keyResult.deadline = RecalculateTheDate(keyResult.deadline);
          });
        });
        if(searchResponseObjectives.value.data && searchResponseObjectives.value.data != null){
          searchRequest.value.PageIndex != undefined ? searchRequest.value.PageIndex  += 1 : searchRequest.value.PageIndex = 1;
          listObjectives.value.push(...searchResponseObjectives.value.data!);
        }
        else{
          noMore.value = true;
          disabled.value = true;
        }
      }
    });
    // searchResponseObjectives.value = responeSeach.data.data;
    // if (!searchResponseObjectives.value) {
    //   searchResponseObjectives.value = new SearchResponse();
    //   searchResponseObjectives.value.data = [];
    // }
    // searchResponseObjectives.value.data?.forEach((item) => {
    //   item.deadline = RecalculateTheDate(item.deadline);
    //   item.startDay = RecalculateTheDate(item.startDay);
    //   item.listKeyResults?.forEach((keyResult) => {
    //     keyResult.deadline = RecalculateTheDate(keyResult.deadline);
    //   });
    // });
  } catch (e) {}
};
onMounted(() => {
  Search();
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
</style>