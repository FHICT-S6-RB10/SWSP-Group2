import axios from 'axios';
import {GET_SERVICES} from "../../constants";
import {envGet} from "../../envHelper";

const SERVER_URL = envGet('SERVER_URL');
const SERVER_PORT = envGet('SERVER_PORT');

export const getServices = () => {
    return async dispatch => {
        const response = await axios.get(`${SERVER_URL}:${SERVER_PORT}/servicestates`)

        dispatch({type: GET_SERVICES, data: response.data});
    }
}
