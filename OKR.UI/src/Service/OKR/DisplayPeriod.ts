export function getDisplayString(timePeriod: string): string {
    const [period, yearStr] = timePeriod.split(":");
    const year = parseInt(yearStr, 10);
    let displayString: string;

    switch (period) {
        case "Q1":
            displayString = `${period} (January - March) ${year}`;
            break;
        case "Q2":
            displayString = `${period} (April - June) ${year}`;
            break;
        case "Q3":
            displayString = `${period} (July - September) ${year}`;
            break;
        case "Q4":
            displayString = `${period} (October - December) ${year}`;
            break;
        case "H1":
            displayString = `${period} (January - June) ${year}`;
            break;
        case "H2":
            displayString = `${period} (July - December) ${year}`;
            break;
        case "FY": // Full year
            displayString = `${period} (January - December) ${year}`;
            break;
        default:
            throw new Error("Invalid period format. Expected Q1, Q2, Q3, Q4, H1, H2, or FY.");
    }

    return displayString;
}