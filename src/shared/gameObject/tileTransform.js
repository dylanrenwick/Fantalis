const plotAStarPath = require('../tile/tilePath.js');
const transform = require('./transform.js');

module.exports = class tileTransform extends transform {
    constructor(pos, size, parent, tileMap) {
        super(pos, size, parent);

        this.tileMap = tileMap;
    }

    getTilePos() {
        return this.tileMap.getTileByPos(this.pos);
    }

    plotPathTo(targetTile) {
        return plotAStarPath(this.getTilePos(), targetTile, this.tileMap);
    }
}