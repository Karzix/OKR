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

    constructor(Objective: Objective) {
        this.id = Objective.id;
        this.name = Objective.name;
        this.startDay = Objective.startDay;
        this.deadline = Objective.deadline;
        this.targetTypeId = Objective.targetTypeId;
        this.targetTypeName = Objective.targetTypeName;
        this.listKeyResults = Objective.listKeyResults;
        this.point = Objective.point
    }

    // constructor() {}
}