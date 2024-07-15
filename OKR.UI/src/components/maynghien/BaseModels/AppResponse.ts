export class AppResponse<T> {
    isSuccess: boolean;
    message: string;
    data: T | undefined;

    
    constructor() {
        this.isSuccess = false;
        this.message = "";
        this.data = undefined;
    }
}