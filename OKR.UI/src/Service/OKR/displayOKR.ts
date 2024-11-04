import type { EntityObjectives } from "@/Models/EntityObjectives";
import { RecalculateTheDate } from "../formatDate";

export const DisplayOKR = (okr: EntityObjectives) =>{
    okr.quarterText = convertValueToLabel(okr);
    okr.listKeyResults.forEach((keyResult) => {
        keyResult.deadline = RecalculateTheDate(keyResult.deadline);
    });
}
const convertValueToLabel = (okr:EntityObjectives) : string => {
    const value = okr.quarter + ":" + okr.year;
    if (!value) return "";
    const [quarter, year] = value.split(":");
    return `Quarter ${quarter} - ${year}`;
};