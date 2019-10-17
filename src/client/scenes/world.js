const sceneFactory = require('../../shared/scenes/sceneFactory.js');

const tileMap = require('../../shared/gameObjects/tileMap.js');
const player = require('../../shared/gameObjects/entity/player.js');
const vector2 = require('../../shared/util/vector2.js');

module.exports = sceneFactory(0, function(s, game, user) {
    let map = new tileMap("tlm_map");
    map.transform.pos = new vector2(0, 0);
    s.addGameObject(map);
    let p = new player("player", new vector2(160, 160), new vector2(32, 32), 1, map, user);
    p.transform.pos = new vector2(160, 160);
    p.transform.size = new vector2(32, 32);
    s.addGameObject(p);
});
