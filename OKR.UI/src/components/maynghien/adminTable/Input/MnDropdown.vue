<template>
  <el-select v-model="editItem" class="m-2" placeholder="Select" size="large" @change="dropdownChange">
    <el-option v-for="item in dropdownData" :key="item[column.dropdownData.keyMember]"
      :label="item[column.dropdownData.displayMember]" :value="item[column.dropdownData.keyMember]" />
  </el-select>
  
</template>
<script setup lang="ts">
import { ref, watch } from 'vue';
import { TableColumn } from '@/components/maynghien/adminTable/Models/TableColumn';
import { SearchDTOItem } from '@/components/maynghien/adminTable/Models/SearchDTOItem';
import { handleAPIGetDropdownList } from '../Service/BasicAdminService';

const props = defineProps<{
  column: TableColumn;
  modelValue?: string;


}>();
const editItem = ref<string | undefined>("");
const dropdownData = ref<any[]>([]);
const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void;
  (e: 'changed', key: string|undefined, value: string): void;

}>();

const dropdownChange = (value: any): void => {


  emit('changed', props.column.key, value);
}
watch(() => props.modelValue, () => {
  console.log(props.modelValue);
  editItem.value = props.modelValue;

}, { immediate: true })
watch(() => props.column, async () => {
  if (props.column.dropdownData.apiUrl != undefined) {
    var data = await handleAPIGetDropdownList(props.column.dropdownData.apiUrl);
    if (data != undefined && data.data) {
      dropdownData.value = data.data;
    }
  }
  else {
    dropdownData.value = props.column.dropdownData.data;
  }

}, { immediate: true })

</script>