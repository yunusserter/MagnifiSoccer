export function authHeader() {
    // return authorization header with jwt token
    let user = JSON.parse(localStorage.getItem('user'));

    if (user && user.message) {        
        return { 'Authorization': 'Bearer ' + user.message };
    } else {
        return {};
    }
}