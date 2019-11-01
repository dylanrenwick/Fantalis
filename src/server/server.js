const connection = require('./network/connection.js');
const packet = require('./network/packet.js');
const WebSocket = require('ws');
const logger = require('../util/logger.js');

module.exports = class server {
    constructor(config) {
        this.wss = new WebSocket.Server({
            port: config.server.port,
            clientTracking: true
        });

        this.connections = [];
        this.config = config;

        logger.logToConsole = config.log.console || false;
        logger.logToFile = config.log.filePath !== undefined && config.log.filePath.length > 0;
        logger.filePath = config.log.filePath;
        logger.logLevelLimit = config.log.logLevel;

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