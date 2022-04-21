import {SET_SERVICES, SET_SELECTED_SERVICES} from '../../constants';

const initState = {
    services: [],
    selectedServices: [],
}

const serviceReducer = (state = initState, action) => {
    switch (action.type) {
        case SET_SERVICES:
            return {
                ...state,
                services: action.data,
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
