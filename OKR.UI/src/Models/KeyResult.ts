import { StatusObjectives } from "./EntityObjectives";
import type { ProgressUpdates } from "./ProgressUpdates";
import type { Sidequest } from "./Sidequests";

export class KeyResult {
  id: string | undefined;
  description: string | undefined;
  active: boolean | undefined;
  // deadline: Date | undefined;
  unit: number | undefined;
  currentPoint: number | undefined;
  maximunPoint: number | undefined;
  note: string = "";
  addedPoints?: number = 0;
  status: StatusObjectives = StatusObjectives.noStatus;
  percentage?: number = 0 // percentage 
  objectivesId?: string | undefined;
  lastProgressUpdate: Date | undefined;
  createdBy?: string | undefined;
    createdOn: Date | undefined;
    startDay?: Date;
    endDay?: Date;

  progressUpdates: ProgressUpdates[] = [];
}
