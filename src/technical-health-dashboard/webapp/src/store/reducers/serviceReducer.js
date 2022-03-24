import {GET_SERVICES} from '../../constants';

const initState = {
    services: [],
    initial: true
}

const serviceReducer = (state = initState, action) => {
    switch (action.type) {
        case GET_SERVICES:
            return {
                ...state,
                services: action.data,
                initial: false
            }
        default:
            return state
    }
}

export default serviceReducer;
