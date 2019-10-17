const lerp = require('./lerp.js');

module.exports = class vector2 {
    constructor(x, y) {
        this.x = x || 0;
        this.y = y || 0;
    }

    add(v) {
        this.x += v.x;
        this.y += v.y;
        return this;
    }
    sub(v) {
        this.x -= v.x;
        this.y -= v.y;
        return this;
    }
    mul(v) {
        if (v instanceof vector2) {
            this.x *= v.x;
            this.y *= v.y;
        } else {
            this.x *= v;
            this.y *= v;
        }
        return this;
    }
    div(v) {
        if (v instanceof vector2) {
            this.x /= v.x;
            this.y /= v.y;
        } else {
            this.x /= v;
            this.y /= v;
        }
        return this;
    }

    distance(other) {
        return other.clone().sub(this).magnitude();
    }

    normalize() {
        let max = Math.max(Math.abs(this.x), Math.abs(this.y));
        this.x /= max;
        this.y /= max;
        return this;
    }
    normalized() {
        return this.clone().normalize();
    }
    clone() {
        return new vector2(this.x, this.y);
    }

    magnitude() {
        return Math.sqrt(this.x ** 2 + this.y ** 2);
    }
    direction() {
        return Math.atan(this.x / this.y) * (180 / Math.PI);
    }

    lerp(other, t) {
        return new vector2(lerp.lerp(this.x, other.x, t), lerp.lerp(this.y, other.y, t));
    }
}

module.exports.ZERO = new module.exports();
module.exports.ONE = new module.exports(1, 1);