import {SET_MESSAGES} from "../../constants";
import {store} from "../../index";

export const setMessages = receivedMessages => {
    return dispatch => {
        const {messages: storedMessages} = store.getState().messages;

        const updatedMessages = [...storedMessages, ...receivedMessages];

        dispatch({type: SET_MESSAGES, data: updatedMessages});
    }
}
