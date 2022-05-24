import React from 'react';
import '../../styles/services/statusCircle.css';
import {serviceColors, serviceStatuses} from "../../constants";
import ReactTooltip from 'react-tooltip';

const StatusCircle = props => {
    const {status} = props;

    return (
        <>
            <div className='status-circle' data-tip={`Service ${serviceStatuses[status]}`}
                 style={{backgroundColor: serviceColors[status]}}
            />
            <ReactTooltip/>
        </>
    )
}

export default StatusCircle
