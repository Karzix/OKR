<template>
  <el-tree
    style="max-width: 600px"
    :props="props"
    :data="treeData"
    @node-click="handleNodeClick"
    :highlight-current="true"
  />
  <p>Selected Node Id: {{ value }}</p>
</template>

<script lang="ts" setup>
import { ref } from 'vue'

interface Tree {
  id: number
  name: string
  leaf?: boolean
  zones?: Tree[]
}

const props = {
  label: 'name',
  children: 'zones',
  isLeaf: 'leaf',
}

const treeData: Tree[] = [
  {
    id: 1,
    name: 'Zone 1',
    zones: [
      {
        id: 11,
        name: 'Sub Zone 1-1',
      },
      {
        id: 12,
        name: 'Sub Zone 1-2',
      },
    ],
  },
  {
    id: 2,
    name: 'Zone 2',
    zones: [
      {
        id: 21,
        name: 'Sub Zone 2-1',
      },
      {
        id: 22,
        name: 'Sub Zone 2-2',
      },
    ],
  },
]

const value = ref<number | null>(null)

const handleNodeClick = (data: Tree) => {
  value.value = data.id
}
</script>
