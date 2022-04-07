import React, {useState} from 'react';
import '../../styles/board/board.css';
import TabContainer from "./tabs/TabContainer";
import MessageContainer from "./messages/MessageContainer";
import {useSelector} from "react-redux";
import Chart from "./Chart";

const Board = () => {
    const {selectedServices} = useSelector(state => state.services);

    const [selectedTabs, setSelectedTabs] = useState([]);

    return (
        <div className="board">
            <Chart
                selectedServices={selectedServices}
            />
            <TabContainer
                selectedTabs={selectedTabs}
                setSelectedTabs={setSelectedTabs}
            />
            <MessageContainer
                selectedTabs={selectedTabs}
                selectedServices={selectedServices}
            />
        </div>
    )
}

export default Board
