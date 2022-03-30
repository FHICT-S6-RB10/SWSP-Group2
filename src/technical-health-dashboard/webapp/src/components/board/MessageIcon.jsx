import React from 'react';
import {ERROR, LOG, WARNING} from "../../constants";
import '../../styles/board/icons.css';

const MessageIcon = (props) => {
    const {type} = props;

    switch (type) {
        case ERROR:
            return (
                <span className={`tab-button-icon ${type}`}>
                    <i className="fa-solid fa-triangle-exclamation"/>
                </span>
            );

        case WARNING:
            return (
                <span className={`tab-button-icon ${type}`}>
                    <i className="fa-solid fa-circle-exclamation"/>
                </span>
            );

        case LOG:
            return (
                <span className={`tab-button-icon ${type}`}>
                    <i className="fa-solid fa-circle-info"/>
                </span>
            );

        default:
            return (
                <span className={`tab-button-icon ${type}`}>
                    <i className="fa-solid fa-circle-info"/>
                </span>
            );
    }
}

export default MessageIcon;
