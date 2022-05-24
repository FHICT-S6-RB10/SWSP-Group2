import React from 'react';
import StatusCircle from "./StatusCircle";
import '../../styles/services/service.css';
import ReactTooltip from "react-tooltip";

const Service = props => {
    const {name, status} = props.service;
    const {handleClick, isSelected} = props;
    return (
        <div
            className={`service ${isSelected && "service-selected"}`}
            onClick={handleClick} data-tip={`${isSelected ? 'Unselect' : 'Select'} ${name}`}
        >
            <span className={"service-name"}>
                {name}
            </span>
            <StatusCircle status={status}/>
            <ReactTooltip/>
        </div>
    )
}

export default Service
