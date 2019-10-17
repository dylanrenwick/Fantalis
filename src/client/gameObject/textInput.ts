import panel from "./panel";
import vector2 from "../../shared/util/vector2";

export default class textInput extends panel {
    focused: boolean;

    value: string;
    font: string;
    placeholder: string;

    padding: vector2;
    caretPos: number;

    censored: boolean;
    censorChar: string;

    constructor(name, img?) {
        super(name, img);

        this.focused = false;
        this.value = "";
        this.padding = new vector2(8, 8);
        this.font = "22px sans-serif";
        this.placeholder = "";
        this.censored = false;
        this.censorChar = '*';
        this.transform.addEventListener("mouseDown", () => this.focus());
        this.transform.addEventListener("global_mouseDown", () => this.defocus());
        this.transform.addEventListener("global_keyDown", (e) => this.onKeyDown(e));
    }

    draw(ctx) {
        super.draw(ctx);

        ctx.fillStyle = "white";
        ctx.strokeStyle = "black";

        let pos = this.transform.pos;
        let size = this.transform.size;
        ctx.fillRect(pos.x, pos.y, size.x, size.y);
        ctx.strokeRect(pos.x, pos.y, size.x, size.y);

        if (this.focused) {
            ctx.strokeStyle = "#ABC4FE";
            ctx.strokeRect(pos.x + 1, pos.y + 1, size.x - 2, size.y - 2);
            ctx.strokeRect(pos.x + 2, pos.y + 2, size.x - 4, size.y - 4);
        }

        ctx.font = this.font;
        ctx.textAlign = "left";
        let maxWidth = size.x - this.padding.x * 2;
        let renderText = this.value
        if (this.censored) {
            renderText = this.censorChar.repeat(this.value.length);
        }
        let textMetrics = ctx.measureText(renderText);
        while (textMetrics.width > maxWidth) {
            renderText = renderText.substring(1);
            textMetrics = ctx.measureText(renderText);
        }
        if (this.value.length > 0 || this.focused) {
            ctx.fillStyle = "black";
            ctx.textBaseline = "bottom";
            ctx.fillText(renderText, pos.x + this.padding.x, pos.y + size.y - this.padding.y, maxWidth);
        } else {
            ctx.fillStyle = "grey";
            ctx.textBaseline = "bottom";
            ctx.fillText(this.placeholder, pos.x + this.padding.x, pos.y + size.y - this.padding.y, maxWidth);
        }
        if (this.focused) {
            let caretSize = ctx.measureText(renderText.substring(0, this.caretPos));
            ctx.strokeStyle = "black";
            ctx.beginPath();
            ctx.moveTo(pos.x + this.padding.x + caretSize.width, pos.y + size.y - this.padding.y);
            ctx.lineTo(pos.x + this.padding.x + caretSize.width, pos.y + this.padding.y);
            ctx.stroke();
        }
    }

    focus() {
        this.focused = true;
        this.caretPos = this.value.length;
    }
    defocus() {
        this.focused = false;
    }

    onKeyDown(e) {
        if (!this.focused) return;
        if (e.key.toString().length === 1) {
            this.value += e.shiftKey ? e.key.toUpperCase() : e.key;
            this.caretPos++;
        } else if (e.key.toLowerCase() === "backspace") {
            this.value = this.value.substring(0, this.value.length - 1);
            this.caretPos--;
        } else if (e.key.toLowerCase() === "delete") {
            if (this.caretPos < this.value.length) {
                let valArr = this.value.split('');
                valArr.splice(this.caretPos, 1);
                this.value = valArr.join('');
            }
        } else if (e.key.toLowerCase() === "escape") this.focused = false;
        else if (e.key.toLowerCase() === "arrowleft") this.caretPos = Math.max(0, this.caretPos - 1);
        else if (e.key.toLowerCase() === "arrowright") this.caretPos = Math.min(this.value.length, this.caretPos + 1);
        else if (e.key.toLowerCase() === "end") this.caretPos = this.value.length;
        else if (e.key.toLowerCase() === "home") this.caretPos = 0;
    }
}