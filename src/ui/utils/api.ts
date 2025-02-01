import axios from 'axios';
import {deleteCookie, getCookie} from "~/utils/cookies";

axios.interceptors.response.use(
    response => {
        if (response.status >= 200 && response.status < 300) {
            return response;
        }
        return response;
    },
    error => {
        if (error.response) {
            const { status } = error.response;
            if (status === 401) {
                deleteCookie('authToken');
            }
        }

        // Возвращаем Promise с ошибкой для дальнейшей обработки
        return Promise.reject(error);
    }
);

const createApiClient = () => {
    const token = getCookie('authToken');
    if (token !== null) {
        return  axios.create({
            baseURL: 'http://localhost:5163/api',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });
    } else {
        return  axios.create({
            baseURL: 'http://localhost:5163/api',
            headers: {
                'Content-Type': 'application/json'
            }
        });
    }
}

const execute = (method: string, url: string, data = null) => {
    const config = {
        method,
        url,
        data,
    };

    const apiClient = createApiClient();

    return apiClient(config).then(response => {
        if (response.status >= 200 && response.status < 300) {
            return response.data;
        }
        throw new Error('Unexpected status code');
    }).catch(error => {
        if (error.response && error.response.status === 401) {
            deleteCookie('your_cookie_name');
            console.error('Unauthorized: Session expired or invalid token');
        }
        throw error;
    });
};

export const httpGet = (url: string, data: any) => execute("GET", url, data);
export const httpPost = (url: string, data: any) => execute("POST", url, data);
export const httpPut = (url: string, data: any) => execute("PUT", url, data);
export const httpDelete = (url: string, data: any) => execute("DELETE", url, data);