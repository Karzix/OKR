import type { StatusObjectives } from "./EntityObjectives";
import type { Sidequest } from "./Sidequests";

export class KeyResult {
  id: string | undefined;
  description: string | undefined;
  active: boolean | undefined;
  deadline: Date | undefined;
  unit: number | undefined;
  currentPoint: number | undefined;
  maximunPoint: number | undefined;
  note: string = "";
  addedPoints?: number = 0;
  status: StatusObjectives | undefined;
  percentage?: number = 0 // percentage 
}
