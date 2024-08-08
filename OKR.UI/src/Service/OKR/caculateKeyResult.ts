import type { KeyResult } from "@/Models/KeyResult";
import type { Objective } from "@/Models/Objective";

export const caculateKeyResult = (keyResult: KeyResult) => {
    var point = 0;
    if(keyResult.sidequests && keyResult.sidequests.length > 0){
        var countSidequestDone = 0
        keyResult.sidequests.forEach(x => {
            if(x.status){
                countSidequestDone++;
            }
        })
        point = countSidequestDone / keyResult.sidequests.length;
    }
    if(keyResult.unit != 2 && keyResult.currentPoint != undefined && keyResult.maximunPoint != undefined){
        var pointKeyResult = keyResult.currentPoint / keyResult.maximunPoint;
        if(keyResult.sidequests && keyResult.sidequests.length > 0){
            point = point/2 + pointKeyResult/2;
        }
        else{
            point = pointKeyResult
        }
    }
    return point*100;
}
export const caculateObjective = (objective: Objective) => {
    var point = 0;
    if(objective.listKeyResults && objective.listKeyResults.length > 0){
        objective.listKeyResults.forEach(x => {
            point = point + caculateKeyResult(x);
        })
    }
    return point / objective.listKeyResults.length;
}