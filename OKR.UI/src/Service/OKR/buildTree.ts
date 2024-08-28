import type { Objective } from "@/Models/Objective";
interface Tree {
  label: string;
  children?: Tree[];
}
export const buildTree = (objective: Objective): Tree[] => {
  var dataTreeTemp = [] as Tree[];
  for (let i = 0; i < objective.listKeyResults?.length; i++) {
    var newTree = {
      label: "",
      children: [] as Tree[],
    } as Tree;
    newTree.label = objective.listKeyResults[i].description ?? "";
    for (
      let j = 0;
      j < (objective.listKeyResults[i]?.sidequests?.length ?? 0);
      j++
    ) {
      newTree.children?.push({
        label: objective.listKeyResults[i].sidequests[j].name ?? "",
        children: [] as Tree[],
      });
    }
    dataTreeTemp.push(newTree);
  }
  return dataTreeTemp;
};
