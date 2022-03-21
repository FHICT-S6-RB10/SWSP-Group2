import axios from 'axios';
import {GET_SERVICES} from "../../constants";

const {SERVER_URL, SERVER_PORT} = window._env_;

export const getServices = () => {
    return async dispatch => {
        const response = await axios.get(`${SERVER_URL}:${SERVER_PORT}/servicestates`)

        dispatch({type: GET_SERVICES, data: response.data});
    }
}
