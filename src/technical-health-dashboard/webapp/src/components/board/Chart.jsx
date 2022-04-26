import React, {useEffect, useState} from 'react';
import '../../styles/board/chart.css';
import { PieChart } from 'react-minimal-pie-chart';
import {messageColors, ERROR, LOG, WARNING, UNKNOWN} from "../../constants";

const Chart = props => {
    const {selectedServices, messages} = props;

    const [filteredMessages, setFilteredMessages] = useState([]);
    const [chartData, setChartData] = useState([]);
    const [chartLabel, setChartLabel] = useState('');

    const selectedByOrigin = origin => {
        return selectedServices.indexOf(origin) > -1;
    }

    useEffect(() => {
        let newFilteredMessages = messages;

        if(selectedServices.length !== 0) {
            newFilteredMessages = messages.filter(message => selectedByOrigin(message.origin));
        }

        setFilteredMessages(newFilteredMessages);

    }, [selectedServices, messages]);

    useEffect(() => {
        const counts = {};

        filteredMessages.forEach(message => {
            const level = message.level;

            if (!counts[level]) {
                counts[level] = 0;
            }

            counts[level] += 1;
        });

        const newChartData = Object.entries(counts).map(entry => {
            const level = entry[0];
            const value = entry[1];

            return {title: level, value: value, color: messageColors[level]}
        })

        setChartData(newChartData);

        const newChartLabel = `${counts[LOG] || 0}/${counts[WARNING] || 0}/${counts[ERROR] || 0}`;
        setChartLabel(newChartLabel);

    }, [filteredMessages]);

    return (
        <div className="chart">
            {chartData.length > 0 && (
                <PieChart
                    radius={50}
                    animate={true}
                    data={chartData}
                    label={() => chartLabel}
                    lineWidth={15}
                    labelStyle={{
                        fontSize: '16px',
                        fill: 'black'
                    }}
                    labelPosition={0}
                    startAngle={270}
                />
            )}
            <div className='chart-title'>
                <span style={{color: `${messageColors[UNKNOWN]}`}}>
                    Unknown
                </span>
                <span className='dash'>/</span>
                <span style={{color: `${messageColors[LOG]}`}}>
                    Logs
                </span>
                <span className='dash'>/</span>
                <span style={{color: `${messageColors[WARNING]}`}}>
                    Warnings
                </span>
                <span className='dash'>/</span>
                <span style={{color: `${messageColors[ERROR]}`}}>
                    Errors
                </span>
            </div>
        </div>
    )
}

export default Chart
