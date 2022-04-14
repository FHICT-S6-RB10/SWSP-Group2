const express = require("express");
const cors = require("cors");
const { WebSocketServer } = require('ws');
const dotenv = require("dotenv");
dotenv.config();

const {
    OFFLINE_STATUS,
    ONLINE_STATUS,
    HAS_ERRORS_STATUS,
    CLIENT_URL,
    CLIENT_PORT,
    PORT,
    LOG,
    WARNING,
    ERROR
} = process.env;

const corsOptions = {
    origin: `${CLIENT_URL}:${CLIENT_PORT}`,
    optionSuccessStatus: 200
}

const app = express();
app.use(cors(corsOptions));

const services = [
    {name: "Technical Health Service", status: ONLINE_STATUS},
    {name: "Sensor Data Service", status: OFFLINE_STATUS},
    {name: "Raw Data Service", status: HAS_ERRORS_STATUS}
];

const messages = [
    {id: 1, type: LOG, date: "23-03-2022, 07:52", serviceName: "Technical Health Service", message: "Service working as expected"},
    {id: 2, type: LOG, date: "23-03-2022, 07:53", serviceName: "Technical Health Service", message: "Service working as expected"},
    {id: 3, type: WARNING, date: "23-03-2022, 07:54", serviceName: "Technical Health Service", message: "Connection is weak!"},
    {id: 4, type: WARNING, date: "23-03-2022, 07:55", serviceName: "Technical Health Service", message: "Connection is weak!"},
    {id: 5, type: ERROR, date: "23-03-2022, 07:56", serviceName: "Technical Health Service", message: "Not receiving heartbeat!"},

    {id: 6, type: LOG, date: "25-03-2022, 11:24", serviceName: "Sensor Data Service", message: "Service working as expected"},
    {id: 7, type: LOG, date: "25-03-2022, 11:25", serviceName: "Sensor Data Service", message: "Service working as expected"},
    {id: 8, type: WARNING, date: "25-03-2022, 11:26", serviceName: "Sensor Data Service", message: "Processing takes longer than expected!"},
    {id: 9, type: LOG, date: "25-03-2022, 11:27", serviceName: "Sensor Data Service", message: "Service working as expected"},
    {id: 10, type: LOG, date: "25-03-2022, 11:28", serviceName: "Sensor Data Service", message: "Service working as expected"},

    {id: 11, type: ERROR, date: "24-03-2022, 16:36", serviceName: "Raw Data Service", message: "Stopped receiving data!"},
    {id: 12, type: ERROR, date: "24-03-2022, 16:37", serviceName: "Raw Data Service", message: "Stopped receiving data!"},
    {id: 13, type: LOG, date: "24-03-2022, 16:38", serviceName: "Raw Data Service", message: "Service working as expected"},
    {id: 14, type: WARNING, date: "24-03-2022, 16:39", serviceName: "Raw Data Service", message: "Default data format is not set!"},
    {id: 15, type: LOG, date: "24-03-2022, 16:40", serviceName: "Raw Data Service", message: "Service working as expected"},
]

const wss = new WebSocketServer({ port: 8080 });

wss.on('connection', ws => {
    console.log('Client connected');

    const data = {services, messages}
    ws.send(JSON.stringify(data));

    setInterval(() => {
        data.messages.push({
                id: messages.length + 1,
                type: WARNING,
                date: new Date().toLocaleDateString("en-EU", {
                    year: 'numeric',
                    month: '2-digit',
                    day: '2-digit',
                    hour: 'numeric',
                    hour12: false,
                    minute: 'numeric'
                }).replace(/\//g, '-'),
                serviceName: "Raw Data Service",
                message: "Default data format is not set!"
            });

        ws.send(JSON.stringify(data));
    }, 3000);

    setTimeout(() => {
        data.services.push({
            name: "Test Service",
            status: ONLINE_STATUS
        });

        ws.send(JSON.stringify(data));
    }, 5000)
});

app.get('/servicestates', (req, res) => {
    res.status(200).send(services);
});

app.get('/messages', (req, res) => {
    res.status(200).send(messages);
})

app.listen(PORT, () => {
    console.log(`Listening on port ${PORT}`);
});
