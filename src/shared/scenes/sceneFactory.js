const scene = require("./scene.js");
const game = require("../../client/game.js");

module.exports = function sceneFactory(sceneID, pred) {
    return function() {
        let newScene = new scene(sceneID);
        let args = [newScene].concat(Array.from(arguments));
        pred(...args);
        return newScene;
    }
}