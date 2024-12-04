export function formatDate(date: any): string {
    let dS = "";
    
    if (typeof date === "string") {
        dS = new Date(date).toLocaleDateString("en-GB");
    } else if (date instanceof Date) {
        dS = date.toLocaleDateString("en-GB");
    } 

    return dS;
}
export function formatDate_dd_mm_yyyy_hh_mm(date : any) {

    var typeDate = new Date();
    if (typeof date === "string") {
        typeDate = new Date(date);
    } else if (date instanceof Date) {
        typeDate = date;
    } 

    const day = String(typeDate.getDate()).padStart(2, '0');
    const month = String(typeDate.getMonth() + 1).padStart(2, '0');
    const year = typeDate.getFullYear();
    const hours = String(typeDate.getHours()).padStart(2, '0');
    const minutes = String(typeDate.getMinutes()).padStart(2, '0');
  
    // Tạo chuỗi định dạng
    return `${day}/${month}/${year} - ${hours}:${minutes}`;
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