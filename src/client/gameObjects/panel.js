const gameObject = require('../../shared/gameObjects/gameObject.js');
const vector2 = require('../../shared/util/vector2.js');

module.exports = class panel extends gameObject {
    constructor(name, arg0, arg1, arg2) {
        super(name);

        if (typeof(arg0) === "string") {
            this.backgroundColor = arg0;
            if (typeof(arg1) === "string") {
                this.borderColor = arg1;
                if (typeof(arg2) === "number") {
                    this.borderSize = arg2;
                }
            }
        } else {
            this.img = arg0 || null;
        }
    }

    draw(ctx) {
        if (!this.img && !this.backgroundColor) return;
        if (this.transform.size.magnitude() === 0) return;

        if (this.img) {
            let pos = this.transform.pos;
            let size = this.transform.size;
            ctx.drawImage(this.img, pos.x, pos.y, size.x, size.y);
        } else if (this.backgroundColor) {
            let pos = this.transform.pos;
            let size = this.transform.size;
            ctx.fillStyle = this.backgroundColor;
            ctx.fillRect(pos.x, pos.y, size.x, size.y);
            if (this.borderColor) {
                ctx.strokeStyle = this.borderColor;
                ctx.strokeWeight = this.borderSize || 1;
                ctx.strokeRect(pos.x, pos.y, size.x, size.y);
            }
        }
    }
}