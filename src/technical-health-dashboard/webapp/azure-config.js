import {envGet} from "./src/utils/envHelper";

export const msalConfig = {
    auth: {
        clientId: envGet('MSAL_CLIENT_ID'),
        authority: envGet('MSAL_AUTHORITY'),
        redirectUri: envGet('MSAL_REDIRECT_URI')
    },
    cache: {
        cacheLocation: "sessionStorage",
        storeAuthStateInCookie: false
    }
};

export const loginRequest = {
    scopes: ["User.Read"]
};

export const appRoles = {
    Admin: "Organization.Admin"
}
