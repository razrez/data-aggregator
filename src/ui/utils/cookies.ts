export const deleteCookie = (name: string) => {
    document.cookie = name + "=; Max-Age=-99999999; path=/"
};

export const setCookie = (name: string, value: string, expirationDate: Date, path = '/') => {
    let expires = '';
    if (expirationDate) {
        expires = `; expires=${expirationDate}`;
    }
    document.cookie = `${name}=${encodeURIComponent(value)}${expires}; path=${path}`;
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