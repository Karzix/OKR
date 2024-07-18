import { axiosInstance } from "../axiosConfig";

import Cookies from "js-cookie";
import { reactive } from "vue";
import type { LoginResult } from "@/Models/LoginResult";
import type { UserModel } from "@/Models/UserModel";
import type { AppResponse } from "@/components/maynghien/BaseModels/AppResponse";
import { LoginModel } from "@/Models/LoginModel";

const loginUrl = "OKR-gateway/login?useCookies=false&useSessionCookies=false";

export const handleLogin = async (model: LoginModel): Promise<boolean> => {
  const LoginResult = await axiosInstance.post(loginUrl, model);
  const responseObject = LoginResult.data;
  if (LoginResult.status == 200) {
    Cookies.set("accessToken", responseObject.accessToken ?? "", { expires: undefined,});
    Cookies.set("refreshToken", responseObject.refreshToken ?? "", { expires: undefined,});
    axiosInstance.defaults.headers.common[
      "Authorization"
    ] = `${responseObject.accessToken}`;

    var infor = await axiosInstance.get("OKR-gateway/account/get-infor-account");
    var inforData = infor.data;
    if (inforData.isSuccess) {
      Cookies.set("userName", inforData.data.userName ?? "", { expires: undefined });
      Cookies.set("Roles", JSON.stringify(inforData.data.roles) ?? "", {expires: undefined,});
      return true;
    }
  }

  return false;
};
