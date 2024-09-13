export class EvaluateTarget {
    departmentObjectivesId : string 
    userObjectivesId : string
    content : string 
    createOn : Date 
    createBy : string 
    id : string
    constructor(departmentObjectivesId: string, userObjectivesId: string, content: string, createOn: Date, createBy: string, id: string) {
        this.id = id
        this.departmentObjectivesId = departmentObjectivesId
        this.userObjectivesId = userObjectivesId
        this.content = content
        this.createOn = createOn
        this.createBy = createBy
    }
}