export class UserModel {
 
    userName: string | undefined;
    password: string| undefined;
    email: string|undefined;
    role: string|unknown;
    token: string | undefined;
    refreshToken: string | undefined;
    id: string | undefined;
    departmentName: string = "";
    departmentId: string | undefined;
    managerName: string = "";
    newPassword: string = "";
    isLocked: boolean = false;
}