import {SET_MESSAGES} from '../../constants';

const initState = {
    initial: true,
    messages: []
}

const messageReducer = (state = initState, action) => {
    switch (action.type) {
        case SET_MESSAGES:
            return {
                ...state,
                initial: false,
                messages: action.data,
            }
        default:
            return state
    }
}

export default messageReducer;
