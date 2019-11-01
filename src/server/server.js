const connection = require('./network/connection.js');
const packet = require('./network/packet.js');
const WebSocket = require('ws');
const logger = require('./util/logger.js');

module.exports = class server {
    constructor(config) {
        this.connections = [];
        this.config = config;

        logger.logToConsole = config.log.console || false;
        logger.logToFile = config.log.filePath !== undefined && config.log.filePath.length > 0;
        logger.filePath = config.log.filePath;
        if (config.log.logLevel) logger.logLevelLimit = config.log.logLevel;
        
        logger.instance.log("Fantalis Online Server " + config.version);
        logger.instance.log("Config loaded, logger configured.");
        logger.instance.log('Logger configuration:'
            + '\nconsole: ' + logger.logToConsole
            + '\nfile: ' + logger.logToFile
            + (logger.logToFile ? '\nfilepath: ' + logger.filePath : '')
            + '\nlogLevel: ' + logger.logLevelLimit);

        let nodeVersions = process.versions;
        let systemStats = 'Node info:';
        for (let dep in nodeVersions) {
            if (!nodeVersions.hasOwnProperty(dep)) continue;
            systemStats += '\n' + dep + ': ' + nodeVersions[dep];
        }
        logger.instance.log(systemStats, 4);
        
        logger.instance.log('Current platform: ' + process.platform);

        let trackedEnvVars = config.debug.trackedEnvironmentVariables
            || [
                'USER', 'SHELL', 'LOGNAME', 'HOME', 'PWD'
            ];
        let envVariables = 'Environment Variables:';
        for (let env in process.env) {
            if (!process.env.hasOwnProperty(env) || !trackedEnvVars.includes(env)) continue;
            envVariables += '\n' + env + ': ' + process.env[env];
        }
        logger.instance.log(envVariables, 4);
    }

    start() {
        logger.instance.log("Starting...");
        logger.instance.log("Binding websocket...", 4);
        this.wss = new WebSocket.Server({
            port: this.config.server.port,
            clientTracking: true
        });
        logger.instance.log("Websocet server bound, hooking events...", 4);

        this.wss.on('connection', (ws, req) => this.newConnection.bind(this, ws, req)());
        logger.instance.log("Events hooked.", 4);
        logger.instance.log("Started, waiting for connections...");
    }

    newConnection(ws, req) {
        let conn = new connection(ws, this, req.connection.remoteAddress);
        conn.addEventListener('message', (message) => this.onMessage.bind(this, message)());
        this.connections.push(conn);
        logger.instance.log('client connected: ' + JSON.stringify(req.connection.remoteAddress));
    }

    onMessage(message) {
        let p = packet.decode(message);
        logger.instance.log('received: [%s] %s', packet.messageType[p.get("type")], p.get("data"), 4);
    }
}