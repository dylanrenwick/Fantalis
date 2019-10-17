import panel from "./panel";
import vector2 from "../../shared/util/vector2";

export default class button extends panel {
    text: string;
    font: string;
    padding: vector2;

    highlighted: boolean;
    highlightBackground: string;
    highlightBorder: string;

    constructor(name, text) {
        super(name);

        this.text = text;
        this.font = "20px sans-serif";
        this.padding = new vector2(8, 8);

        this.highlightBackground = this.backgroundColor;
        this.highlightBorder = this.borderColor;

        this.transform.addEventListener("mouseOver", () => this.highlight());
        this.transform.addEventListener("mouseOut", () => this.dehighlight());
    }

    draw(ctx) {
        let oldSettings;
        if (this.highlighted) {
            oldSettings = [this.backgroundColor, this.borderColor];
            this.backgroundColor = this.highlightBackground;
            this.borderColor = this.highlightBorder;
        }
        super.draw(ctx);
        if (this.highlighted) {
            this.backgroundColor = oldSettings[0];
            this.borderColor = oldSettings[1];
        }

        if (!this.text) return;
        let pos = this.transform.pos;
        let size = this.transform.size;

        let centerX = pos.x + size.x / 2;
        let centerY = 10 + pos.y + size.y / 2;
        ctx.textAlign = "center";
        ctx.textBaseline = "alphabet";
        ctx.fillStyle = "black";
        ctx.font = this.font;
        ctx.fillText(this.text, centerX, centerY);
    }

    highlight() {
        this.highlighted = true;
    }
    dehighlight() {
        this.highlighted = false;
    }
}