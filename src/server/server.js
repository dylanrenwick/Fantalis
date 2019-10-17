const connection = require('./network/connection.js');
const packet = require('./network/packet.js');
const WebSocket = require('ws');

module.exports = class server {
    constructor(config) {
        this.wss = new WebSocket.Server({
            port: config.server.port,
            clientTracking: true
        });

        this.connections = [];
        this.config = config;

        this.wss.on('connection', (ws, req) => this.newConnection.bind(this, ws, req)());
    }

    newConnection(ws, req) {
        let conn = new connection(ws, this, req.connection.remoteAddress);
        conn.addEventListener('message', (message) => this.onMessage.bind(this, message)());
        this.connections.push(conn);
        console.log('client connected: ' + JSON.stringify(req.connection.remoteAddress));
    }

    onMessage(message) {
        let p = packet.decode(message);
        console.log('received: [%s] %s', packet.messageType[p.get("type")], p.get("data"));
    }
}