import eventHandler from '../../shared/util/eventHandler';
import packet from '../../shared/network/packet';

export default class networkManager extends eventHandler {
    static instance: networkManager;
    static getInstance() {
        return networkManager.instance;
    }

    config: any;
    socket: WebSocket;

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
            if (res.get("type") !== packet.messageType["AUTH_VERSION"]) return false;
            if (res.get("version") !== this.config.version || res.get("protocolVersion") !== this.config.server.version) {
                throw new Error(`Invalid version! Client has ${this.config.version}:${this.config.server.version} but server wants ${res.get("version")}:${res.get("protocolVersion")}!`);
            }
            console.log(`Server has version: ${res.get("version")}:${res.get("protocolVersion")}`);
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