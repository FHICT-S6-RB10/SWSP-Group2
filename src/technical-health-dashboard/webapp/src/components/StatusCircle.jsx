import React from 'react';
import '../styles/statusCircle.css';

const StatusCircle = (props) => {
    return (
        <div className={`status-circle color-${props.status}`} />
    )
}

export default StatusCircle
