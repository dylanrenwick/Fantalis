const vector2 = require('../util/vector2.js');

class pathNode {
    constructor(parent, x, y) {
        this.parent = parent;
        this.x = x;
        this.y = y;
    }

    getGCost(origin) {
        let prev = this;
        let next = this.parent;
        let total = 0;

        while (next !== undefined && (next.x !== origin.x || next.y !== origin.y)) {
            total += new vector2(prev.x, prev.y).distance(new vector2(next.x, next.y));
            prev = next;
            next = next.parent;
        }

        return total;
    }
    getHCost(goal) {
        return new vector2(this.x, this.y).distance(new vector2(goal.x, goal.y));
    }
    getFCost(origin, goal) {
        return this.getGCost(origin) + this.getHCost(goal);
    }

    getNeighbours(map) {
        return map.getNeighbours(this.x, this.y).filter(x => x !== null).map(x => 
            pathNode.fromTilePos(new vector2(x.x, x.y), this)
        );
    }

    getFullPath() {
        let path = [this];
        let next = this.parent;
        while (next.parent !== undefined) {
            path.push(next);
            next = next.parent;
        }
        return path;
    }

    static fromTilePos(pos, prev) {
        return new pathNode(prev, pos.x, pos.y);
    }
}

function plotAStarPath(from, to, map) {
    // A* impl
    let open = [pathNode.fromTilePos(from, undefined)];
    let closed = [];
    let current;

    while (true) {
        current = open.sort((a, b) => a.getFCost(from, to) - b.getFCost(from, to))[0];
        open.splice(open.indexOf(current), 1);
        closed.push(current);

        if (current.x === to.x && current.y === to.y) break;

        let neighbours = current.getNeighbours(map);
        for (let n of neighbours) {
            if (map.isBlocked(n.x, n.y) || closed.includes(n)) continue;

            let updateNeighbour = false;
            if (!open.includes(n)) updateNeighbour = true;
            else {
                let oldParent = n.parent;
                let oldGCost = n.getGCost(from);
                n.parent = current;
                let newGCost = n.getGCost(from);
                n.parent = oldParent;
                if (newGCost > oldGCost) updateNeighbour = true;
            }

            if (updateNeighbour) {
                n.parent = current;
                if (!open.includes(n))
                open.push(n);
            }
        }
    }

    return current.getFullPath();
}

module.exports = plotAStarPath;
