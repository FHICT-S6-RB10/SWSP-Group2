import React from 'react';
import ReactDOM from 'react-dom';
import './styles/index.css';
import App from './components/App';
import {envGet} from "./utils/envHelper";
import {SOCKET_HOST, SOCKET_PORT} from "./constants";

import { PublicClientApplication, InteractionType } from "@azure/msal-browser";
import { MsalProvider, MsalAuthenticationTemplate } from "@azure/msal-react";
import {appRoles, msalConfig} from "../azure-config";
import {Auth} from "./components/Auth";

import {createStore, applyMiddleware} from 'redux';
import rootReducer from './store/reducers/rootReducer';
import {Provider} from 'react-redux';
import thunk from 'redux-thunk';
import {setServices} from "./store/actions/serviceActions";
import {setMessages} from "./store/actions/messageActions";
import {setTenants} from "./store/actions/tenantActions";

const msalInstance = new PublicClientApplication(msalConfig);

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
    if (receivedMessages && receivedMessages.length > 0) store.dispatch(setTenants(receivedMessages));
}

ReactDOM.render(
    <Provider store={store}>
      <React.StrictMode>
          <MsalProvider instance={msalInstance}>
              <MsalAuthenticationTemplate interactionType={InteractionType.Redirect}>
                  <Auth exact roles={[appRoles.Admin]}>
                    <App />
                  </Auth>
              </MsalAuthenticationTemplate>
          </MsalProvider>
      </React.StrictMode>
    </Provider>,
  document.getElementById('root')
)
