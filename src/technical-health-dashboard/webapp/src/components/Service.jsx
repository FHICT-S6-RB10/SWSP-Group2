import React from 'react';
import StatusCircle from "./StatusCircle";
import '../styles/service.css';

const Service = (props) => {
    return (
        <div className="service">
            <span className={"service-name"}>{props.service.name}</span>
            <StatusCircle status={props.service.status}/>
        </div>
    )
}

export default Service
