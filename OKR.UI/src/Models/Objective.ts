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
}