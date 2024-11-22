import type { DepartmentProgressApprovalDto } from "./DepartmentProgressApprovalDto";
import  { StatusObjectives } from "./EntityObjectives";
import type { TargetType } from "./Enum/TargetType";
import type { KeyResult } from "./KeyResult";

export class Objectives {
    name: string | undefined;
    startDay?: Date;
    endDay?: Date;
    targetType: TargetType = 0;
    targetTypeName?: string;
    status: StatusObjectives = StatusObjectives.noStatus;
    departmentId?: string;
    applicationUserId?: string;
    isPublic: boolean = true;
    isUserObjectives: boolean = true;
    keyResults: KeyResult[] = [];
    period?: string;
    year?: number;
    id?: string;
    point?: number;
    lastProgressUpdate: Date | undefined;
    createdBy?: string | undefined;
    createdOn: Date | undefined;
    numberOfPendingUpdates: number = 0
}
export const recaculateObjectivesAfterProgressApproval = (objectives: Objectives, DepartmentProgressApproval : DepartmentProgressApprovalDto) => {
    const keyResult = objectives.keyResults.find(x => x.id == DepartmentProgressApproval.keyresultID);
    if (keyResult) {
      keyResult.currentPoint! += DepartmentProgressApproval.addedPoints;
      keyResult.isCompleted = DepartmentProgressApproval.isCompleted ?? false;
    }
    var newpoint = 0;
    objectives.lastProgressUpdate = new Date();
    objectives.keyResults.forEach((k) => {
      if(k.unit == 2 && k.isCompleted == true) {
        newpoint += k.percentage!
      }
      else{
      newpoint += (k.currentPoint! / k.maximunPoint!)* k.percentage!;
      }
    });
    objectives.point = Math.round(newpoint);

    return objectives.point;
}
