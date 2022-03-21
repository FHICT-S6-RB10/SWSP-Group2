const express = require("express");
const cors = require("cors");
const dotenv = require("dotenv");
dotenv.config();

const {OFFLINE_STATUS, ONLINE_STATUS, HAS_ERRORS_STATUS, CLIENT_URL} = process.env;

const corsOptions = {
    origin: CLIENT_URL,
    optionSuccessStatus: 200
}

const app = express();
app.use(cors(corsOptions));

const services = [
    {name: "Technical Health Service", status: ONLINE_STATUS},
    {name: "Sensor Data Service", status: OFFLINE_STATUS},
    {name: "Raw Data Service", status: HAS_ERRORS_STATUS},
];

app.get('/servicestates', (req, res) => {
    res.send(services);
});

app.listen(4000, () => {
    console.log('Listening on port 4000');
});
