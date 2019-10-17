exports.lerp = function(a, b, t) {
    let diff = b - a;
    return a + diff * t;
}