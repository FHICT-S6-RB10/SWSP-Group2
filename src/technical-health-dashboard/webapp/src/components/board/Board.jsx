import React, {useState} from 'react';
import '../../styles/board/board.css';
import TabContainer from "./tabs/TabContainer";
import MessageContainer from "./messages/MessageContainer";
import {useSelector} from "react-redux";
import Chart from "./Chart";
import {ERROR, LOG, UNKNOWN, WARNING} from "../../constants";

const Board = () => {
    const {selectedServices} = useSelector(state => state.services);
    const {messages} = useSelector(state => state.messages);

    const [selectedTabs, setSelectedTabs] = useState([UNKNOWN, LOG, WARNING, ERROR]);

    return (
        <div className="board">
            <Chart
                selectedServices={selectedServices}
                messages={messages}
            />
            <TabContainer
                selectedTabs={selectedTabs}
                setSelectedTabs={setSelectedTabs}
            />
            <MessageContainer
                selectedTabs={selectedTabs}
                selectedServices={selectedServices}
                messages={messages}
            />
        </div>
    )
}

export default Board
