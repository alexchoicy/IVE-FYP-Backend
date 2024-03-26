const WebSocket = require('ws');
require('dotenv').config()

const jwt = process.env.ADMIN;

const config = {
  headers: {
    'Authorization': `Bearer ${jwt}`
  }
}

const wsURL = process.env.wsURL;
function cws() {

  const ws = new WebSocket(wsURL, config);

  ws.on('open', () => {
    console.log('WebSocket connection established.');
    // You can start sending/receiving messages here
  });

  // Event handler for incoming messages
  ws.on('message', (data) => {
    console.log('Received message:', data);
  });

  // Event handler when the connection is closed
  ws.on('close', (code, reason) => {
    console.log(`WebSocket connection closed (code ${code}): ${reason}`);
    setTimeout(cws, 5000);
  });
}

cws();
