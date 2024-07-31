export function formatDate(date: any): string {
    let dS = "";
    
    if (typeof date === "string") {
        dS = new Date(date).toLocaleDateString("en-GB");
    } else if (date instanceof Date) {
        dS = date.toLocaleDateString("en-GB");
    } else {
        console.error("Invalid date format");
    }

    return dS;
}
export const getUtcOffsetInHours = (): number => {
    const date = new Date();
    const offsetInMinutes = date.getTimezoneOffset();
    const offsetInHours = -offsetInMinutes / 60;
    return offsetInHours;
};
export const RecalculateTheDate = (date: any): Date => {
    let dS: Date;

    if (typeof date === "string") {
        dS = new Date(date);
    } else if (date instanceof Date) {
        dS = new Date(date); 
    } else {
        throw new Error("Invalid date format");
    }

    dS.setHours(dS.getHours() + getUtcOffsetInHours());
    return dS;
};