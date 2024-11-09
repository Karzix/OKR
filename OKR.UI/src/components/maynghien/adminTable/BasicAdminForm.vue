
<template>
  <MnActionPane :allowAdd="allowAdd" :tableColumns="tableColumns" :isEdit="isEditting"
    @onBtnSearchClicked="handleBtnSearchClicked" @onBtnAddClicked="handleOpenCreate" :CustomActions="CustomButtons"
    :openDialog="openDialogCreate" @onCustomAction="handleCustomAction">
  </MnActionPane>
  <el-select v-model="pageSize" class="m-2" placeholder="Select" size="small" v-if="changePageSize == true">
    <el-option v-for="item in pageSizeSelect" :key="item" :label="item" :value="item" />
  </el-select>
  <MnTable :columns="tableColumns" :datas="datas" :onSaved="handleSaved" :enableEdit="allowEdit"
    :enableDelete="allowDelete" :onCloseClicked="handleOnEditCloseClicked" @onEdit="handleEdit" @onDelete="handleDelete"
    :CustomActions="CustomRowActions" @on-custom-action="handleCustomAction" @onSortChange="handleSortChange"
    :scroll="scroll" :loadding="loadding" />


  <el-select v-model="pageSize" class="m-2" placeholder="Select" size="small" v-if="changePageSize == true">
    <el-option v-for="item in pageSizeSelect" :key="item" :label="item" :value="item" />
  </el-select>

  <el-pagination small background layout="prev, pager, next" :total="totalItem" :page-size="pageSize"
    @current-change="handlePageChange" :current-page="searchRequest.PageIndex" class="mt-4" />
  Found {{ totalItem }} results. Page {{ searchRequest.PageIndex }} of total {{ totalPages }} pages


  <MnEditItem ref="MnEdit" :columns="tableColumns" :apiName="apiName" :openDialog="openDialogCreate" :title="title"
    :createUrl="createUrl" :editUrl="editUrl" :editItem="EdittingItem" :isEdit="isEditting" @onSaved="handleSaved"
    @onCloseClicked="handleOnEditCloseClicked" />
</template>
  
<script setup lang="ts">
import MnTable from './MnTable.vue'
import MnActionPane from './MnActionPane.vue'
import MnEditItem from './MnEditItem.vue'
import { ref, watch } from 'vue';
import { TableColumn } from './Models/TableColumn'
import { SearchDTOItem } from './Models/SearchDTOItem'
import { handleAPICustom, handleAPIDelete, handleAPISearch } from './Service/BasicAdminService'
import { Filter } from '../BaseModels/Filter';
import { SearchResponse } from '../BaseModels/SearchResponse';
import { SearchRequest } from '../BaseModels/SearchRequest';
import { ElMessage } from 'element-plus';
import type { CustomAction, CustomActionResponse } from './Models/CustomAction';
import { SortByInfo } from '../BaseModels/SortByInfo';
import Cookies from 'js-cookie';
//#region Method

// Get the token from the cookies
const getCookie = (name: string): string | null => {
  const value = `; ${document.cookie}`;
  const parts = value.split(`; ${name}=`);
  if (parts.length === 2) return parts.pop()?.split(';').shift() || null;
  return null;
};

const pageSizeSelect = ref<number[]>([10, 50, 100, 200, 500, 1000]);
const pagesizeCookie = getCookie('pageSize');
const pageSize = ref<number>(10);
if (pagesizeCookie) {
  pageSize.value = parseInt(pagesizeCookie);
}


const loadding = ref<boolean>(false);
const Search = async () => {
  console.log(props.searchUrl);
  searchRequest.PageSize = pageSize.value;
  loadding.value = true;
  var searchApiResponse = await handleAPISearch(searchRequest, props.apiName, props.searchUrl ? props.searchUrl : undefined);
  if (searchApiResponse.isSuccess && searchApiResponse.data != undefined) {
    let dataresponse: SearchResponse<SearchDTOItem[] | undefined> = searchApiResponse.data;

    if (dataresponse != undefined && dataresponse.data != undefined && dataresponse.data.length > 0) {
      datas.value = dataresponse.data;
      if (dataresponse.totalPages != undefined)
        totalPages.value = dataresponse.totalPages;
      else
        totalPages.value = 0;
      if (dataresponse.totalRows != undefined) {
        totalItem.value = dataresponse.totalRows;
      }
      else
        totalItem.value = 0;
    }
    else {
      datas.value = [];
    }
  }
  loadding.value = false
}

//#endregion
//#region main

const props = defineProps<{
  tableColumns: TableColumn[];
  apiName: string;
  createUrl?: string;
  editUrl?: string;
  allowAdd: boolean;
  allowEdit: boolean;
  allowDelete: boolean;
  title: string;
  CustomActions: CustomAction[];
  CustomFilters?: Filter[];
  isEditedOutSide?: boolean;

  scroll?: boolean;
  changePageSize?: boolean;

  searchUrl?: string;
}>();
const emit = defineEmits<{

  (e: 'onCustomAction', item: CustomActionResponse): void;
}>()
let datas = ref<SearchDTOItem[]>([]);
const totalPages = ref(0);
const totalItem = ref(10);


let searchRequest: SearchRequest = {
  PageIndex: 1,
  PageSize: pageSize.value,
  filters: props.CustomFilters,
  SortBy: undefined
}
const CustomButtons = ref<CustomAction[]>([{}]);
const CustomRowActions = ref<CustomAction[]>([{}]);

Search();
//#endregion
//#region variable
const SelectedRowId = ref<string | null>(null);
const EdittingItem = ref<SearchDTOItem>({});
const openDialogCreate = ref<boolean>(false);
const isEditting = ref(false);
//#endregion

//#region event funcs
const handleBtnSearchClicked = (filters: Filter[]) => {
  filters = filters.concat(props.CustomFilters ?? []);
  searchRequest.filters = filters;
  searchRequest.PageIndex = 1;
  Search();

}
const handleSaved = async () => {
  openDialogCreate.value = false;
  searchRequest.PageIndex = searchRequest.PageIndex;
  EdittingItem.value = new SearchDTOItem(props.tableColumns);
  Search();
}
const handleOnEditCloseClicked = async () => {
  openDialogCreate.value = false;
  EdittingItem.value = new SearchDTOItem(props.tableColumns);
}
type OpenCreateDialogType = () => void;
type ChildMethodType = () => void;

// // Explicitly specify the type of the injected value with a default value
// const childMethod: ChildMethodType|undefined = inject('childMethod', undefined);

// const OpenCreateDialog: OpenCreateDialogType = inject('OpenDialogEditItem', undefined);

const handleOpenCreate = async () => {
  console.log("open create");

  EdittingItem.value = new SearchDTOItem(props.tableColumns);

  isEditting.value = false;
  openDialogCreate.value = true;
}

const handleDelete = async (item: SearchDTOItem) => {
  var id = item.toString();
  var deleteresult = await handleAPIDelete(id, props.apiName);
  if (deleteresult.isSuccess) {
    ElMessage({
      message: 'row deleted.',
      type: 'success',
    });
    await Search();
  }
  else {
    ElMessage({
      message: 'row not deleted.',
      type: 'error',
    });
  }
}
const handleSortChange = async (event: any) => {
  const sortByInfo: SortByInfo = {
    FieldName: event.column.property,
    Ascending: event.column.order != "descending"

  }
  searchRequest.SortBy = sortByInfo;
  searchRequest.PageIndex = 1;
  await Search();
};
const SelectedId = ref("");
//provide('OpenDialogCreateItem', openDialogCreate);
const handleEdit = async (item: SearchDTOItem) => {
  EdittingItem.value = { ...item };
  isEditting.value = true;
  openDialogCreate.value = true;
}
const handleCustomAction = async (item: CustomActionResponse) => {
  if (item.Action.ApiAction != undefined) {
    var url: string = props.apiName + "/" + item.Action.ActionName;
    var apiResult = await handleAPICustom(item.Data, item.Action, url);
    console.log(apiResult);

    if (!apiResult.isSuccess) {
      console.log(apiResult);
      return;
    }
    else {
      searchRequest.PageIndex = 1;
      await Search();
    }
  }
  else {
    emit("onCustomAction", item);
  }
}
const handlePageChange = async (value: number) => {
  searchRequest.PageIndex = value;
  await Search();
}
//#endregion

watch(() => props.CustomActions, () => {
  CustomButtons.value = props.CustomActions.filter(m => m.IsRowAction == false);
  CustomRowActions.value = props.CustomActions.filter(m => m.IsRowAction == true);
  console.log(CustomRowActions);
}, { immediate: true })

watch(() => props.isEditedOutSide, () => {
  if (props.isEditedOutSide != undefined && props.isEditedOutSide == true) {
    Search();
  }
}, { immediate: true })

watch(() => pageSize.value, () => {
  Cookies.set('pageSize', pageSize.value.toString(), { expires: undefined });
  Search();
})
defineExpose({ Search });

</script>