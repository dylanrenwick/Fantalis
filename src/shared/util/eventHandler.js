module.exports = class eventHandler {
    constructor(eventNames) {
        this.eventListeners = {};
        this.tempEventListeners = {};
        eventNames.forEach(name => {
            this.eventListeners[name] = [];
            this.tempEventListeners[name] = [];
        });
    }

    addEventListener(name, pred) {
        if (this.eventListeners[name] === undefined) return;
        this.eventListeners[name].push(pred);
    }

    clearEventListeners(name) {
        if (this.eventListeners[name] === undefined) return;
        this.eventListeners[name] = [];
    }
    clearAllEventListeners() {
        for (let name in this.eventListeners) {
            if (!this.eventListeners.hasOwnProperty(name)) continue;
            this.eventListeners[name] = [];
        }
    }

    addTemporaryEventListener(name, pred) {
        if (this.tempEventListeners[name] === undefined) return;
        this.tempEventListeners[name].push(pred);
    }

    clearTemporaryEventListeners(name) {
        if (this.tempEventListeners[name] === undefined) return;
        this.tempEventListeners[name] = [];
    }

    triggerEvent(name) {
        if (this.eventListeners[name] === undefined && this.tempEventListeners[name] === undefined) return;
        let args = Array.from(arguments);
        args.splice(0, 1);
        this.eventListeners[name].forEach(x => x(...args));

        let done = this.tempEventListeners[name].filter(x => x(...args) === true);
        done.forEach(x => this.tempEventListeners[name].splice(this.tempEventListeners[name].indexOf(x), 1));
    }
}