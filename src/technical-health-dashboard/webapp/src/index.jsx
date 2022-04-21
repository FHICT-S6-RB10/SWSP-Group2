import React from 'react';
import ReactDOM from 'react-dom';
import './styles/index.css';
import App from './components/App';
import {envGet} from "./utils/envHelper";
import {SOCKET_HOST, SOCKET_PORT} from "./constants";

import {createStore, applyMiddleware} from 'redux';
import rootReducer from './store/reducers/rootReducer';
import {Provider} from 'react-redux';
import thunk from 'redux-thunk';
import {setServices} from "./store/actions/serviceActions";
import {setMessages} from "./store/actions/messageActions";

export const store = createStore(rootReducer, applyMiddleware(thunk));

const SOCKET_URL = `${envGet(SOCKET_HOST)}:${envGet(SOCKET_PORT)}`;

const client = new WebSocket(SOCKET_URL);

client.onopen = () => {
    console.log('Connected to THS');
}

client.onmessage = message => {
    const {services: receivedServices, messages: receivedMessages} = JSON.parse(message.data);

    if (receivedServices && receivedServices.length > 0) store.dispatch(setServices(receivedServices));
    if (receivedMessages && receivedMessages.length > 0) store.dispatch(setMessages(receivedMessages));
}

ReactDOM.render(
    <Provider store={store}>
      <React.StrictMode>
        <App />
      </React.StrictMode>
    </Provider>,
  document.getElementById('root')
)
