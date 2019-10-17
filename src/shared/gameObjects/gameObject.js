const transform = require('./transform.js');

module.exports = class gameObject {
    constructor(name, pos, size) {
        this.name = name;
        this.transform = new transform(pos, size, this);
    }

    toJson() {
        return JSON.stringify({
            name: this.name
        });
    }

    draw(ctx) {}
}