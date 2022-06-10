import React from 'react';
import {messageColors, messageIcons} from "../../constants";

const MessageIcon = props => {
    const {level, usedIn} = props;

    return (
        <div className={usedIn} style={{color: messageColors[level]}}>
            <div>
                <i className={messageIcons[level]}/>
            </div>
        </div>
    );
}

export default MessageIcon;
