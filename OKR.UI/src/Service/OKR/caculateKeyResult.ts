import type { KeyResult } from "@/Models/KeyResult";

export const caculateKeyResult = (keyResult: KeyResult) => {
    var point = 0;
    if(keyResult.sidequests){
        var countSidequestDone = 0
        keyResult.sidequests.forEach(x => {
            if(x.status){
                countSidequestDone++;
            }
        })
        point = countSidequestDone / keyResult.sidequests.length;
    }
    if(keyResult.unit != "01" && keyResult.currentPoint != undefined && keyResult.maximumPoint != undefined){
        var pointKeyResult = keyResult.currentPoint / keyResult.maximumPoint;
        point = point/2 + pointKeyResult/2;
    }
    return point*100;
}