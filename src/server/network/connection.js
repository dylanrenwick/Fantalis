const authStatus = require('./authStatus.js');
const eventHandler = require('../util/eventHandler.js');
const packet = require('./packet.js');
const logger = require('../util/logger.js');

module.exports = class connection extends eventHandler {
    constructor(socket, server, address) {
        super([
            "message",
            "close"
        ]);

        this.socket = socket;
        this.server = server;
        this.address = address;

        this.authStatus = new authStatus(address);

        this.socket.on("message", (e) => this.triggerEvent("message", e, this));
        this.socket.on("close", (e) => this.triggerEvent("close", e, this));

        this.connectedTime = Date.now();
        this.lastSendActivity = Date.now();
        this.lastReceiveActivity = Date.now();

        this.addEventListener("message", () => this.lastReceiveActivity = Date.now());
        this.addEventListener("message", (e, conn) => this.processMessage(e));
    }

    send(pack) {
        logger.instance.log(`sending [${packet.messageType[pack.get("type")]}] packet to ${this.address}`, 4);
        this.socket.send(pack.encode());
        this.lastSendActivity = Date.now();
    }
    sendWithCallback(pack, callback) {
        this.addTemporaryEventListener("message", callback);
        this.socket.send(pack.encode());
    }

    close(code, reason) {
        if (code === undefined) code = 1000;
        logger.instance.log(`client ${this.address} closing server-side with code ${code}${reason !== undefined ? ` and reason '${reason}'` : ''}`)
        this.socket.close(code, reason);
    }

    processMessage(e) {
        let message = packet.decode(e);
        switch (message.get("type")) {
            case packet.messageType["AUTH_VERSION"]:
                if (this.authStatus.versionVerified) return;
                let version = message.get("version");
                let protocolVersion = message.get("protocolVersion");
                if (version === this.server.config.version && protocolVersion === this.server.config.server.version) {
                    this.authStatus.versionVerified = true;
                    let pack = new packet({
                        version: this.server.config.version,
                        protocolVersion: this.server.config.server.version
                    }, packet.messageType["AUTH_VERSION"]);
                    this.send(pack);
                    return;
                } else {
                    logger.instance.log(`connection ${conn.address} is running an outdated client! Client is: ${version}:${protocolVersion} but server is: ${this.server.config.version}:${this.server.config.server.version}`, 2);
                    this.close(1002, "Outdated client");
                    return;
                }
            case packet.messageType["AUTH_LOGIN"]:
                let pack = new packet({
                    status: 1,
                    user: {
                        id: 1
                    }
                }, packet.messageType["AUTH_LOGIN"]);
                this.send(pack);
                return;
        }
    }
}