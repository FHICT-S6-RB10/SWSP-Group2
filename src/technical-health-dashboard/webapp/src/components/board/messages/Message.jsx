import React from 'react';
import '../../../styles/board/messages/message.css';
import MessageIcon from "../MessageIcon";
import {convertDate} from "../../../utils/dates";
import {MESSAGE_ICON} from "../../../constants";

const Message = props => {
    const {invoked, origin, message, level} = props.message;

    const date = convertDate(invoked);
    return (
        <div className="message">
            <MessageIcon level={level} usedIn={MESSAGE_ICON}/>
            <div className="message-date">{date}</div>
            <div className="message-service">{origin}</div>
            <div className="message-text">{message}</div>
        </div>
    )
}

export default Message
