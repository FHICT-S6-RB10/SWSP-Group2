import React, {useState} from 'react';
import '../../styles/board/board.css';
import TabContainer from "./tabs/TabContainer";
import MessageContainer from "./messages/MessageContainer";
import {useSelector} from "react-redux";
import Chart from "./Chart";
import {ERROR, LOG, WARNING} from "../../constants";

const Board = () => {
    const {selectedServices} = useSelector(state => state.services);
    const {messages} = useSelector(state => state.messages);
    const {selectedTenant} = useSelector(state => state.tenants);

    const [selectedTabs, setSelectedTabs] = useState([LOG, WARNING, ERROR]);

    return (
        <div className="board">
            <Chart
                selectedServices={selectedServices}
                selectedTenant={selectedTenant}
                messages={messages}
            />
            <TabContainer
                selectedTabs={selectedTabs}
                setSelectedTabs={setSelectedTabs}
            />
            <MessageContainer
                selectedTabs={selectedTabs}
                selectedServices={selectedServices}
                selectedTenant={selectedTenant}
                messages={messages}
            />
        </div>
    )
}

export default Board
