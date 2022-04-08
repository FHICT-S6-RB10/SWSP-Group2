import React, {useEffect, useState} from 'react';
import '../../../styles/board/messages/messageContainer.css';
import Message from "./Message";
import {sortMessagesByDate} from "../../../utils/messages";

const MessageContainer = (props) => {
    const {selectedTabs, selectedServices, messages} = props;

    const [filteredMessages, setFilteredMessages] = useState([]);

    const selectedByService = serviceName => {
        return selectedServices.indexOf(serviceName) > -1;
    }

    const selectedByType = type => {
        return selectedTabs.indexOf(type) > -1;
    }

    useEffect(() => {
        let newFilteredMessages;

        if(selectedServices.length === 0) {
            newFilteredMessages = messages.filter(message => selectedByType(message.type));
        } else {
            newFilteredMessages = messages.filter(message => selectedByType(message.type) && selectedByService(message.serviceName));
        }

        sortMessagesByDate(newFilteredMessages);

        setFilteredMessages(newFilteredMessages);

    }, [selectedTabs, selectedServices])

    const styledMessages = filteredMessages.map(message => (
        <Message key={message.id} message={message}/>
    ));

    return (
        <div className="message-container">
            <div className="message-list">
                {styledMessages}
            </div>
        </div>
    )
}

export default MessageContainer
