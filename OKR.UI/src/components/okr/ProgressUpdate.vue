<template>
  <div class="infinite-list-wrapper" style="overflow: auto">
    <ul class="list">
      <li v-for="item in listProgressUpdate" :key="item.id" class="list-item">
        <el-card>
          <div class="point" v-if="isNumberOrNumericString(item.oldPoint) && isNumberOrNumericString(item.newPoint)">
            <strong>{{ item.oldPoint }}</strong>
            <el-icon><Right /></el-icon>
            <strong>{{ item.newPoint }}</strong>
            <el-tag>
              {{ item.unit == 0 ? "Percent" : item.unit == 1 ? "Value" : "Checked" }}
            </el-tag>
          </div>
          <div class="content">
            <p>Date: <strong>{{ formatDate(item.createOn) }}</strong></p>
            <p>{{ item.note }}</p>
          </div>
        </el-card>
      </li>
      <p v-if="loading" class="loading-av"><el-icon class="spinning-icon"><Loading /></el-icon></p>
      <p v-if="noMore" class="no-more-text">No more items</p>
    </ul>
    <el-button v-if="!noMore" @click="searchProgressUpdate" :loading="loading" type="primary" link>Show more...</el-button>
    <!-- <p v-if="noMore" class="no-more-text">No more items</p> -->
  </div>
</template>

<script lang="ts" setup>
import { ref, watch, onMounted } from "vue";
import { axiosInstance } from "@/Service/axiosConfig";
import { ElMessage } from "element-plus";
import { useRoute } from "vue-router";
import { formatDate, RecalculateTheDate } from "../../Service/formatDate";
import { isNumberOrNumericString } from '@/Service/Number';
import { Right } from "@element-plus/icons-vue";
import { SearchRequest } from "../../components/maynghien/BaseModels/SearchRequest";
import { ProgressUpdates } from "../../Models/ProgressUpdates";
import { SearchResponse } from "../maynghien/BaseModels/SearchResponse";
import { Loading } from "@element-plus/icons-vue";

const route = useRoute();
const loading = ref(false);
const noMore = ref(false);
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
  if (loading.value || noMore.value) return;
  loading.value = true;
  
  try {
    const response = await axiosInstance.post("ProgressUpdates/search", searchRequest.value);
    if (!response.data.isSuccess) {
      ElMessage.error(response.data.message);
      noMore.value = true;
    } else {
      searchResponse.value = response.data.data || { data: [] };
      searchResponse.value.data?.forEach(item => {
        item.createOn = RecalculateTheDate(item.createOn);
      });
      
      if (searchResponse.value.data && searchResponse.value.data.length > 0) {
        searchRequest.value.PageIndex = (searchRequest.value.PageIndex || 1) + 1;
        listProgressUpdate.value.push(...searchResponse.value.data);
      } else {
        noMore.value = true;
      }
    }
  } catch (e) {
    console.error(e);
  } finally {
    loading.value = false;
  }
};

watch(() => props.searchRequest.filters, () => {
  searchRequest.value = props.searchRequest;
  listProgressUpdate.value = [];
  searchRequest.value.PageIndex = 1;
  searchProgressUpdate();
}, { deep: true });

onMounted(() => {
  searchProgressUpdate();
});

</script>

<style scoped>
.infinite-list-wrapper {
  max-height: calc(100vh - 200px);
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

.load-more {
  text-align: center;
  margin: 20px 0;
}

.no-more-text {
  text-align: center;
  color: #999;
  margin-top: 20px;
}

</style>
