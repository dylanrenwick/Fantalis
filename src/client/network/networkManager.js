const eventHandler = require('../../shared/util/eventHandler.js');
const packet = require('../../shared/network/packet.js');

module.exports = class networkManager extends eventHandler {
    static getInstance() {
        return networkManager.instance;
    }

    constructor(config) {
        super([
            "open",
            "message",
            "login"
        ]);

        this.config = config;

        this.socket = new WebSocket(`ws://${this.config.server.host}:${this.config.server.port}`);
        this.socket.addEventListener('open', (e) => this.onLoad(e));
        this.socket.addEventListener('message', (e) => this.onMessage(e));

        networkManager.instance = this;
    }

    onLoad(e) {
        this.sendVersionCheck();
        this.triggerEvent("open", e);
    }

    onMessage(e) {
        this.triggerEvent("message", e);
    }

    sendVersionCheck() {
        let pack = new packet({
            version: this.config.version,
            protocolVersion: this.config.server.version
        }, packet.messageType["AUTH_VERSION"]);
        this.addTemporaryEventListener("message", (e) => {
            let res = packet.decode(e.data);
            if (res.type !== packet.messageType["AUTH_VERSION"]) return false;
            if (res.version !== this.config.version || res.protocolVersion !== this.config.server.version) {
                throw new Error(`Invalid version! Client has ${this.config.version}:${this.config.server.version} but server wants ${res.version}:${res.protocolVersion}!`);
            }
            console.log(`Server has version: ${res.version}:${res.protocolVersion}`);
            return true;
        });
        this.socket.send(pack.encode());
    }

    login(user, pass) {
        let pack = new packet({user, pass}, packet.messageType["AUTH_LOGIN"]);
        this.addTemporaryEventListener("message", (e) => {
            let res = packet.decode(e.data);
            if (res.get("type") !== packet.messageType["AUTH_LOGIN"]) return false;
            if (res.get("status") !== 1) {
                throw new Error(`Invalid login!`);
            }
            this.triggerEvent("login", res.get("user"));
        });
        this.socket.send(pack.encode());
    }
}