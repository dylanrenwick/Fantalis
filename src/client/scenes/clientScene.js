const scene = require('../../shared/scenes/scene.js');

module.exports = class clientScene extends scene {
    draw(ctx) {
        this.gameObjects.forEach(x => x.draw(ctx));
    }

    setActive(game) {
        game.clearAllEventListeners();
        game.addEventListener("mouseDown", (e) => this.onMouseDown(e));
        game.addEventListener("mouseUp", (e) => this.onMouseUp(e));
        game.addEventListener("mouseMove", (e) => this.onMouseMove(e));
        game.addEventListener("keyDown", (e) => this.onKeyDown(e));
        game.addEventListener("keyUp", (e) => this.onKeyUp(e));
        this.game = game;
    }

    onMouseDown(e) {
        let caught = false;
        for (let gObj of this.gameObjects) {
            let result = gObj.transform.checkMouseDown(e, caught);
            if (!caught) caught = result;
        }
    }
    onMouseUp(e) {
        let caught = false;
        for (let gObj of this.gameObjects) {
            let result = gObj.transform.checkMouseUp(e, caught);
            if (!caught) caught = result;
        }
    }
    onMouseMove(e) {
        let caught = false;
        for (let gObj of this.gameObjects) {
            let result = gObj.transform.checkMouseMove(e, caught);
            if (!caught) caught = result;
        }
    }
    onKeyDown(e) {
        this.gameObjects.forEach(x => x.transform.onKeyDown(e));
    }
    onKeyUp(e) {
        this.gameObjects.forEach(x => x.transform.onKeyUp(e));
    }

    static byId(id, game) {
        if (!clientScene.sceneList[id]) return null;
        if (typeof(clientScene.sceneList[id]) === "function") {
            let args = Array.from(arguments);
            args.splice(0, 1);
            args = [game].concat(args);
            clientScene.sceneList[id] = clientScene.sceneList[id](...args);
        }
        
        if (clientScene.sceneList[id] instanceof scene) {
            return clientScene.fromScene(clientScene.sceneList[id]);
        }
        return null;
    }

    static fromScene(scene) {
        let cScene = new clientScene(scene.id);
        cScene.gameObjects = scene.gameObjects;
        cScene.gameObjects.forEach(gObj => gObj.scene = cScene);
        return cScene;
    }
}

module.exports.sceneList = require('./sceneList.js');