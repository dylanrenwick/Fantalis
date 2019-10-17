const vector2 = require('./vector2.js');

module.exports = class rect {
    constructor() {
        let args = Array.from(arguments);
        let i = 0;
        if (args[i] instanceof vector2) {
            this.pos = args[i];
            i++;
        } else if (typeof(args[i]) === "number" && typeof(args[i+1]) === "number") {
            this.pos = new vector2(args[i], args[i+1]);
            i += 2;
        }
        
        if (args[i] instanceof vector2) {
            this.size = args[i];
            i++;
        } else if (typeof(args[i]) === "number" && typeof(args[i+1]) === "number") {
            this.size = new vector2(args[i], args[i+1]);
            i += 2;
        }
    }

    checkCollision(point) {
        return point.x >= this.pos.x && point.y >= this.pos.y
            && point.x < this.pos.x + this.size.x
            && point.y < this.pos.y + this.size.y;
    }
}