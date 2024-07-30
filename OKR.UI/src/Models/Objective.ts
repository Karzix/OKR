import type { KeyResult } from "./KeyResult";

export class Objective {
    id: string | undefined;
    name: string | undefined;
    startDay: Date | undefined;
    deadline: Date | undefined;
    targetTypeId: string | undefined;
    targetTypeName: string | undefined;
    listKeyResults: KeyResult[]  = []; 
    point: number | undefined;   
}