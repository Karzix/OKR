<template>
  <el-card style="" v-for="item in props.data.data" @click=" handleDeatail(item)">
    <div style="">
      <h3>
        {{ item.name }} <el-icon @click="() =>{if(isLogin)editObjective(item)}"><Edit /></el-icon>
      </h3>
      <el-progress :percentage="item.point" :color="customColorMethod" />
    </div>
    <el-tree :data="buildTree(item)" :props="defaultProps" />
  </el-card>
</template>
<script setup lang="ts">
import type { Objective } from '@/Models/Objective';
import { SearchResponse } from '../../components/maynghien/BaseModels/SearchResponse';
import Cookies from 'js-cookie';
import { onMounted, ref } from 'vue';


const props = defineProps<{
  data: SearchResponse<Objective[]>;
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
const isLogin = ref(false)
const customColorMethod = (percentage: number) => {
  if (percentage < 30) {
    return "#909399";
  }
  if (percentage < 70) {
    return "#e6a23c";
  }
  return "#67c23a";
};
const editObjective = (objective: Objective) =>{
    emit("onEditObjective",objective)
}
const handleDeatail = (objective: Objective) => {
    emit("onDeatail",objective)
}
const buildTree = (objective: Objective) : Tree[] => {
  var dataTreeTemp = [] as Tree[] 
  for(let i  = 0; i < objective.listKeyResults?.length; i++) {
    var newTree = {
          label: "",
          children:[] as Tree[]
        } as Tree
    newTree.label= objective.listKeyResults[i].description ?? '';
    for(let j  = 0; j < (objective.listKeyResults[i]?.sidequests?.length ?? 0); j++) {
      newTree.children?.push({
        label: objective.listKeyResults[i].sidequests[j].name ?? '',
        children:[] as Tree[]
      })
    }
    dataTreeTemp.push(newTree);
  }
  return dataTreeTemp
}
onMounted(() => {
  if(!Cookies.get("accessToken")){

  }
  else{
    isLogin.value = true
  }
})
</script>