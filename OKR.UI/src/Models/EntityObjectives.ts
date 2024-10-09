import type { KeyResult } from "./KeyResult";

export class EntityObjectives {
    id: string | undefined;
    name: string | undefined;
    startDay: Date | undefined;
    deadline: Date | undefined;
    targetType: number | undefined;
    targetTypeName: string | undefined;
    listKeyResults: KeyResult[]  = []; 
    point: number | undefined;
    createBy?: string | undefined;
    createOn?: Date | undefined;
    objectivesId?: string | undefined;
    status: StatusObjectives | undefined;
    // constructor() {}
}
export enum StatusObjectives {
    working,
    end,
}