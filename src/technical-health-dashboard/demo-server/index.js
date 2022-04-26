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
    UNKNOWN,
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
    {name: "Technical Health Service", status: parseInt(ONLINE_STATUS)},
    {name: "Sensor Data Service", status: parseInt(OFFLINE_STATUS)},
    {name: "Raw Data Service", status: parseInt(HAS_ERRORS_STATUS)}
];

const messages = [
    {id: 1, level: parseInt(LOG), invoked: "2022-04-14Т18:49:41.2623951+02:00", origin: "Technical Health Service", message: "Service working as expected"},
    {id: 2, level: parseInt(LOG), invoked: "2022-04-14Т18:49:41.2623951+02:00", origin: "Technical Health Service", message: "Service working as expected"},
    {id: 3, level: parseInt(WARNING), invoked: "2022-04-14Т18:49:41.2623951+02:00", origin: "Technical Health Service", message: "Connection is weak!"},
    {id: 4, level: parseInt(WARNING), invoked: "2022-04-14Т18:49:41.2623951+02:00", origin: "Technical Health Service", message: "Connection is weak!"},
    {id: 5, level: parseInt(ERROR), invoked: "2022-04-14Т18:49:41.2623951+02:00", origin: "Technical Health Service", message: "Not receiving heartbeat!"},

    {id: 6, level: parseInt(LOG), invoked: "2022-04-14Т18:49:41.2623951+02:00", origin: "Sensor Data Service", message: "Service working as expected"},
    {id: 7, level: parseInt(UNKNOWN), invoked: "2022-04-14Т18:49:41.2623951+02:00", origin: "Sensor Data Service", message: "Service working as expected"},
    {id: 8, level: parseInt(WARNING), invoked: "2022-04-14Т18:49:41.2623951+02:00", origin: "Sensor Data Service", message: "Processing takes longer than expected!"},
    {id: 9, level: parseInt(LOG), invoked: "2022-04-14Т18:49:41.2623951+02:00", origin: "Sensor Data Service", message: "Service working as expected"},
    {id: 10, level: parseInt(LOG), invoked: "2022-04-14Т18:49:41.2623951+02:00", origin: "Sensor Data Service", message: "Service working as expected"},

    {id: 11, level: parseInt(ERROR), invoked: "2022-04-14Т18:49:41.2623951+02:00", origin: "Raw Data Service", message: "Stopped receiving data!"},
    {id: 12, level: parseInt(ERROR), invoked: "2022-04-14Т18:49:41.2623951+02:00", origin: "Raw Data Service", message: "Stopped receiving data!"},
    {id: 13, level: parseInt(LOG), invoked: "2022-04-14Т18:49:41.2623951+02:00", origin: "Raw Data Service", message: "Service working as expected"},
    {id: 14, level: parseInt(WARNING), invoked: "2022-04-14Т18:49:41.2623951+02:00", origin: "Raw Data Service", message: "Default data format is not set!"},
    {id: 15, level: parseInt(LOG), invoked: "2022-04-14Т18:49:41.2623951+02:00", origin: "Raw Data Service", message: "Service working as expected"},
]

const wss = new WebSocketServer({ port: 8080 });

wss.on('connection', ws => {
    console.log('Client connected');

    ws.send(JSON.stringify({services, messages}));

    // let serviceStatus = 0
    // setInterval(() => {
    //     if (serviceStatus === 2) {
    //         serviceStatus = 0
    //     } else {
    //         serviceStatus++
    //     }
    //
    //     const testService = {
    //         name: "Test Service",
    //         status: serviceStatus
    //     }
    //
    //     ws.send(JSON.stringify({services: [testService], messages: []}));
    // }, 3000);
    //
    // let messageId = messages.length;
    // let messageLevel = 0;
    // setInterval(() => {
    //     if (messageLevel === 3) {
    //         messageLevel = 0
    //     } else {
    //         messageLevel++
    //     }
    //
    //     messageId++;
    //
    //     const testMessage = {
    //         id: messageId,
    //         level: messageLevel,
    //         invoked: "2022-04-14Т18:49:41.2623951+03:00",
    //         origin: "Test Service",
    //         message: "Random message."
    //     }
    //
    //     ws.send(JSON.stringify({services: [], messages: [testMessage]}));
    // }, 4000);
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
