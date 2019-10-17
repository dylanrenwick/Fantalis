const game = require("./game.js");

window.addEventListener("load", () => {
    window.game = new game(document.getElementById("fantalisCanvas"));
});
