const atob = require('atob');
const btoa = require('btoa');

module.exports = class packet {
    constructor(contents, type) {
        this.contents = contents;
        if (type !== undefined) this.contents.type = type;
    }

    get(key) {
        return this.contents[key];
    }

    toJson() {
        return JSON.stringify(this.contents);
    }

    encode() {
        return btoa(this.toJson());
    }

    static decode(message) {
        return new packet(JSON.parse(atob(message)));
    }
}

module.exports.messageType = {
    AUTH_VERSION: 0,
    0: "AUTH_VERSION",
    AUTH_LOGIN: 1,
    1: "AUTH_LOGIN"
};