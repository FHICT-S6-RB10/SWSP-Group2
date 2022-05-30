import {envGet} from "./src/utils/envHelper";

export const msalConfig = {
    auth: {
        clientId: envGet('MSAL_CLIENT_ID'),
        authority: envGet('MSAL_AUTHORITY'),
        redirectUri: envGet('MSAL_REDIRECT_URI')
    },
    cache: {
        cacheLocation: "sessionStorage", // This configures where your cache will be stored
        storeAuthStateInCookie: false // Set this to "true" if you are having issues on IE11 or Edge
    }
};

// Add scopes here for ID token to be used at Microsoft identity platform endpoints.
export const loginRequest = {
    scopes: ["User.Read"]
};

export const appRoles = {
    Admin: "Organization.Caregiver"
}
