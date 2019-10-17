const gameObject = require('./gameObject.js');
const vector2 = require('../util/vector2.js');

const defaultMap = [
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1]
];
const tileSize = 32;

module.exports = class tileMap extends gameObject {
    constructor(name, map) {
        super(name);

        this.map = map || defaultMap;
        this.transform.size = new vector2(Math.max(...this.map.map(x => x.length)) * tileSize, this.map.length * tileSize);
        this.transform.addEventListener('mouseDown', (e) => this.moveToTile(e));
    }

    draw(ctx) {
        for (let y = 0; y < this.map.length; y++) {
            for (let x = 0; x < this.map[y].length; x++) {
                this.drawTile(this.map[y][x], ctx, x * tileSize, y * tileSize);
            }
        }
    }

    drawTile(id, ctx, x, y) {
        if (id === 1) {
            ctx.fillStyle = "green";
            ctx.fillRect(x, y, tileSize, tileSize);
        }
    }

    get(x, y) {
        if (this.map[y] && this.map[y][x])
            return this.map[y][x];
        else return null;
    }

    getTileByPos(pos) {
        let relativePos = pos.clone().sub(this.transform.pos);
        relativePos = new vector2(Math.floor(relativePos.x / tileSize), Math.floor(relativePos.y / tileSize));
        return relativePos;
    }

    getNeighbours(x, y) {
        return [
            {x: x-1, y: y-1, tile: this.get(x-1, y-1)},
            {x: x,   y: y-1, tile: this.get(x,   y-1)},
            {x: x+1, y: y-1, tile: this.get(x+1, y-1)},
            {x: x-1, y: y,   tile: this.get(x-1, y)},
            {x: x+1, y: y,   tile: this.get(x+1, y)},
            {x: x-1, y: y+1, tile: this.get(x-1, y+1)},
            {x: x,   y: y+1, tile: this.get(x,   y+1)},
            {x: x+1, y: y+1, tile: this.get(x+1, y+1)}
        ];
    }

    isBlocked(x, y) {
        return false;
    }

    moveToTile(e) {
        let mouseX = e.clientX;
        let mouseY = e.clientY;

        let tileX = Math.floor(mouseX / tileSize);
        let tileY = Math.floor(mouseY / tileSize);

        let player = this.scene.game.player;
        if (player !== null) {
            player.targetTile = new vector2(tileX, tileY);
        }
    }
}