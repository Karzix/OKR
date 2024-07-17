import { axiosInstance } from "../axiosConfig";

import Cookies from 'js-cookie';
import { reactive } from 'vue';
import type { LoginResult } from '@/Models/LoginResult';
import type { UserModel } from "@/Models/UserModel";
import type { AppResponse } from "@/components/maynghien/BaseModels/AppResponse";
const loginUrl = "OKR-gateway/login?useCookies=false&useSessionCookies=false";

export const handleLogin = async (model: UserModel): Promise<AppResponse<LoginResult>> => {

    let resust: AppResponse<LoginResult>=({
        isSuccess: true,
        message: '',
        data: undefined
    });
    var u = {
        email: "karzix1809@gmail.com",
        password: "CdzuOsSbBH",
        twoFactorCode: "string",
        twoFactorRecoveryCode: "string"
      };
        const postResult = await axiosInstance.post(loginUrl, u);
        console.log(postResult.data);
        const responseObject = postResult.data
        resust = responseObject;

                Cookies.set('accessToken', responseObject.accessToken ?? "", { expires: undefined });
                // Cookies.set('Roles', JSON.stringify(resust.data.roles) ?? "", { expires: undefined });
                Cookies.set('refreshToken', responseObject.refreshToken ?? "", { expires: undefined });
                axiosInstance.defaults.headers.common['Authorization'] = `Bearer ${responseObject.accessToken}`;

    return resust;

}