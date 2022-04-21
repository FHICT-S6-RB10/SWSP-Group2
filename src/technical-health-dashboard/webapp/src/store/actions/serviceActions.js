import {SET_SERVICES, SET_SELECTED_SERVICES} from "../../constants";
import {store} from "../../index";
import {isEqual} from "../../utils/equalityChecker";
import {sortServicesByName} from "../../utils/services";

export const setServices = receivedServices => {
    return dispatch => {
        const {initial, services: storedServices} = store.getState().services;

        sortServicesByName(receivedServices);

        if (initial) return dispatch({type: SET_SERVICES, data: receivedServices});

        const newServices = [...storedServices];
        receivedServices.forEach(receivedService => {
            const existingService = newServices.find(storedService => storedService.name === receivedService.name);

            if (existingService) return existingService.status = receivedService.status
            newServices.push(receivedService);
        });

        sortServicesByName(newServices);

        if (!isEqual(receivedServices, storedServices)) return dispatch({type: SET_SERVICES, data: newServices});
    }
}

export const saveSelectedServices = serviceNames => {
    return dispatch => {
        dispatch({type: SET_SELECTED_SERVICES, data: serviceNames});
    }
}
