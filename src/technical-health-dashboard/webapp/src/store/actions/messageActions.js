import {SET_MESSAGES} from "../../constants";
import {store} from "../../index";
import {isEqual} from "../../utils/equalityChecker";

export const setMessages = receivedMessages => {
    return dispatch => {
        const {initial, messages: storedMessages} = store.getState().messages;

        if (initial) return dispatch({type: SET_MESSAGES, data: receivedMessages});

        const updatedMessages = [...storedMessages, ...receivedMessages];

        if (!isEqual(receivedMessages, updatedMessages)) return dispatch({type: SET_MESSAGES, data: updatedMessages});
    }
}
