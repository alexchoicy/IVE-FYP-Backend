const mqtt = require('mqtt');
const env = require('dotenv').config();
const client = mqtt.connect(process.env.MQTT_HOST);
client.subscribe('plate');
client.on("message", (topic, message) => {
    // message is Buffer
    console.log([topic, message.toString()].join(": "));
});