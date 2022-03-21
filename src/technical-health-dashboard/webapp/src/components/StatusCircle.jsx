import React, {useEffect, useState} from 'react';
import '../styles/statusCircle.css';
import {GREEN, RED, WHITE, YELLOW} from "../constants";

const StatusCircle = (props) => {
    const {ONLINE_STATUS, OFFLINE_STATUS, HAS_ERRORS_STATUS} = window._env_;

    const [circleColor, setCircleColor] = useState('');

    useEffect(() => {
        switch (props.status) {
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
