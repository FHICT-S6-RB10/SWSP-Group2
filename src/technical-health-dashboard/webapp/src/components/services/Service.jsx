import React from 'react';
import StatusCircle from "./StatusCircle";
import '../../styles/services/service.css';

const Service = props => {
    const {name, status} = props.service;
    const {handleClick, isSelected} = props;
    return (
        <div
            className={`service ${isSelected && "service-selected"}`}
            onClick={handleClick}
        >
            <span className={"service-name"}>
                {name}
            </span>
            <StatusCircle status={status}/>
        </div>
    )
}

export default Service
