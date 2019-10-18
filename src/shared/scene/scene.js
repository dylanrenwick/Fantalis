const gameObject = require('../gameObject/gameObject.js');

module.exports = class scene {
    constructor(id) {
        this.id = id;
        this.gameObjects = [];
    }

    addGameObject(obj) {
        this.gameObjects.push(obj);
        obj.scene = this;
    }

    findGameObject(name) {
        let results = this.gameObjects.filter(x => x.name === name);
        if (results.length === 0) return null;
        return results[0];
    }
}