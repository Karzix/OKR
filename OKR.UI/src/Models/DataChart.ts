export class DataChart {
    oldPoint?: number; 
    newPoint?: number;
    label: string;
    date?: Date; 
    userName: string;
    objectivesCompletionRate?: number;
    keyresultCompletionRate?: number;

    constructor(
        oldPoint?: number,
        newPoint?: number,
        label: string = '',
        date?: Date,
        userName: string = ''
    ) {
        this.oldPoint = oldPoint;
        this.newPoint = newPoint;
        this.label = label;
        this.date = date;
        this.userName = userName;
    }
}