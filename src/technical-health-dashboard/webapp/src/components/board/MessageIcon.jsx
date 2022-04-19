import React from 'react';
import {messageColors, messageIcons} from "../../constants";

const MessageIcon = props => {
    const {level} = props;

    return (
        <span className={`tab-button-icon`} style={{color: messageColors[level]}}>
            <i className={messageIcons[level]}/>
        </span>
    );
}

export default MessageIcon;
