const express = require("express");
const cors = require("cors");
const dotenv = require("dotenv");
dotenv.config();

const {OFFLINE_STATUS, ONLINE_STATUS, HAS_ERRORS_STATUS, CLIENT_URL, CLIENT_PORT, PORT} = process.env;

const corsOptions = {
    origin: `${CLIENT_URL}:${CLIENT_PORT}`,
    optionSuccessStatus: 200
}

const app = express();
app.use(cors(corsOptions));

const services = [];

app.get('/servicestates', (req, res) => {
    res.send(services);
});

app.get('/servicestatesmock', (req, res) => {
    services.push({name: "Technical Health Service", status: ONLINE_STATUS});
    services.push({name: "Sensor Data Service", status: OFFLINE_STATUS});
    services.push({name: "Raw Data Service", status: HAS_ERRORS_STATUS});
});

app.listen(PORT, () => {
    console.log(`Listening on port ${PORT}`);
});
