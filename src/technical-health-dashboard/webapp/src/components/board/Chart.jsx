import React, {useEffect, useState} from 'react';
import '../../styles/board/chart.css';
import { PieChart } from 'react-minimal-pie-chart';
import {messageColors, messageTitles} from "../../constants";
import ReactTooltip from "react-tooltip";

const Chart = props => {
    const {selectedServices, messages} = props;

    const [filteredMessages, setFilteredMessages] = useState([]);

    const [chartData, setChartData] = useState([]);
    const [hovered, setHovered] = useState(null);

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
                color: messageColors[level],
                name: messageTitles[level]
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

    const getTooltip = entry => {
        return `${entry.name}s`;
    }

    return (
        <div className="chart-container">
            {chartData.length > 0 && (
                <div data-tip="" data-for="chart">
                    <PieChart
                        radius={50}
                        animate={true}
                        data={chartData}
                        lineWidth={20}
                        startAngle={270}
                        onMouseOver={(_, index) => {
                            setHovered(index);
                        }}
                        onMouseOut={() => {
                            setHovered(null);
                        }}
                    />
                    <ReactTooltip
                        id="chart"
                        getContent={() =>
                            typeof hovered === 'number' ? getTooltip(chartData[hovered]) : null
                        }
                    />
                </div>
            )}
            <div className='chart-title'>
                {styledCounts}
            </div>
            <ReactTooltip/>
        </div>
    )
}

export default Chart
