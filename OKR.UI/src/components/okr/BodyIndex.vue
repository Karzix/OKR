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
                v-if="Cookies.get('userName') == item.createBy"
                @click.stop="
                  () => {
                    if (isLogin) editObjective(item);
                  }
                "
                ><Edit
              /></el-icon>
              <el-popover 
                placement="top"
                trigger="click"
                width="100%"
                
              >
                <template #reference>
                  <el-badge class="item" 
                  @click.stop="showDialogDepartmentProgressQueue"
                    :value="item.numberOfPendingUpdates" 
                    v-if="item.numberOfPendingUpdates != 0 && (Cookies.get('userName') == item.createBy )" 
                  >
                    <el-icon class="icon-Notification"><BellFilled /></el-icon>
                  </el-badge>
                </template>
                <DepartmentProgressQueue :EntotyOfjectivesId="item.id ?? ''" 
                @close="DialogDepartmentProgressQueueVisible = false"
                @onSuccess="(request) => RecalculateObjectives(request, item)"
                />
              </el-popover>
              
            </h3>
            <el-progress :percentage="item.point" :color="customColorMethod" />
          </div>
          <el-tree :data="buildTree(item)" :props="defaultProps" />
        </el-card>
      </li>
    </ul>
  </div>
  <p v-if="noMore">noMore</p>

  
</template>
<script setup lang="ts">
import { Objective } from "@/Models/Objective";
import { SearchResponse } from "../../components/maynghien/BaseModels/SearchResponse";
import Cookies from "js-cookie";
import { onMounted, ref } from "vue";
import { axiosInstance } from "@/Service/axiosConfig";
import type { SearchRequest } from "../maynghien/BaseModels/SearchRequest";
import { RecalculateTheDate } from "@/Service/formatDate";
import * as handleSearch from "@/components/maynghien/Common/handleSearchFilter";
import { ElMessage } from "element-plus";
import {EntityObjectives} from "@/Models/EntityObjectives";
import { Edit, BellFilled } from "@element-plus/icons-vue";
import { TargetType } from "@/Models/Enum/TargetType";
//@ts-ignore
import DepartmentProgressQueue from "../DepartmentProgressApproval/DepartmentProgressQueue.vue";
import {caculateObjective} from "@/Service/OKR/caculateKeyResult";
import type { DepartmentProgressApprovalDto } from "@/Models/DepartmentProgressApprovalDto";
const props = defineProps<{
  searchRequest: SearchRequest;
}>();
const emit = defineEmits<{
  (e: "onEditObjective", objective: EntityObjectives): void;
  (e: "onDeatail", objective: EntityObjectives): void;
  (e: "onSearch"): void;
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
const searchResponseObjectives = ref<SearchResponse<EntityObjectives[]>>({
  data: [] as EntityObjectives[],
  totalRows: 0,
  totalPages: 0,
  currentPage: 1,
  rowsPerPage: 0,
});
const loading = ref(true);
const noMore = ref(false);
const disabled = ref(true);
const listObjectives = ref<EntityObjectives[]>([]);
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
const DialogDepartmentProgressQueueVisible = ref(false);
const editObjective = (entityObjectives: EntityObjectives) => {
  emit("onEditObjective", entityObjectives);
};
const handleDeatail = (entityObjectives: EntityObjectives) => {
  emit("onDeatail", entityObjectives);
};
const buildTree = (objective: EntityObjectives): Tree[] => {
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
  console.log("search_bodyIndex", searchRequest.value);
  if (loading.value && noMore.value) return; 
  loading.value = true;
  try {
    await axiosInstance.post("EntityObjectives/search",searchRequest.value)
    .then((rs) => {
      if(!rs.data.isSuccess){
        console.log(rs.data.message);
        ElMessage.error(rs.data.message);
        noMore.value = true;
        disabled.value = true;
      }else{
        searchResponseObjectives.value = rs.data.data;
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
        if(searchResponseObjectives.value.data && searchResponseObjectives.value.data != null && searchResponseObjectives.value.data.length > 0){
          searchRequest.value.PageIndex != undefined ? searchRequest.value.PageIndex  += 1 : searchRequest.value.PageIndex = 1;
          listObjectives.value.push(...searchResponseObjectives.value.data!);
        }
        else{
          noMore.value = true;
          disabled.value = true;
        }
      }
    });
  } catch (e) {}
};
onMounted(() => {
  Search();
});
const showDialogDepartmentProgressQueue = () => {
  DialogDepartmentProgressQueueVisible.value = true;
  console.log("showDialogDepartmentProgressQueue");
}
const RecalculateObjectives = (request : DepartmentProgressApprovalDto, item : EntityObjectives) => {
  const keyResult = item.listKeyResults.filter(x => x.id == request.keyresultID)[0];
  if (keyResult) {
    keyResult.currentPoint = request.addedPoints + (keyResult.currentPoint ?? 0);
    item.point = caculateObjective(item);
    emit("onSearch");
  }
}
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
</style>
<style>
.icon-Notification{
  font-size: 22px;
  color: #00cbff;
}
</style>