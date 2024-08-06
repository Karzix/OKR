import type { KeyResult } from "@/Models/KeyResult";

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