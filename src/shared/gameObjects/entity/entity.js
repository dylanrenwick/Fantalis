const gameObject = require('../gameObject.js');
const tileTransform = require('../tileTransform.js');
const vector2 = require('../../util/vector2.js');

module.exports = class entity extends gameObject {
    constructor(name, pos, size, speed, tileMap) {
        super(name, pos, size);
        delete this.transform;
        this.transform = new tileTransform(pos, size, this, tileMap);

        this.speed = speed === undefined ? 1 : speed;
    }

    draw(ctx) {
        if (this.targetTile) {
            let tilePos = this.transform.getTilePos();
            if (tilePos.x === this.targetTile.x && tilePos.y === this.targetTile.y) {
                this.targetTile = null;
            } else {
                if (!this.tilePath || this.tilePath.length === 0) {
                    this.tilePath = this.transform.plotPathTo(this.targetTile);
                }

                let remainingSpeed = this.speed;

                // Lerp toward nextTile until we have no speed left
                while (remainingSpeed > 0) {
                    tilePos = this.transform.getTilePos();
                    if (tilePos.x === this.targetTile.x && tilePos.y === this.targetTile.y) {
                        this.targetTile = null;
                        break;
                    }

                    if (!this.nextTile) {
                        this.nextTile = this.tilePath.shift();
                    }
                    if (this.nextTile.x === tilePos.x && this.nextTile.y === tilePos.y) {
                        this.nextTile = this.tilePath.shift();
                    }

                    let nextTilePos = new vector2(this.nextTile.x, this.nextTile.y).mul(32);
                    let distToNext = this.transform.pos.distance(nextTilePos);
                    let lerpT = remainingSpeed / distToNext;
                    if (lerpT >= 1) {
                        remainingSpeed -= distToNext;
                        this.transform.pos = nextTilePos;
                    } else {
                        remainingSpeed = 0;
                        this.transform.pos = this.transform.pos.lerp(nextTilePos, lerpT);
                    }
                }
            }
        }
        super.draw(ctx);
    }
}