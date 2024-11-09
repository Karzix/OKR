export class StatusStatistics {
    noStatus: number;
    onTrack: number;
    atRisk: number;
    offTrack: number;
    closed: number;
    total: number;

    constructor(
        noStatus: number = 0,
        onTrack: number = 0,
        atRisk: number = 0,
        offTrack: number = 0,
        closed: number = 0,
        total: number = 0
    ) {
        this.noStatus = noStatus;
        this.onTrack = onTrack;
        this.atRisk = atRisk;
        this.offTrack = offTrack;
        this.closed = closed;
        this.total = total;
    }
}