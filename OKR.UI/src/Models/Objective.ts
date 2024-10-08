import type { KeyResult } from "./KeyResult";

export class Objective {
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

    // constructor() {}
}