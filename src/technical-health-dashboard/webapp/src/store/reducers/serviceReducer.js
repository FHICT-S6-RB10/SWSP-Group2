import {GET_SERVICES, SET_SELECTED_SERVICES} from '../../constants';

const initState = {
    services: [],
    selectedServices: [],
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
        case SET_SELECTED_SERVICES:
            return {
                ...state,
                selectedServices: action.data
            }
        default:
            return state
    }
}

export default serviceReducer;
