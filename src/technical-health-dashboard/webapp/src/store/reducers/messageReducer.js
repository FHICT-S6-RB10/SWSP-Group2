import {GET_MESSAGES} from '../../constants';

const initState = {
    messages: []
}

const messageReducer = (state = initState, action) => {
    switch (action.type) {
        case GET_MESSAGES:
            return {
                ...state,
                messages: action.data,
            }
        default:
            return state
    }
}

export default messageReducer;
