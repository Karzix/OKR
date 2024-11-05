import type { KeyResult } from "./KeyResult";

export class EntityObjectives {
    id: string | undefined;
    name: string | undefined;
    // startDay: Date | undefined;
    // deadline: Date | undefined;
    quarter: string | undefined;
    quarterText: string | undefined;
    targetType: number | undefined;
    targetTypeName: string | undefined;
    keyResults: KeyResult[]  = []; 
    point: number | undefined;
    createBy?: string | undefined;
    createOn?: Date | undefined;
    objectivesId?: string | undefined;
    status: StatusObjectives | undefined;
    numberOfPendingUpdates: number = 0;
    year: number | undefined;
    endDay: Date | undefined;
    startDay: Date | undefined;
    stringOfDays: string | undefined;
    // constructor() {}
}
export enum StatusObjectives {
    noStatus,
    onTrack,
    atRisk,
    offTrack,
    closed
}
export function getStatusText(status: StatusObjectives): string {
    switch (status) {
        case StatusObjectives.noStatus:
            return "No Status";
        case StatusObjectives.onTrack:
            return "On Track";
        case StatusObjectives.atRisk:
            return "At Risk";
        case StatusObjectives.offTrack:
            return "Off Track";
        case StatusObjectives.closed:
            return "Closed";
        default:
            return "Unknown Status";
    }
}
export function getTagType(status: StatusObjectives): string | undefined {
    switch (status) {
        case StatusObjectives.noStatus:
            return ''; // Không có kiểu cho "No Status"
        case StatusObjectives.onTrack:
            return 'success';
        case StatusObjectives.atRisk:
            return 'warning';
        case StatusObjectives.offTrack:
            return 'danger';
        case StatusObjectives.closed:
            return 'info';
        default:
            return undefined;
    }
}