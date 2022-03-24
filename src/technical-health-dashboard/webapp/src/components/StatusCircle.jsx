import React, {useEffect, useState} from 'react';
import '../styles/statusCircle.css';
import {GREEN, RED, WHITE, YELLOW} from "../constants";
import {envGet} from "../envHelper";

const StatusCircle = (props) => {
    const ONLINE_STATUS = envGet('ONLINE_STATUS');
    const OFFLINE_STATUS = envGet('OFFLINE_STATUS');
    const HAS_ERRORS_STATUS = envGet('HAS_ERRORS_STATUS');

    const [circleColor, setCircleColor] = useState('');

    useEffect(() => {
        switch (props.status.toString()) {
            case ONLINE_STATUS:
                setCircleColor(GREEN);
                break
            case OFFLINE_STATUS:
                setCircleColor(RED);
                break
            case HAS_ERRORS_STATUS:
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
