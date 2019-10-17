/**
 * Represents an unordered collection of unique items of type T
 */
module.exports = class HashSet {
    constructor(...items) {
        this.container = items;
    }

    /**
     * Attempts to add an item to the collection if it does not already exist
     * @param item The item to add
     * @return boolean False if the item already exists in the collection
     */
    Add(item) {
        if (this.Contains(item)) return false;
        this.container.push(item);
        return true;
    }
    /**
     * Attempts to remove an item from the collection
     * @param item The item to remove
     * @return boolean False if the item does not exist in the collection
     */
    Remove(item) {
        if (!this.Contains(item)) return false;
        let index = this.container.indexOf(item);
        this.container.splice(index, 1);
        return true;
    }

    /**
     * @return number The number of items in the collection
     */
    get Count() {
        return this.container.length;
    }

    /**
     * Searches for a given item in the collection
     * @param item The item to search for
     * @return boolean Whether the item exists in the collection
     */
    Contains(item) {
        return this.container.indexOf(item) >= 0;
    }

    forEach(f) {
        this.container.forEach(f, thisArg);
    }
    filter(f) {
        return HashSet.FromArray(this.container.filter(f, thisArg));
    }
    map(f) {
        return HashSet.FromArray(this.container.map(f, thisArg));
    }

    /**
     * Returns the contents of the HashSet in an ordered Array
     * @return The contents of the HashSet in an ordered Array
     */
    ToArray() {
        return this.container;
    }

    /**
     * Creates a HashSet from an array of unique items
     * @param array An array to convert to a HashSet
     * @return The contents of the Array in a HashSet
     */
    static FromArray(array) {
        let set = new HashSet();
        set.container = array;
        return set;
    }
}
