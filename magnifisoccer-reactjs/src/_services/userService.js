import config from 'config';
import { authHeader } from '../_helpers/auth-header';

export const userService = {
    login,
    logout,
    register,
    forgetPassword,
    getAllUsersForSquad,
    getById
};

function login(Email, Password) {
    const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ Email, Password })
    };

    return fetch(`${config.apiUrl}/api/auth/login`, requestOptions)
        .then(handleResponse)
        .then(user => {
            localStorage.setItem('user', JSON.stringify(user));

            return user;
        });
}

function logout() {
    localStorage.removeItem('user');
}

function getAllUsersForSquad() {
    const requestOptions = {
        method: 'GET',
        headers: authHeader()
    };
    
    return fetch(`${config.apiUrl}/api/auth/users`, requestOptions).then(handleResponse);
}

function getById(id) {
    const requestOptions = {
        method: 'GET',
        headers: authHeader()
    };

    return fetch(`${config.apiUrl}/api/auth/${id}`, requestOptions).then(handleResponse);
}

function register(user) {
    const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(user)
    };

    return fetch(`${config.apiUrl}/api/auth/register`, requestOptions).then(handleResponse);
}

function forgetPassword(email) {
    const requestOptions = {
        method: 'POST'        
    };

    return fetch(`${config.apiUrl}/api/auth/forgetPassword?email=${email}`,requestOptions).then(handleResponse);
}

function handleResponse(response) {
    return response.text().then(text => {
        const data = text && JSON.parse(text);
        if (!response.ok) {
            if (response.status === 401) {
                // auto logout if 401 response returned from api
                logout();
                location.reload(true);
            }

            const error = (data && data.message) || response.statusText;
            return Promise.reject(error);
        }

        return data;
    });
}