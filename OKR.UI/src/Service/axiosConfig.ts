import { UserModel } from "@/Models/UserModel";
import axios from "axios";
import Cookies from 'js-cookie';
export const baseAPIUrl =  "http://103.209.34.217:8080/";
export const urlUI = "http://103.209.34.217/"
export const axiosInstance = axios.create({
    baseURL: baseAPIUrl,
   // timeout: 10000,
    
  });

  
// Get the token from the cookies
const getCookie = (name: string): string | null => {
  const value = `; ${document.cookie}`;
  const parts = value.split(`; ${name}=`);
  if (parts.length === 2) return parts.pop()?.split(';').shift() || null;
  return null;
};
const token = getCookie('accessToken');

if (token) {
  axiosInstance.defaults.headers.common['Authorization'] = `Bearer ${token}`;
}
axiosInstance.interceptors.request.use(config => {
  const token = getCookie('accessToken');
  if (token) {
      config.headers['Authorization'] = `Bearer ${token}`;
  }
  return config;
});

axiosInstance.interceptors.response.use(response => response, async error => {
  try{
    const originalRequest = error.config;
    if (error.response.status === 401 && !originalRequest._retry) {
        originalRequest._retry = true;
        // const refreshToken = localStorage.getItem('refreshToken');
        var user = new UserModel();
        user.token = Cookies.get('accessToken')?.toString();
        user.refreshToken = Cookies.get('refreshToken')?.toString();
        const response = await axios.post(baseAPIUrl + "refresh",user.refreshToken);
  
        if (response.status === 200) {
            Cookies.set('accessToken',  response.data.data.token ?? "", { expires: undefined });
            Cookies.set('refreshToken', response.data.data.refreshToken ?? "", { expires: undefined });
            axios.defaults.headers.common['Authorization'] = 'Bearer ' + response.data.token;
            return axiosInstance(originalRequest);
        }
        else{
          alert("Login session has expired! Please log in again")
          var cookies = document.cookie.split(";");
          for (var i = 0; i < cookies.length; i++) {
            var cookie = cookies[i];
            var eqPos = cookie.indexOf("=");
            var name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
            document.cookie = name + "=;expires=Thu, 01 Jan 1970 00:00:00 UTC;path=/;";
          }
          window.location.reload();
        }
    }
    return Promise.reject(error);
  }
  catch(error){
    // alert("Login session has expired! Please log in again")
    // var cookies = document.cookie.split(";");
    // for (var i = 0; i < cookies.length; i++) {
    //   var cookie = cookies[i];
    //   var eqPos = cookie.indexOf("=");
    //   var name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
    //   document.cookie = name + "=;expires=Thu, 01 Jan 1970 00:00:00 UTC;path=/;";
    // }
    // window.location.reload();
  }
  
});