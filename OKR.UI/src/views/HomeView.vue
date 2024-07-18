<template>
<button @click="test">click</button>
    {{ a }}
</template>
<script setup lang="ts">
import {axiosInstance} from "../Service/axiosConfig";
import { ref } from 'vue';

const a = ref();
const test = async () => {
    const result = await axiosInstance.get("OKR-gateway/Account/test");
    a.value = result.data
}

</script>