import axios from 'axios';
import {GET_SERVICES} from "../../constants";

const {VITE_SERVER_URL, VITE_SERVER_PORT} = import.meta.env;

export const getServices = () => {
    return async dispatch => {
        const response = await axios.get(`${VITE_SERVER_URL}:${VITE_SERVER_PORT}/servicestates`)

        dispatch({type: GET_SERVICES, data: response.data});
    }
}
