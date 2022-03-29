import React, {useState} from 'react';
import '../../styles/board/board.css';
import TabContainer from "./tabs/TabContainer";
import MessageContainer from "./messages/MessageContainer";
import {useSelector} from "react-redux";

const Board = () => {
    const {selectedServices} = useSelector(state => state.services);

    const [selectedTabs, setSelectedTabs] = useState([]);

    return (
        <div className="board">
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
