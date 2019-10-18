import scene from '../../shared/scene/scene';
import sceneList from './sceneList';
import game from '../game';

export default class clientScene extends scene {
    static sceneList: Array<scene | ((...args: Array<any>) => scene)> = sceneList;

    game: game;

    constructor(id) {
        super(id);
    }

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

    static byId(id, game, ...args) {
        if (!clientScene.sceneList[id]) return null;
        let sceneItem: scene | ((...args: Array<any>) => scene) = clientScene.sceneList[id];
        if (!(sceneItem instanceof scene)) {
            args = [game].concat(args);
            clientScene.sceneList[id] = sceneItem(...args);
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
