import React, {useEffect, useState} from 'react';
import '../../../styles/board/messages/messageContainer.css';
import Message from "./Message";
import {sortMessagesByDate} from "../../../utils/messages";

const MessageContainer = (props) => {
    const {selectedTabs, selectedServices, selectedTenant, messages} = props;

    const [filteredMessages, setFilteredMessages] = useState([]);
    const [styledMessages, setStyledMessages] = useState([]);

    const selectedByOrigin = origin => {
        return selectedServices.indexOf(origin) > -1;
    }

    const selectedByLevel = level => {
        return selectedTabs.indexOf(level.toString()) > -1;
    }

    const selectedByTenant = tenantId => {
        return selectedTenant === tenantId;
    }

    useEffect(() => {
        let newFilteredMessages = messages;

        if (selectedTenant) {
            newFilteredMessages = newFilteredMessages.filter(message => selectedByTenant(message.tenantId));
        }

        if (selectedServices.length > 0) {
            newFilteredMessages = newFilteredMessages.filter(message => selectedByOrigin(message.origin));
        }

        newFilteredMessages = newFilteredMessages.filter(message => selectedByLevel(message.level));

        sortMessagesByDate(newFilteredMessages);

        setFilteredMessages(newFilteredMessages);

    }, [selectedTabs, selectedServices, selectedTenant, messages])

    useEffect(() => {
        const newStyledMessages = filteredMessages.map(message => (
            <Message key={message.id} message={message}/>
        ));

        setStyledMessages(newStyledMessages);
    }, [filteredMessages]);

    return (
        <div className="message-container">
            <div className="message-list">
                {styledMessages}
            </div>
        </div>
    )
}

export default MessageContainer
