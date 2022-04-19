import React from 'react';
import '../../styles/services/statusCircle.css';
import {serviceColors} from "../../constants";

const StatusCircle = props => {
    const {status} = props;

    return (
        <div className='status-circle'
             style={{backgroundColor: serviceColors[status]}}
        />
    )
}

export default StatusCircle
