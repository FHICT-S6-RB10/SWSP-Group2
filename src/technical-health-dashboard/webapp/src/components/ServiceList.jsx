import React, {useEffect, useState} from 'react';
import '../styles/serviceList.css';
import Service from "./Service";
import {useDispatch, useSelector} from "react-redux";
import {createMockServices, getServices} from "../store/actions/serviceActions";

const ServiceList = () => {
    const dispatch = useDispatch();
    const {services, initial} = useSelector(state => state.services)
    const [styledServices, setStyledServices] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

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
        setStyledServices(services.map(service => <Service key={service.name} service={service}/>));
    }, [services])

    return (
        <div className="service-list">
            <div className={"service-list-title"}>Service Status</div>
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

export default ServiceList
