MQTT

create a MQTT broker

in the broker config i setted anonymous allow
```
    docker compose up
```

for mqtt test
```
    npm install
```

.env 
```
    MQTT_HOST=
```


client.js is to listen all message of 'plate' Topic

send.js is a to send a message to 'plate' Topic 
