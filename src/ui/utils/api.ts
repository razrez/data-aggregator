import axios from 'axios';
import {deleteCookie, getCookie} from "~/utils/cookies";

const createApiClient = () => {
    const token = getCookie('authToken');
    if (token !== null) {
        return  axios.create({
            baseURL: 'http://localhost:5163/api',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
        });
    } else {
        return  axios.create({
            baseURL: 'http://localhost:5163/api',
            headers: {
                'Content-Type': 'application/json'
            },
        });
    }
}

const execute = async (method: string, url: string, data = null) => {
    const config = {
        method,
        url,
        data,
    };

    const apiClient = createApiClient();

    try {
        const response = await apiClient(config);
        if (response.status >= 200 && response.status < 300) {
            return response.data;
        }
        if (response.status === 401) {
            console.log("401");
            deleteCookie('authToken');
            navigateTo("/signin");
        }
    }
    catch (error) {
        if (!error.response && error.response?.status !== 401) {
            console.log(error);
            deleteCookie('authToken');
            navigateTo("/signin");
        }
    }
};

export const httpGet = (url: string, data: any) => execute("GET", url, data);
export const httpPost = (url: string, data: any) => execute("POST", url, data);
export const httpPut = (url: string, data: any) => execute("PUT", url, data);
export const httpDelete = (url: string, data: any) => execute("DELETE", url, data);