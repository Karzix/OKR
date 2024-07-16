<template>
<button @click="test">click</button>
    
</template>
<script setup lang="ts">
import {axiosInstance} from "../Service/axiosConfig";

const test = async () => {
    const result = await axiosInstance.get("OKR-gateway/Account/test");
    console.log(result.data);
}

</script>