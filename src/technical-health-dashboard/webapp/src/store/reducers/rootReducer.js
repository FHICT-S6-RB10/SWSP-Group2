import { combineReducers } from 'redux';
import serviceReducer from './serviceReducer';
import messageReducer from "./messageReducer";
import tenantReducer from "./tenantReducer";

const rootReducer = combineReducers({
    services: serviceReducer,
    messages: messageReducer,
    tenants: tenantReducer
});

export default rootReducer;
