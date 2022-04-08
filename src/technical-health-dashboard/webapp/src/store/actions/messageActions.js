import axios from 'axios';
import {GET_MESSAGES} from "../../constants";
import {envGet} from "../../envHelper";

const SERVER_HOST = envGet('SERVER_HOST');
const SERVER_PORT = envGet('SERVER_PORT');

export const getMessages = () => {
    return async dispatch => {
        const response = await axios.get(`${SERVER_HOST}:${SERVER_PORT}/messages`);

        dispatch({type: GET_MESSAGES, data: response.data});
    }
}
