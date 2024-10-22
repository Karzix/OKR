export class DepartmentProgressApprovalDto {
    keyresultID: string | undefined;
    note: string = "";
    addedPoints: number = 0;
    createdOn?: Date;
    createdBy?: string;
    isApproved: boolean = false;
}
