import React, {useEffect, useState} from 'react';
import '../../styles/services/serviceContainer.css';
import Service from "./Service";
import {useDispatch, useSelector} from "react-redux";
import {saveSelectedServices} from "../../store/actions/serviceActions";

const ServiceContainer = () => {
    const dispatch = useDispatch();
    const {services} = useSelector(state => state.services);

    const [styledServices, setStyledServices] = useState([]);
    const [selectedServices, setSelectedServices] = useState([]);

    const isSelected = serviceName => {
        return selectedServices.indexOf(serviceName) > -1;
    }

    const handleServiceClick = clickedServiceName => {
        let newSelection;

        if(isSelected(clickedServiceName)) {
            newSelection = selectedServices.filter(serviceName => serviceName !== clickedServiceName)
        } else {
            newSelection = [...selectedServices, clickedServiceName]
        }

        setSelectedServices(newSelection);
        dispatch(saveSelectedServices(newSelection));
    }

    useEffect(() => {
        setStyledServices(services.map(service => (
                <Service
                    key={service.name}
                    service={service}
                    isSelected={isSelected(service.name)}
                    handleClick={() => handleServiceClick(service.name)}/>
            )
        ));
    }, [services, selectedServices]);

    return (
        <div className="service-container">
            <div className={"service-container-title"}>Service Status</div>
            {services.length > 0 ? styledServices : 'No services'}
        </div>
    )
}

export default ServiceContainer
