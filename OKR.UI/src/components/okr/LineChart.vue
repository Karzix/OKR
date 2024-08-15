<template>
  <div class="line-chart-container">
    <div class="line-chart">
      <canvas id="acquisitions"></canvas>
    </div>
  </div>
  
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import Chart from "chart.js/auto";
import type { SearchRequest } from "../maynghien/BaseModels/SearchRequest";
import { axiosInstance } from "@/Service/axiosConfig";
import type { AppResponse } from "../maynghien/BaseModels/AppResponse";
import { DataChart } from "@/Models/DataChart";
import { RecalculateTheDate } from "@/Service/formatDate";

const labels = ref<string[]>([""]);
const customLabels = ref<string[]>([""]);
const point = ref<number[]>([0]);

const respone = ref<AppResponse<DataChart[]>>({
  isSuccess: true,
  message: "",
  data: [],
});

const config = ref();
const props = defineProps<{
  searchRequest: SearchRequest;
}>();
const build = () => {
  const n = respone.value.data?.length ?? 0;
  labels.value = [];
  customLabels.value = [];
  point.value = [];
  point.value.push(0);
  labels.value.push("");
  customLabels.value.push("");
  if (respone.value.data)
    for (let i = 0; i < n; i++) {
      const dataPoint = respone.value.data[i];
      dataPoint.date = RecalculateTheDate(dataPoint.date)
      labels.value.push(new Date(dataPoint.date ?? '').toLocaleDateString("en-GB") + " " + new Date(dataPoint.date ?? '').toLocaleTimeString("en-GB"));
      customLabels.value.push(dataPoint.label);
      point.value.push(dataPoint.objectivesCompletionRate ?? 0);
    }
};
const load = () => {
  config.value = {
    type: "line",
    data: {
      labels: labels.value,
      datasets: [
        {
          label: "",
          data: point.value,
          fill: false,
        },
      ],
    },
    options: {
      plugins: {
        tooltip: {
          callbacks: {
            label: function (context : any) {
              const label = context.dataset.label || "";
              const index = context.dataIndex;

              return customLabels.value[index];
            },
          },
        },
      },
    },
  };
  new Chart(
    document.getElementById("acquisitions") as HTMLCanvasElement,
    config.value
  );
};

const getdata = async () =>{
  try {
    await axiosInstance.post("ProgressUpdates/data-chart", props.searchRequest).then((res) => {
      respone.value = res.data;
    }).then(() => {
      build();
    }).then(() => {
      load();
    })
  } catch (error) {
    console.error("Error fetching data:", error);
  }
}
getdata();
</script>
<style scoped>
.line-chart-container {
  overflow-x: auto;
}

.line-chart {
  min-width: 400px;
  max-width: 100%;
}
</style>