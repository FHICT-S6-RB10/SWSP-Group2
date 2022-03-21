import React, {useEffect, useState} from 'react';
import '../styles/statusCircle.css';
import {GREEN, RED, WHITE, YELLOW} from "../constants";

const StatusCircle = (props) => {
    const {VITE_ONLINE_STATUS, VITE_OFFLINE_STATUS, VITE_HAS_ERRORS_STATUS} = import.meta.env;

    const [circleColor, setCircleColor] = useState('');

    useEffect(() => {
        switch (props.status) {
            case VITE_ONLINE_STATUS:
                setCircleColor(GREEN);
                break
            case VITE_OFFLINE_STATUS:
                setCircleColor(RED);
                break
            case VITE_HAS_ERRORS_STATUS:
                setCircleColor(YELLOW);
                break
            default:
                setCircleColor(WHITE)
        }
    }, [props.status])

    return (
        <div className={`status-circle status-circle-${circleColor}`} />
    )
}

export default StatusCircle
