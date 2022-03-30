import {ERROR, WARNING, LOG} from "./constants";

export const demoMessages = [
    {id: 1, type: LOG, date: "23-03-2022 07:52", serviceName: "Technical Health Service", message: "Service working as expected"},
    {id: 2, type: LOG, date: "23-03-2022 07:53", serviceName: "Technical Health Service", message: "Service working as expected"},
    {id: 3, type: WARNING, date: "23-03-2022 07:54", serviceName: "Technical Health Service", message: "Connection is weak!"},
    {id: 4, type: WARNING, date: "23-03-2022 07:55", serviceName: "Technical Health Service", message: "Connection is weak!"},
    {id: 5, type: ERROR, date: "23-03-2022 07:56", serviceName: "Technical Health Service", message: "Not receiving heartbeat!"},

    {id: 6, type: LOG, date: "25-03-2022 11:24", serviceName: "Sensor Data Service", message: "Service working as expected"},
    {id: 7, type: LOG, date: "25-03-2022 11:25", serviceName: "Sensor Data Service", message: "Service working as expected"},
    {id: 8, type: WARNING, date: "25-03-2022 11:26", serviceName: "Sensor Data Service", message: "Processing takes longer than expected!"},
    {id: 9, type: LOG, date: "25-03-2022 11:27", serviceName: "Sensor Data Service", message: "Service working as expected"},
    {id: 10, type: LOG, date: "25-03-2022 11:28", serviceName: "Sensor Data Service", message: "Service working as expected"},

    {id: 11, type: ERROR, date: "24-03-2022 16:36", serviceName: "Raw Data Service", message: "Stopped receiving data!"},
    {id: 12, type: ERROR, date: "24-03-2022 16:37", serviceName: "Raw Data Service", message: "Stopped receiving data!"},
    {id: 13, type: LOG, date: "24-03-2022 16:38", serviceName: "Raw Data Service", message: "Service working as expected"},
    {id: 14, type: WARNING, date: "24-03-2022 16:39", serviceName: "Raw Data Service", message: "Default data format is not set!"},
    {id: 15, type: LOG, date: "24-03-2022 16:40", serviceName: "Raw Data Service", message: "Service working as expected"},
];
