import { axiosInstance } from "../axiosConfig";

import Cookies from 'js-cookie';
import { reactive } from 'vue';
import type { LoginResult } from '@/Models/LoginResult';
import type { UserModel } from "@/Models/UserModel";
import type { AppResponse } from "@/components/maynghien/BaseModels/AppResponse";
const loginUrl = "OKR-gateway/account/login";

export const handleLogin = async (model: UserModel): Promise<AppResponse<LoginResult>> => {

    let resust: AppResponse<LoginResult>=({
        isSuccess: false,
        message: '',
        data: undefined
    });

    try {
        const postResult = await axiosInstance.post(loginUrl, model);
        console.log(postResult.data);
        const responseObject = postResult.data
        resust = responseObject;
        if (resust.isSuccess) {
            if(resust.data!=undefined){
                Cookies.set('accessToken', resust.data.token ?? "", { expires: undefined });
                Cookies.set('Roles', JSON.stringify(resust.data.roles) ?? "", { expires: undefined });
                Cookies.set('refreshToken', resust.data.refreshToken ?? "", { expires: undefined });
                axiosInstance.defaults.headers.common['Authorization'] = `Bearer ${resust.data.token}`;
            }
            
        }
        else {
            console.log(resust.message);

        }
    } catch (error) {
        console.error(error);

    }
    return resust;

}