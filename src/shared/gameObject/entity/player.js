const entity = require('./entity.js');

module.exports = class player extends entity {
    constructor(name, pos, size, speed, tileMap, user) {
        super(name, pos, size, speed, tileMap);

        this.user = user;
    }

    draw(ctx) {
        super.draw(ctx);

        ctx.fillStyle = 'gray';
        ctx.strokeStyle = 'black';
        ctx.beginPath();
        ctx.arc(this.transform.pos.x + 16, this.transform.pos.y + 16, 16, 0, 2 * Math.PI);
        ctx.fill();
        ctx.stroke();
    }
}