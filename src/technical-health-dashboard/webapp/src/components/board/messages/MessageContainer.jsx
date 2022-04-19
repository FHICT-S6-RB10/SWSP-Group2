import React, {useEffect, useState} from 'react';
import '../../../styles/board/messages/messageContainer.css';
import Message from "./Message";
import {sortMessagesByDate} from "../../../utils/messages";

const MessageContainer = (props) => {
    const {selectedTabs, selectedServices, messages} = props;

    const [filteredMessages, setFilteredMessages] = useState([]);
    const [styledMessages, setStyledMessages] = useState([]);

    const selectedByOrigin = origin => {
        return selectedServices.indexOf(origin) > -1;
    }

    const selectedByLevel = level => {
        return selectedTabs.indexOf(level) > -1;
    }

    useEffect(() => {
        let newFilteredMessages;

        if(selectedServices.length === 0) {
            newFilteredMessages = messages.filter(message => selectedByLevel(message.level));
        } else {
            newFilteredMessages = messages.filter(message => selectedByLevel(message.level) && selectedByOrigin(message.origin));
        }

        sortMessagesByDate(newFilteredMessages);

        setFilteredMessages(newFilteredMessages);

    }, [selectedTabs, selectedServices, messages])

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
