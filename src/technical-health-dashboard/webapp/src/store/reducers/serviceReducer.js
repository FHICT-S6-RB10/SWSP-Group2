import {GET_SERVICES} from '../../constants';

const initState = {
    services: [],
}

const serviceReducer = (state = initState, action) => {
    switch (action.type) {
        case GET_SERVICES:
            return {
                ...state,
                services: action.data,
            }
        default:
            return state
    }
}

export default serviceReducer;
