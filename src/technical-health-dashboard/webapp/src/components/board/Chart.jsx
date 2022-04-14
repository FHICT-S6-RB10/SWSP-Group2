import React, {useEffect, useState} from 'react';
import '../../styles/board/chart.css';
import { PieChart } from 'react-minimal-pie-chart';
import {colors, ERROR, LOG, WARNING} from "../../constants";

const Chart = props => {
    const {selectedServices, messages} = props;

    const [filteredMessages, setFilteredMessages] = useState([]);
    const [chartData, setChartData] = useState([]);
    const [chartLabel, setChartLabel] = useState('');

    const selectedByService = serviceName => {
        return selectedServices.indexOf(serviceName) > -1;
    }

    useEffect(() => {
        let newFilteredMessages = messages;

        if(selectedServices.length !== 0) {
            newFilteredMessages = messages.filter(message => selectedByService(message.serviceName));
        }

        setFilteredMessages(newFilteredMessages);

    }, [selectedServices, messages]);

    useEffect(() => {
        const counts = {};

        filteredMessages.forEach(message => {
            const type = message.type;

            if (!counts[type]) {
                counts[type] = 0;
            }

            counts[type] += 1;
        });

        const newChartData = Object.entries(counts).map(entry => {
            const type = entry[0];
            const value = entry[1];

            return {title: type, value: value, color: colors[type]}
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
                />
            )}
            <div className='chart-title'>
                <span className={`chart-title-${LOG}`}>
                    Logs
                </span>
                <span className='dash'>/</span>
                <span className={`chart-title-${WARNING}`}>
                    Warnings
                </span>
                <span className='dash'>/</span>
                <span className={`chart-title-${ERROR}`}>
                    Errors
                </span>
            </div>
        </div>
    )
}

export default Chart
