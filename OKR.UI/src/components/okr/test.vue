<template>
  <div>
    <canvas id="acquisitions"></canvas>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import Chart from "chart.js/auto";

const labels = ref<string[]>([""]);
const customLabels = ref<string[]>([""]);
const point = ref<number[]>([0]);

const respone = ref({
  isSuccess: true,
  message: "",
  data: [],
});

const config = ref();

const build = () => {
  const n = respone.value.data.length;
  labels.value = [];
  customLabels.value = [];
  point.value = [];

  for (let i = 0; i < n; i++) {
    const dataPoint = respone.value.data[i] as {date: Date, point: number, lable: string};
    labels.value.push(new Date(dataPoint.date).toLocaleDateString("en-GB"));
    customLabels.value.push(dataPoint.lable);
    point.value.push(dataPoint.point);
  }
};
const load = () => {
  config.value = {
    type: "line",
    data: {
      labels: labels.value,
      datasets: [
        {
          label: "My First Dataset",
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

              // // Custom labels for each point
              // const customLabels = customLabels;

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

onMounted(async () => {
  try {
    const response = await fetch(
      "https://localhost:7231/ProgressUpdates/data-chart",
      {
        method: "POST",
        body: null,
      }
    );
    const result = await response.json();
    respone.value.data = result.data;
    await build();
    load();
  } catch (error) {
    console.error("Error fetching data:", error);
  }
});
</script>
