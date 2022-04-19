import {SET_MESSAGES} from '../../constants';

const initState = {
    messages: []
}

const messageReducer = (state = initState, action) => {
    switch (action.type) {
        case SET_MESSAGES:
            return {
                ...state,
                messages: action.data,
            }
        default:
            return state
    }
}

export default messageReducer;
