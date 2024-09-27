import { LoginResult } from "@/Models/LoginResult";
import Cookies from "js-cookie";

export function hasPermission(userRoles: string[], requiredRoles: string[]): boolean {
  for (const requiredRole of requiredRoles) {
    if (userRoles.includes(requiredRole)) {
      return true;
    }
  }
  return false;
}
export function getRolesFromToken(): string[] | null {
  try {
    // var token = Cookies.get('accessToken')?.toString() ?? "";
    const decodedToken = new LoginResult();
    var jsonString = Cookies.get("Roles")?.toString() ?? "";
    var jsonObject = JSON.parse(jsonString);
    var arrayFromString = Object.values(jsonObject);
    decodedToken.roles = arrayFromString as string[];
    console.log(decodedToken);
    return decodedToken.roles || [];
  } catch (error) {
    // console.error(error);
    return null;
  }
}
interface TokenPayload {
  [x: string]: never[];
}
export function IdentifyRoles(requiredRoles: string[]){
  return hasPermission(getRolesFromToken() ?? [], requiredRoles);
}