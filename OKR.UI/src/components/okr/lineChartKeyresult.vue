<template>
  <div class="line-chart-container" style="max-width: 800px">
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
import { KeyResult } from "@/Models/KeyResult";

const labels = ref<string[]>([""]);
const customLabels = ref<string[]>([""]);
const point = ref<number[]>([0]);

const props = defineProps<{
    keyResult: KeyResult;
}>();
const config = ref();
const build = () => {
  const n = props.keyResult?.progressUpdates?.length ?? 0;
  labels.value = [];
  customLabels.value = [];
  point.value = [];
  
 
  if (props.keyResult.progressUpdates)
    for (let i = 0; i < n; i++) {
      const dataPoint = props.keyResult.progressUpdates[i];
      dataPoint.createOn = RecalculateTheDate(dataPoint.createOn);
      labels.value.push(
        new Date(dataPoint.createOn ?? "").toLocaleDateString("en-GB") +
          " " +
          new Date(dataPoint.createOn ?? "").toLocaleTimeString("en-GB")
      );
      customLabels.value.push(dataPoint.note ?? "");
      point.value.push(dataPoint.newPoint ?? 0);
    }

    labels.value.push("");
    customLabels.value.push("");
    point.value.push(0);
  labels.value.reverse();
  customLabels.value.reverse();
  point.value.reverse();
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
            label: function (context: any) {
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

const getdata = async () => {
//   try {
//     await axiosInstance
//       .post("ProgressUpdates/data-chart", props.searchRequest)
//       .then((res) => {
//         respone.value = res.data;
//       })
//       .then(() => {
//         build();
//       })
//       .then(() => {
//         load();
//       });
//   } catch (error) {
//     console.error("Error fetching data:", error);
//   }

await build();
await load();
};
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
