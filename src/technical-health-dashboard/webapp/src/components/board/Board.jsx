import React, {useEffect, useState} from 'react';
import '../../styles/board/board.css';
import TabContainer from "./tabs/TabContainer";
import MessageContainer from "./messages/MessageContainer";
import {useDispatch, useSelector} from "react-redux";
import Chart from "./Chart";
import {getMessages} from "../../store/actions/messageActions";

const Board = () => {
    const dispatch = useDispatch();
    const {selectedServices} = useSelector(state => state.services);
    const {messages} = useSelector(state => state.messages);

    const [selectedTabs, setSelectedTabs] = useState([]);

    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    useEffect(() => {
        setLoading(true);
        dispatch(getMessages())
            .then(() => setLoading(false))
            .catch((error) => {
                setLoading(false);
                setError(error);
            });
    }, [dispatch])

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

            {loading && (
                <div>
                    Loading...
                </div>
            )}

            {error && (
                <div>
                    {error}
                </div>
            )}
        </div>
    )
}

export default Board
