import { axiosInstance } from "../axiosConfig";

import Cookies from "js-cookie";
import { reactive } from "vue";
import type { LoginResult } from "@/Models/LoginResult";
import type { UserModel } from "@/Models/UserModel";
import type { AppResponse } from "@/components/maynghien/BaseModels/AppResponse";
import { LoginModel } from "@/Models/LoginModel";

const loginUrl = "login";

export const handleLogin = async (model: LoginModel): Promise<boolean> => {
  try{
    const LoginResult = await axiosInstance.post(loginUrl, model);
    const responseObject = LoginResult.data;
    if (LoginResult.status == 200) {
      Cookies.set("accessToken", responseObject.accessToken ?? "", { expires: undefined,});
      Cookies.set("refreshToken", responseObject.refreshToken ?? "", { expires: undefined,});
      axiosInstance.defaults.headers.common[
        "Authorization"
      ] = `Bearer ${responseObject.accessToken}`;
  
      var infor = await axiosInstance.get("account/account-infor");
      var inforData = infor.data;
      if (inforData.isSuccess) {
        Cookies.set("userName", inforData.data.userName ?? "", { expires: undefined });
        Cookies.set("Roles", JSON.stringify(inforData.data.roles) ?? "", {expires: undefined,});
        Cookies.set("UserId", inforData.data.id ?? "", { expires: undefined });
        Cookies.set("DepartmentId", inforData.data.departmentId ?? "", { expires: undefined });
        return true;
      }
    }
  }
  catch (error) {
    console.error(error);
    axiosInstance.defaults.headers.common["Authorization"] = ``;
    return false;
  }
  
  return false;
};
