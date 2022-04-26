import React from 'react';
import {MESSAGE_ICON, messageColors, messageIcons, TAB_ICON} from "../../constants";

const MessageIcon = props => {
    const {level, usedIn} = props;

    let className;
    if (usedIn === TAB_ICON) className = 'tab-icon';
    if (usedIn === MESSAGE_ICON) className = 'message-icon';

    return (
        <div className={className} style={{color: messageColors[level]}}>
            <i className={messageIcons[level]}/>
        </div>
    );
}

export default MessageIcon;
