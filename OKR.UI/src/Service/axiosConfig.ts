import axios from "axios";
const baseAPIUrl = "https://localhost:7231/";
export const axiosInstance = axios.create({
    baseURL: baseAPIUrl,
   // timeout: 10000,
    headers: {'Access-Control-Allow-Origin': 'http://localhost:5173/'} 
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