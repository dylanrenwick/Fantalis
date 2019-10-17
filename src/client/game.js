const clientScene = require('./scenes/clientScene.js');
const eventHandler = require('../shared/util/eventHandler.js');
const networkManager = require('./network/networkManager.js');

module.exports = class game extends eventHandler {
    constructor(canvas) {
        super([
            "mouseDown",
            "mouseUp",
            "mouseMove",
            "keyDown",
            "keyUp"
        ]);

        this.canvas = canvas;
        this.ctx = canvas.getContext('2d');

        this.canvas.width = 800;
        this.canvas.height = 480;

        this.canvas.setAttribute("tabindex", 0);

        this.canvas.addEventListener("mousedown", (e) => this.triggerEvent("mouseDown", e));
        this.canvas.addEventListener("mouseup", (e) => this.triggerEvent("mouseUp", e));
        this.canvas.addEventListener("mousemove", (e) => this.triggerEvent("mouseMove", e));
        this.canvas.addEventListener("keydown", (e) => this.triggerEvent("keyDown", e));
        this.canvas.addEventListener("keyup", (e) => this.triggerEvent("keyUp", e));

        this.config = {};
        this.httpReq = new XMLHttpRequest();
        this.httpReq.addEventListener("readystatechange", (e) => { this.onConfigLoad.bind(this, e)(); });
        this.httpReq.open("GET", "bin/client/config.json");
        this.httpReq.send();

        this.currentScene = clientScene.byId(0, this);
        this.currentScene.setActive(this);

        this.drawInterval = setInterval(() => this.draw.bind(this)(), 1000 / 60);
    }

    draw() {
        if (!this.currentScene) return;
        this.currentScene.draw(this.ctx);
    }

    onConfigLoad(e) {
        if (this.httpReq.readyState === XMLHttpRequest.DONE && this.httpReq.status === 200) {
            try {
                this.config = JSON.parse(this.httpReq.responseText);
                this.netManager = new networkManager(this.config);
                this.netManager.addEventListener("login", (ev) => this.loadWorld(ev));
            } catch (e) {
                console.error("Could not load config.json!");
                throw e;
            }
        }
    }

    setActiveScene(id) {
        let args = Array.from(arguments);
        args.splice(0, 1);
        this.currentScene = clientScene.byId(id, this, ...args);
        this.ctx.fillStyle = "white";
        this.ctx.fillRect(0, 0, 800, 480);
        this.currentScene.setActive(this);
        this.player = this.currentScene.findGameObject("player");
    }

    loadWorld(e) {
        this.setActiveScene(1, e);
    }
}