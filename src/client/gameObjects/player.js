const gameObject = require('../../shared/gameObjects/gameObject.js');

module.exports = class player extends gameObject {
    constructor(name) {
        super(name);
    }
}