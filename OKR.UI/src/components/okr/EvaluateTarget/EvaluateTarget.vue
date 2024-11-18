<template>
  <div class="infinite-list-wrapper" >
    <ul
      class="list"
    >
      <div class="comment" v-for="(item, index)  in listEvaluateTarget" :key="item.id">
        <p class="comment-header">
          <strong>{{ item.createBy }}</strong> -
          <span class="timestamp">{{ item.createOn ? formatDate_dd_mm_yyyy_hh_mm(item.createOn) : '' }}</span>
        </p>
        <p class="comment-body" v-if="!isEdit">{{ item.content }}</p>
        <div class="comment-body-edit" v-else>
          <el-input v-model="item.content" type="textarea"></el-input>
          <el-button type="primary" @click="Edit(item)"><el-icon><Promotion /></el-icon></el-button>
        </div>
        
        <div class="comment-btn" v-if="Cookies.get('userName') == item.createBy">
          <el-button-group class="ml-4">
            <el-button @click="Delete(item.id ?? '')" :icon="CloseBold" type="danger" plain/>
            <el-button @click="isEdit = !isEdit" :icon="EditPen" type="primary" plain></el-button>
          </el-button-group>
          
        </div>
      </div>
      <p v-if="loading" class="loading-av"><el-icon class="spinning-icon"><Loading /></el-icon></p>
      <p v-if="noMore" class="no-more-text">No more items</p>
    </ul>
    <el-button v-if="!noMore" @click="search" :loading="loading" type="primary" link>Show more...</el-button>
  </div>
  <div class="form-comment" >
    <el-input v-model="content" />
    <el-button @click="AddEvaluateTarget"><el-icon><Promotion /></el-icon></el-button>
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
import { ElLoading, ElMessage, ElMessageBox } from "element-plus";
import { formatDate, RecalculateTheDate, formatDate_dd_mm_yyyy_hh_mm } from "@/Service/formatDate";
import { Loading } from "@element-plus/icons-vue";
import { CloseBold, EditPen, Promotion } from "@element-plus/icons-vue";
import Cookies from "js-cookie";

const route = useRoute();
const loading = ref(false);
const noMore = ref(false);
const disabled = ref(true);
const searchResponse = ref<SearchResponse<EvaluateTarget>>({
  data: [],
  totalRows: 0,
  totalPages: 0,
  currentPage: 1,
  rowsPerPage: 0,
});
const props = defineProps<{
  searchRequest: SearchRequest;
}>();
const searchRequest = ref<SearchRequest>({
  PageIndex: 1,
  PageSize: 10,
  filters: props.searchRequest.filters ?? [],
  SortBy: undefined,
});
const listEvaluateTarget = ref<EvaluateTarget[]>([]);
const content = ref("");
const editContent = ref("");
const isEdit = ref(false);
const search = async () => {
  if (loading.value && noMore.value) return;
  loading.value = true;
  try {
    await axiosInstance
      .post("EvaLuateTarget/search", searchRequest.value)
      .then((res) => {
        if (res.data.isSuccess) {
          searchResponse.value = res.data.data;
          searchResponse.value.data?.forEach((item) => {
            item.createOn = RecalculateTheDate(item.createOn);
          });
          if (
            searchResponse.value.data &&
            searchResponse.value.data != null &&
            searchResponse.value.data.length > 0
          ) {
            searchRequest.value.PageIndex != undefined
              ? (searchRequest.value.PageIndex += 1)
              : (searchRequest.value.PageIndex = 1);
            listEvaluateTarget.value.push(...searchResponse.value.data!);
          } else {
            noMore.value = true;
            disabled.value = true;
          }
          if(searchResponse.value.currentPage == searchResponse.value.totalPages){
            noMore.value = true;
          }
        } else {
          ElMessage({
            message: res.data.message,
            type: "error",
            plain: true,
          });
          noMore.value = true;
          disabled.value = true;
        }
      });
  } catch (e) {
    console.error(e);
  } finally {
    loading.value = false;
    disabled.value = false;
    disabled.value = noMore.value;
  }
};

// onMounted(() => {
//   // var filter = new Filter();
//   // filter.FieldName = "objectivesId";
//   // filter.Value = prop;
//   search();
// });
watch(
  () => props.searchRequest.filters,
  () => {
    searchRequest.value = props.searchRequest;
    listEvaluateTarget.value = [];
    searchRequest.value.PageIndex = 1;
    search();
  },
  { deep: true }
);
// onMounted(() => {
//   searchRequest.value = props.searchRequest;
//   console.log(props.targetType);
//   search();
// });

const AddEvaluateTarget = async () => {
  // const evaluateTarget = new EvaluateTarget();
  const evaluateTarget = new EvaluateTarget();
  evaluateTarget.content = content.value;
  evaluateTarget.objectivesId = props.searchRequest.filters?.filter(x=>x.FieldName == "objectivesId")[0].Value;
  console.log(evaluateTarget);
  await axiosInstance.post("EvaLuateTarget", evaluateTarget).then((res) => {
    if (res.data.isSuccess) {
      ElMessage({
        message: "Add success",
        type: "success",
        plain: true,
      });
      listEvaluateTarget.value = [];
      searchRequest.value.PageIndex = 1;
      noMore.value = false;
      search();
      content.value = "";
    } else {
      ElMessage({
        message: res.data.message,
        type: "error",
        plain: true,
      });
    }
  });

  console.log(props.searchRequest);
};

const Delete = (id: string) => {
  ElMessageBox.confirm(
    'Are you sure you want to delete?',
    'Warning',
    {
      confirmButtonText: 'OK',
      cancelButtonText: 'Cancel',
      type: 'warning',
    }
  )
    .then(() => {
      axiosInstance
      .delete("EvaLuateTarget/" + id)
      .then((res) => {
        if (res.data.isSuccess) {
          ElMessage({
            message: "Delete success",
            type: "success",
            plain: true,
          });
          listEvaluateTarget.value = listEvaluateTarget.value.filter(
            (x) => x.id != id
          );
        } else {
          ElMessage({
            message: res.data.message,
            type: "error",
            plain: true,
          });
        }
      });
    })
    .catch(() => {
      ElMessage({
        type: 'info',
        message: 'Delete canceled',
      })
    })
  
}
const Edit = (item: EvaluateTarget) => {
  console.log(item);
  axiosInstance
    .put("EvaLuateTarget", item)
    .then((res) => {
      if (res.data.isSuccess) {
        ElMessage({
          message: "Edit success",
          type: "success",
          plain: true,
        });
        isEdit.value = false  ;
      }
      else {
        ElMessage({
          message: res.data.message,
          type: "error",
          plain: true,
        }); 
      }
    }
      
    )
}
onMounted(() => {
  search();
});
</script>
<style scoped>
.infinite-list-wrapper {
  /* max-height: calc(100vh - 200px); */
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
  position: relative;
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
p.loading-av {
    text-align: center;
    font-size: 25px;
}
.spinning-icon {
  animation: spin 1s infinite linear;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}
.comment-btn {
  display: block;
  position: absolute;
  right: 0;
  top: 0;
}
.comment-body-edit{
  display: flex;
}
.no-more-text {
  text-align: center;
  color: #999;
  margin-top: 20px;
}
.form-comment{
  display: flex;
}
</style>
