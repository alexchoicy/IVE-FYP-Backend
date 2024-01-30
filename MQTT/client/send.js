const mqtt = require('mqtt');
const env = require('dotenv').config();
const client = mqtt.connect(process.env.MQTT_HOST);
client.subscribe('plate');

client.on("connect", () => {
    client.publish("plate", "Hello mqtt", {}, function(err) {
        if (!err) {
            client.end();
        }
    });
});