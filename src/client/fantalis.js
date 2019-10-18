import game from './game';

window.addEventListener("load", () => {
    window.game = new game(document.getElementById("fantalisCanvas"));
});
