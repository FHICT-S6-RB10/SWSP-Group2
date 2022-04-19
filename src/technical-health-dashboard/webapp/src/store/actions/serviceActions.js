import {SET_SERVICES, SET_SELECTED_SERVICES} from "../../constants";

export const setServices = services => {
    return dispatch => {
        dispatch({type: SET_SERVICES, data: services});
    }
}

export const saveSelectedServices = serviceNames => {
    return dispatch => {
        dispatch({type: SET_SELECTED_SERVICES, data: serviceNames});
    }
}
