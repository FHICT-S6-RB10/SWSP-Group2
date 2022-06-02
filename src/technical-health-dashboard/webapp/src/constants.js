import {envGet} from "./utils/envHelper";

export const SOCKET_HOST = "SOCKET_HOST";
export const SOCKET_PORT = "SOCKET_PORT";

export const SET_SERVICES = 'GET_SERVICES';
export const SET_SELECTED_SERVICES = 'SET_SELECTED_SERVICES';

export const SET_MESSAGES = 'GET_MESSAGES';

export const SET_TENANTS = 'SET_TENANTS';
export const SET_SELECTED_TENANT = 'SET_SELECTED_TENANT';

const OFFLINE = envGet("OFFLINE_STATUS");
const ONLINE = envGet("ONLINE_STATUS");
const HAS_ERRORS = envGet("HAS_ERRORS_STATUS");

export const LOG = envGet("LOG_LEVEL");
export const WARNING = envGet("WARNING_LEVEL");
export const ERROR = envGet("ERROR_LEVEL");

export const TAB_ICON = 'tab-icon';
export const MESSAGE_ICON = 'message-icon';

const LOG_ICON = 'fa-solid fa-circle-exclamation';
const WARNING_ICON = 'fa-solid fa-triangle-exclamation';
const ERROR_ICON = 'fa-solid fa-circle-info'

const GREEN = 'limegreen';
const RED = 'red';
const YELLOW = 'yellow';
const BLUE = 'rgb(48, 148, 255)';

export const serviceColors = {
    [ONLINE]: GREEN,
    [HAS_ERRORS]: YELLOW,
    [OFFLINE]: RED
}

export const serviceStatuses = {
    [ONLINE]: 'Online',
    [HAS_ERRORS]: 'Has Errors',
    [OFFLINE]: 'Offline',
}

export const messageTitles = {
    [LOG]: 'Log',
    [WARNING]: 'Warning',
    [ERROR]: 'Error'
}

export const messageColors = {
    [LOG]: BLUE,
    [WARNING]: YELLOW,
    [ERROR]: RED
}

export const messageIcons = {
    [LOG]: LOG_ICON,
    [WARNING]: WARNING_ICON,
    [ERROR]: ERROR_ICON
}
