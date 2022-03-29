import axios from 'axios';
import {GET_SERVICES, SET_SELECTED_SERVICES} from "../../constants";
import {envGet} from "../../envHelper";

const SERVER_HOST = envGet('SERVER_HOST');
const SERVER_PORT = envGet('SERVER_PORT');

export const createMockServices = () => {
    axios.get(`${SERVER_HOST}:${SERVER_PORT}/servicestatesmock`)
        .then(() => {
            getServices();
        })
}

export const getServices = () => {
    return async dispatch => {
        const response = await axios.get(`${SERVER_HOST}:${SERVER_PORT}/servicestates`);

        dispatch({type: GET_SERVICES, data: response.data});
    }
}

export const saveSelectedServices = serviceName => {
    return dispatch => {
        dispatch({type: SET_SELECTED_SERVICES, data: serviceName});
    }
}
