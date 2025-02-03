export const deleteCookie = (name: string) => {
    document.cookie = name + "=; Expires=Thu, 01 Jan 1970 00:00:01 GMT; path=/"
};

export const setCookie = (name: string, value: string, expirationDate: Date, path = '/') => {
    let expires = '';
    if (expirationDate) {
        expires = `; Expires=${expirationDate}`;
    }
    document.cookie = `${name}=${encodeURIComponent(value)}${expires}; Path=${path}`;
}

export const  getCookie = (name: string) => {
    const nameEQ = `${name}=`;
    const cookies = document.cookie.split(';');
    for (let cookie of cookies) {
        cookie = cookie.trim();
        if (cookie.startsWith(nameEQ)) {
            return decodeURIComponent(cookie.substring(nameEQ.length));
        }
    }
    return null;
}