import { combineReducers } from 'redux';
import serviceReducer from './serviceReducer';

const rootReducer = combineReducers({
    services: serviceReducer,
})

export default rootReducer;
