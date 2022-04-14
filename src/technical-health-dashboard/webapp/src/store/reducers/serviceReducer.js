import {SET_SERVICES, SET_SELECTED_SERVICES} from '../../constants';

const initState = {
    services: [],
    selectedServices: [],
}

const serviceReducer = (state = initState, action) => {
    switch (action.type) {
        case SET_SERVICES:
            return {
                services: action.data,
                selectedServices: action.data.map(service => service.name)
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
