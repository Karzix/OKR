<template>
  <el-table :data="data.data" style="width: 100%; height: 200px;" >
    <el-table-column prop="createdOn" label="Date" min-width="100"></el-table-column>
    <el-table-column prop="createdBy" label="Created By" min-width="100"></el-table-column>
    <el-table-column prop="note" label="Note" min-width="140"></el-table-column>
    <el-table-column fixed="right" label="Operations" min-width="120">
      <template  #default="scope">
        <el-button link type="primary" size="small" @click="approve(scope.row)">
          active
        </el-button>
        <el-button link type="primary" size="small" @click="reject(scope.row)">reject</el-button>
      </template>
    </el-table-column>
    <!-- Sử dụng append để thêm hàng tùy chỉnh -->
    <template #append>
      <div style="text-align: center; padding: 10px;" @click="handleAddRow" class="btn-add-item" v-if="!noMore">
        <el-text class="mx-1" type="primary">see more...</el-text>
      </div>
      <div style="text-align: center; padding: 10px;" class="btn-add-item" v-else>
        <el-text class="mx-1" type="primary">no more...</el-text>
      </div>
    </template>
  </el-table>
</template>

<script setup lang="ts">
import  { DepartmentProgressApprovalDto } from '@/Models/DepartmentProgressApprovalDto';
import { onMounted, ref } from "vue";
import {EntityObjectives} from "@/Models/EntityObjectives";
import type { SearchResponse } from '../maynghien/BaseModels/SearchResponse';
import type { SearchRequest } from '../maynghien/BaseModels/SearchRequest';
import { Filter } from '../maynghien/BaseModels/Filter';
import { addFilter } from '../maynghien/Common/handleSearchFilter';
import { deepCopy } from '@/Service/deepCopy';
import { axiosInstance } from '@/Service/axiosConfig';
import { ElMessage } from 'element-plus';



const props = defineProps<{
  objectivesId: string;
}>();
const emit = defineEmits<{
  (e: "onSuccess",request : DepartmentProgressApprovalDto): void;
}>();
const noMore = ref(false);
const data = ref<SearchResponse<DepartmentProgressApprovalDto[]>>({
  data: [] as DepartmentProgressApprovalDto[],
  totalRows: 0,
  totalPages: 0,
  currentPage: 1,
  rowsPerPage: 0,
});
const searchRequest = ref<SearchRequest>({
  PageIndex: 1,
  PageSize: 10,
  filters: [],
  SortBy: undefined,
});
const Search = async () => {
 
  await axiosInstance.post("DepartmentProgressApproval/search",searchRequest.value).then((res) => {
    if(!res.data.data.data || res.data.data.data.length == 0){
      noMore.value = true
    }
    else{
      data.value.data = res.data.data.data;
      searchRequest.value.PageIndex = (searchRequest.value.PageIndex ?? 0)+ 1
    }
    
  });

}
const handleAddRow = () => {
  Search()
}
onMounted(() => {
  var filter = new Filter();
  filter.FieldName = "entityObjectivesId";
  filter.Value = props.objectivesId;
  addFilter(searchRequest.value.filters as [],deepCopy(filter));
  Search()
})
const approve = (request : DepartmentProgressApprovalDto) => {
  request.isApproved = true;
  saveState(request);
};
const reject = (request : DepartmentProgressApprovalDto) => {
  request.isApproved = false;
  saveState(request);
};
const saveState = (request : DepartmentProgressApprovalDto) => {
  axiosInstance
    .put("DepartmentProgressApproval/confirm", request)
    .then((res) => {
      if (!res.data.isSuccess) {
        ElMessage.error(res.data.message);
      } else {
    
      }
      remove(request);
      emit("onSuccess", request); 
    })
};
const remove = (request : DepartmentProgressApprovalDto) => {
  const index = data.value.data?.findIndex(item => item.id === request.id);
  if (index != undefined && index !== -1) {
    data.value.data?.splice(index, 1);
  }
};
</script>
<style>
.btn-add-item :hover{
  cursor: pointer;
}
</style>