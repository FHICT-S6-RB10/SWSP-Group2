import {SET_SELECTED_TENANT, SET_TENANTS} from "../../constants";
import {store} from "../../index";

export const setTenants = receivedMessages => {
    return dispatch => {
        const {tenants: storedTenants} = store.getState().tenants;
        const receivedTenants = [];

        receivedMessages.forEach(message => {
            if (storedTenants.indexOf(message.tenantId) < 0 && receivedTenants.indexOf(message.tenantId) < 0) {
                receivedTenants.push(message.tenantId);
            }
        });

        const updatedTenants = [...storedTenants, ...receivedTenants];

        dispatch({type: SET_TENANTS, data: updatedTenants});
    }
}

export const saveSelectedTenant = tenantId => {
    return dispatch => {
        dispatch({type: SET_SELECTED_TENANT, data: tenantId});
    }
}
