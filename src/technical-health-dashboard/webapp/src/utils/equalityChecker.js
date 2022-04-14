export const isEqual = (value, other) => {
    // Get the value type
    const type = Object.prototype.toString.call(value);

    // Check if the two objects are of the same type
    if (type !== Object.prototype.toString.call(other)) return false;

    // Check if items are an object or an array
    if (['[object Array]', '[object Object]'].indexOf(type) < 0) return false;

    // Compare the length of the two items
    const valueLength = type === '[object Array]' ? value.length : Object.keys(value).length;
    const otherLength = type === '[object Array]' ? other.length : Object.keys(other).length;
    if (valueLength !== otherLength) return false;

    // Compare two items
    const compare = (item1, item2) => {
        // Get the item type
        const itemType = Object.prototype.toString.call(item1);

        // If the item is an object or an array, compare recursively
        if (['[object Array]', '[object Object]'].indexOf(itemType) >= 0) {
            if (!isEqual(item1, item2)) return false;

        // Otherwise do a simple comparison
        } else {
            // Check if the two items are of the same type
            if (itemType !== Object.prototype.toString.call(item2)) return false;

            // If it's a function, convert to a string and compare
            if (itemType === '[object Function]') {
                if (item1.toString() !== item2.toString()) return false;

            // Otherwise, just compare
            } else {
                if (item1 !== item2) return false;
            }
        }
    };

    // Compare properties
    if (type === '[object Array]') {
        for (let i = 0; i < valueLength; i++) {
            if (compare(value[i], other[i]) === false) return false;
        }
    } else {
        for (let key in value) {
            if (value.hasOwnProperty(key)) {
                if (compare(value[key], other[key]) === false) return false;
            }
        }
    }

    return true;
}
