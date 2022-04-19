import {SET_MESSAGES} from "../../constants";

export const setMessages = messages => {
    return dispatch => {
        dispatch({type: SET_MESSAGES, data: messages});
    }
}
