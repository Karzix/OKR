import type { Objectives } from "@/Models/Objective";
interface Tree {
  label: string;
  children?: Tree[];
}
export const buildTree = (objective: Objectives): Tree[] => {
  var dataTreeTemp = [] as Tree[];
  for (let i = 0; i < objective.keyResults?.length; i++) {
    var newTree = {
      label: "",
      children: [] as Tree[],
    } as Tree;
    newTree.label = objective.keyResults[i].description ?? "";
    dataTreeTemp.push(newTree);
  }
  return dataTreeTemp;
};
