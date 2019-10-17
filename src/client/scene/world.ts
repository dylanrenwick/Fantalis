import sceneFactory from '../../shared/scene/sceneFactory';

import tileMap from '../../shared/gameObject/tileMap';
import player from '../../shared/gameObject/entity/player';
import vector2 from '../../shared/util/vector2';

export default sceneFactory(0, function(s, game, user) {
    let map = new tileMap("tlm_map");
    map.transform.pos = new vector2(0, 0);
    s.addGameObject(map);
    let p = new player("player", new vector2(160, 160), new vector2(32, 32), 1, map, user);
    p.transform.pos = new vector2(160, 160);
    p.transform.size = new vector2(32, 32);
    s.addGameObject(p);
});
