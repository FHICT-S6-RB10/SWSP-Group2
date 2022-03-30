import React, {useEffect, useState} from 'react';
import '../../styles/services/serviceContainer.css';
import Service from "./Service";
import {useDispatch, useSelector} from "react-redux";
import {createMockServices, getServices, saveSelectedServices} from "../../store/actions/serviceActions";

const ServiceContainer = () => {
    const dispatch = useDispatch();
    const {services, initial} = useSelector(state => state.services);

    const [styledServices, setStyledServices] = useState([]);
    const [selectedServices, setSelectedServices] = useState([]);

    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

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
        setLoading(true);
        dispatch(getServices())
            .then(() => setLoading(false))
            .catch((err) => {
                setLoading(false);
                setError(err);
            });
    }, [dispatch])

    useEffect(() => {
        if(services.length === 0 && loading === false && initial === false) {
            createMockServices();
        }
    }, [services, loading, initial])

    useEffect(() => {
        setStyledServices(services.map(service => (
                <Service
                    key={service.name}
                    service={service}
                    isSelected={isSelected(service.name)}
                    handleClick={() => handleServiceClick(service.name)}/>
            )
        ));
    }, [services, selectedServices])

    return (
        <div className="service-container">
            <div className={"service-container-title"}>Service Status</div>
            {error ?
                error.message
                :
                <>
                    {loading && 'Loading...'}
                    {services.length > 0 ? styledServices : 'No services'}
                </>
            }
        </div>
    )
}

export default ServiceContainer
