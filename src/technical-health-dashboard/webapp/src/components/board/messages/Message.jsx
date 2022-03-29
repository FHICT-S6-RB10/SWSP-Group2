import React from 'react';
import '../../../styles/board/messages/message.css';
import MessageIcon from "../MessageIcon";

const Message = (props) => {
    const {date, serviceName, message, type} = props.message;
    return (
        <div className="message">
            <MessageIcon type={type}/>
            <div className="message-date">{date}</div>
            <div className="message-service">{serviceName}</div>
            <div className="message-text">{message}</div>
        </div>
    )
}

export default Message
