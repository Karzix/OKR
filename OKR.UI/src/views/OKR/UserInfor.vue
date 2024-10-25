<template>
  <div id="info-user" class="container">
    <div class="user-info">
      <el-card class="user-card">
        <p><strong>Name:</strong> {{ User.email }}</p>
        <p><strong>Department:</strong> {{ User.departmentName }}</p>
      </el-card>
    </div>
    <div class="list-objective">
      <el-card>
        <template #header>
          <div class="card-header">
            <span>Individual</span>
            <el-progress :percentage="percentage" :color="customColors" />
          </div>
        </template>
        <div v-for="item in Individual" :key="item.id" class="objective-card" @click="DetailObjectives(item)">
          <p><strong>{{ item.name }}</strong></p>
          <el-progress :percentage="item.point" :color="customColors" />
          <el-tree :data="buildTree(item)" :props="defaultProps" class="objective-tree" />
        </div>
        <p class="read-more" @click="handleShowDialog('0')">Read More</p>
      </el-card>

      <el-card>
        <template #header>
          <div class="card-header">
            <span>Branch</span>
            <el-progress :percentage="percentage" :color="customColors" />
          </div>
        </template>
        <div v-for="item in Branch" :key="item.id" class="objective-card">
          <p><strong>{{ item.name }}</strong></p>
          <el-progress :percentage="item.point" :color="customColors" />
          <el-tree :data="buildTree(item)" :props="defaultProps" class="objective-tree" />
        </div>
        <p class="read-more" @click="handleShowDialog('1')">Read More</p>
      </el-card>

      <el-card>
        <template #header>
          <div class="card-header">
            <span>Team</span>
            <el-progress :percentage="percentage" :color="customColors" />
          </div>
        </template>
        <div v-for="item in Team" :key="item.id" class="objective-card">
          <p><strong>{{ item.name }}</strong></p>
          <el-progress :percentage="item.point" :color="customColors" />
          <el-tree :data="buildTree(item)" :props="defaultProps" class="objective-tree" />
        </div>
        <p class="read-more" @click="handleShowDialog('2')">Read More</p>
      </el-card>
    </div>
  </div>

  <el-dialog v-model="showDialog">
    <DialogListObjectives
      v-if="showDialog"
      :filters="DialogListObjectives_SearchRequest.filters"
      :title="targetType"
      :showDialog="showDialog"
    ></DialogListObjectives>
  </el-dialog>
</template>

<script setup lang="ts">
import { onMounted, ref } from "vue";
import { Minus, Plus } from "@element-plus/icons-vue";
import { SearchRequest } from "@/components/maynghien/BaseModels/SearchRequest";
import { axiosInstance } from "@/Service/axiosConfig";
import { SearchResponse } from "@/Models/SearchResponse";
import type { Objective } from "@/Models/Objective";
import {
  addFilter,
  removeFilter,
} from "@/components/maynghien/Common/handleSearchFilter";
import { Filter } from "@/components/maynghien/BaseModels/Filter";
import { buildTree } from "@/Service/OKR/buildTree";
import DialogListObjectives from "@/components/okr/DialogListObjectives.vue";
import * as handleSearch  from "@/components/maynghien/Common/handleSearchFilter";
import SeeObjectives from "./SeeObjectives.vue"
import { useRoute } from "vue-router";
import router from "@/router";
import { deepCopy } from "@/Service/deepCopy";
import { UserModel } from "@/Models/UserModel";
import { TargetType } from "@/Models/Enum/TargetType";

const route = useRoute();
const percentage = ref(50);
const showDialog = ref(false);
const customColors = [
  { color: "#909399", percentage: 40 },
  { color: "#e6a23c", percentage: 70 },
  { color: "#67c23a", percentage: 100 },
];
const defaultProps = {
  children: "children",
  label: "label",
};
const Individual = ref<Objective[]>([]);
const Branch = ref<Objective[]>([]);
const Team = ref<Objective[]>([]);
const User = ref<UserModel>({
  userName: "",
  password: "",
  email: "",
  role: "",
  token: "",
  refreshToken: "",
  id: "",
  departmentName: "",
});
const DialogListObjectives_SearchRequest = ref<SearchRequest>({
  PageIndex: 1,
  PageSize: 10,
  filters: [],
  SortBy: undefined,
});
const targetType = ref<string>("");
const Search = async (searchRequest: SearchRequest, url: string): Promise<Objective[]> => {
  let data = new SearchResponse<Objective>();
  data.data = [];
  try {
    await axiosInstance.post(url, searchRequest).then((res) => {
      data = res.data.data;
      console.log(data);
    });
    return data.data;
  } catch (e) {
    console.error(e);
    return data.data;
  }
};

onMounted(async () => {
  var searchRequest = new SearchRequest();
  searchRequest.PageIndex = 1;
  searchRequest.PageSize = 5;
  searchRequest.filters = [];
  var filtercreateBy = new Filter();

  filtercreateBy.FieldName = "createBy";
  filtercreateBy.Value = route.params.UserName.toString();
  addFilter(searchRequest.filters, deepCopy(filtercreateBy));

  var filtertargetType = new Filter();
  filtertargetType.FieldName = "targetType";
  filtertargetType.Value = "0";

  addFilter(searchRequest.filters, deepCopy(filtertargetType)); 
  Individual.value = await Search(searchRequest,"EntityObjectives/search");
  searchRequest.filters = [];
  filtertargetType.Value = "1";
  addFilter(searchRequest.filters, deepCopy(filtertargetType));
  Branch.value = await Search(searchRequest,"EntityObjectives/search");
  searchRequest.filters = [];
  filtertargetType.Value = "2";
  addFilter(searchRequest.filters, deepCopy(filtertargetType));
  Team.value = await Search(searchRequest,"EntityObjectives/search");
});

const handleShowDialog = (TargetType: string) => {
  var filter = new Filter();
  filter.FieldName = "targetType";
  filter.Value = TargetType;
  handleSearch.addFilter(DialogListObjectives_SearchRequest.value.filters as [], filter);
  targetType.value = TargetType;

  showDialog.value = true;
};

const DetailObjectives  = (objective: Objective) => {
  router.push('objectives=' + objective.id+ '&'+objective.targetType);
}
const getUser = async () => {
  var usernamme = route.params.UserName.toString();
  await axiosInstance.get(`User/${usernamme}`).then((res) => {
    console.log(res.data.data);
    User.value = res.data.data
  })
}
getUser();
</script>
<style scoped>
.container {
  display: flex;
  flex-direction: column;
  align-items: center; /* Center the items horizontally */
  padding: 20px;
  background-color: #f4f6f8; /* Light background for better contrast */
}

/* User Info Card */
.user-info {
  width: 100%;
  max-width: 600px; /* Limit the width for larger screens */
  margin-bottom: 20px; /* Spacing between user info and objectives */
}

.user-card {
  border-radius: 8px;
  border: 1px solid #e0e4e8;
  background-color: #ffffff; /* White background for cards */
  padding: 20px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1); /* Subtle shadow for depth */
}

/* Objective Cards */
.list-objective {
  display: flex;
  flex-direction: column;
  gap: 20px; /* Space between cards */
  width: 100%;
  max-width: 800px; /* Limit the width of the objective cards */
}

/* Individual objective card */
.objective-card {
  border-radius: 8px;
  border: 1px solid #e0e4e8;
  background-color: #ffffff; /* Keep the white background */
  padding: 15px;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1); /* Subtle shadow for depth */
  transition: transform 0.2s; /* Smooth transition for hover effect */
}

.objective-card:hover {
  transform: translateY(-5px); /* Lift effect on hover */
}

/* Card header styles */
.card-header {
  display: flex;
  justify-content: space-between; /* Space between title and progress */
  align-items: center; /* Center vertically */
  padding: 10px 15px; /* Padding for the header */
  background-color: #f0f4f8; /* Light background for header */
  border-bottom: 1px solid #e0e4e8; /* Bottom border for separation */
}

/* Read More link */
.read-more {
  color: #007bff; /* Bootstrap primary color */
  cursor: pointer; /* Pointer cursor on hover */
  text-align: center; /* Center the link */
  margin-top: 10px; /* Space above the link */
}

.read-more:hover {
  text-decoration: underline; /* Underline effect on hover */
}

/* Responsive Design */
@media (max-width: 768px) {
  .container {
    padding: 10px; /* Reduce padding on smaller screens */
  }

  .user-card,
  .objective-card {
    padding: 15px; /* Reduce padding for cards on smaller screens */
  }

  .list-objective {
    gap: 15px; /* Reduce gap between cards */
  }
}
</style>