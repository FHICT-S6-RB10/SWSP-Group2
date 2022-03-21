import React, {useEffect, useState} from 'react';
import '../styles/serviceList.css';
import Service from "./Service";
import {useDispatch, useSelector} from "react-redux";
import {getServices} from "../store/actions/serviceActions";

const ServiceList = () => {
    const dispatch = useDispatch();
    const {services} = useSelector(state => state.services)
    const [styledServices, setStyledServices] = useState([]);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        setLoading(true);
        dispatch(getServices())
            .then(() => setLoading(false));
    }, [dispatch])

    useEffect(() => {
        setStyledServices(services.map(service => <Service key={service.name} service={service}/>));
    }, [services])

    return (
        <div className="service-list">
            <div className={"service-list-title"}>Service Status</div>
            {loading ? 'Loading...' : styledServices}
        </div>
    )
}

export default ServiceList
