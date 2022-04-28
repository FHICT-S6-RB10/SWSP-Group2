import React, {useEffect, useState} from 'react';
import '../../styles/board/chart.css';
import { PieChart } from 'react-minimal-pie-chart';
import {messageColors, messageTitles} from "../../constants";

const Chart = props => {
    const {selectedServices, messages} = props;

    const [filteredMessages, setFilteredMessages] = useState([]);

    const [chartData, setChartData] = useState([]);

    const [counts, setCounts] = useState({});
    const [styledCounts, setStyledCounts] = useState([]);

    const selectedByOrigin = origin => {
        return selectedServices.indexOf(origin) > -1;
    }

    useEffect(() => {
        let newFilteredMessages = messages;

        if(selectedServices.length !== 0) {
            // If the service that the message originates from is selected
            newFilteredMessages = messages.filter(message => selectedByOrigin(message.origin));
        }

        setFilteredMessages(newFilteredMessages);

    }, [selectedServices, messages]);

    useEffect(() => {
        const newCounts = {};

        filteredMessages.forEach(message => {
            const level = message.level;

            // If there is no previous record for this type of message create one
            if (!newCounts[level]) {
                newCounts[level] = 0;
            }

            // For each message of a type add to this type's count
            newCounts[level] += 1;
        });

        setCounts(newCounts);

        // Update chart data based on new messages
        const newChartData = Object.entries(newCounts).map(entry => {
            const level = entry[0];
            const value = entry[1];

            return {
                value,
                title: `${messageTitles[level]}s`,
                color: messageColors[level]
            }
        });

        setChartData(newChartData);
    }, [filteredMessages]);

    useEffect(() => {
        const newStyledCounts = Object.entries(counts).map(entry => {
            const level = entry[0];
            const count = entry[1];

            return (
                <span style={{color: `${messageColors[level]}`}} key={level}>
                    {count} {messageTitles[level]}{count !== 1 && 's'}
                </span>
            )
        });

        setStyledCounts(newStyledCounts);
    }, [counts]);

    return (
        <div className="chart">
            {chartData.length > 0 && (
                <PieChart
                    radius={50}
                    animate={true}
                    data={chartData}
                    lineWidth={20}
                    startAngle={270}
                />
            )}
            <div className='chart-title'>
                {styledCounts}
            </div>
        </div>
    )
}

export default Chart
