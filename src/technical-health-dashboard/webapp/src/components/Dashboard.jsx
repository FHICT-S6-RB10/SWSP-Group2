import React, {useEffect, useState} from 'react';
import '../styles/dashboard.css';
import {demoServices} from "../demoServices";
import Service from "./Service";

const Dashboard = () => {
    const [services, setServices] = useState([]);
    const [styledServices, setStyledServices] = useState([]);

    useEffect(() => {
        setServices(demoServices)
    },[])

    useEffect(() => {
        setStyledServices(services.map(service => <Service key={service.id} service={service}/>));
    }, [services])

    return (
        <div className="dashboard">
            {styledServices}
        </div>
    )
}

export default Dashboard
