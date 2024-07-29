//1 : branch
//2 : team

export const getLevelName = (level: number) => {
    switch (level) {
        case 1:
            return "Branch";
        case 2:
            return "Team";
        case 3:
            return "level 3";
        case 4:
            return "level 4";
        case 5:
    }
}