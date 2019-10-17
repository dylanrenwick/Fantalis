const eventHandler = require("../util/eventHandler.js");
const rect = require("../util/rect.js");
const vector2 = require("../util/vector2.js");

module.exports = class transform extends eventHandler {
    constructor(pos, size, parent) {
        super([
            "mouseDown",
            "mouseUp",
            "mouseMove",,
            "mouseOver",
            "mouseOut",
            "global_mouseDown",
            "global_mouseUp",
            "global_mouseMove",
            "global_keyDown",
            "global_keyUp"
        ]);

        this.pos = pos || vector2.ZERO;
        this.size = size || vector2.ZERO;
        this.gameObject = parent || null;
    }

    getRect() {
        return new rect(this.pos, this.size);
    }

    checkMouseDown(e, caught) {
        this.triggerEvent("global_mouseDown", e);
        if (caught) return;
        if (this.size.magnitude() === 0) return false;
        if (e.clientX >= this.pos.x && e.clientY >= this.pos.y &&
            e.clientX < this.pos.x + this.size.x &&
            e.clientY < this.pos.y + this.size.y) {
            this.triggerEvent("mouseDown", e);
            return true;
        }
    }

    checkMouseUp(e, caught) {
        this.triggerEvent("global_mouseUp", e);
        if (caught) return;
        if (this.size.magnitude() === 0) return false;
        if (e.clientX >= this.pos.x && e.clientY >= this.pos.y &&
            e.clientX < this.pos.x + this.size.x &&
            e.clientY < this.pos.y + this.size.y) {
            this.triggerEvent("mouseUp", e);
            return true;
        }
    }

    checkMouseMove(e, caught) {
        let mPos = new vector2(e.clientX, e.clientY);
        let lPos = mPos.clone().sub(new vector2(e.movementX, e.movementY));
        this.triggerEvent("global_mouseMove", e);
        this.checkMouseOver(e, caught);
        if (caught) return;
        if (this.size.magnitude() === 0) return false;
        if (this.getRect().checkCollision(mPos)) {
            this.triggerEvent("mouseMove", e);
            return true;
        }
    }

    checkMouseOver(e, caught) {
        let mPos = new vector2(e.clientX, e.clientY);
        let lPos = mPos.clone().sub(new vector2(e.movementX, e.movementY));
        if (!this.getRect().checkCollision(mPos) && this.getRect().checkCollision(lPos)) {
            this.triggerEvent("mouseOut", e);
        }
        if (caught) return;
        if (this.getRect().checkCollision(mPos) && !this.getRect().checkCollision(lPos)) {
            this.triggerEvent("mouseOver", e);
        }
    }

    onKeyDown(e) {
        this.triggerEvent("global_keyDown", e);
    }
    onKeyUp(e) {
        this.triggerEvent("global_keyUp", e);
    }
}