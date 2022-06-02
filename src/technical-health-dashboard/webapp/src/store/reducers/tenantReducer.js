import {SET_TENANTS, SET_SELECTED_TENANT} from '../../constants';

const initState = {
    tenants: [],
    selectedTenant: null,
}

const tenantReducer = (state = initState, action) => {
    switch (action.type) {
        case SET_TENANTS:
            return {
                ...state,
                tenants: action.data,
            }
        case SET_SELECTED_TENANT:
            return {
                ...state,
                selectedTenant: action.data
            }
        default:
            return state
    }
}

export default tenantReducer;
