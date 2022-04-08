import { combineReducers } from 'redux';
import serviceReducer from './serviceReducer';
import messageReducer from "./messageReducer";

const rootReducer = combineReducers({
    services: serviceReducer,
    messages: messageReducer
})

export default rootReducer;
