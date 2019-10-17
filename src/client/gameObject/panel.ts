import gameObject from '../../shared/gameObject/gameObject';

export default class panel extends gameObject {
    backgroundColor: string;
    borderColor: string;
    borderSize: number;

    img: HTMLImageElement;

    constructor(name, img?) {
        super(name);

        this.img = img || null;
    }

    draw(ctx) {
        if (!this.img && !this.backgroundColor) return;
        if (this.transform.size.magnitude() === 0) return;

        if (this.img) {
            let pos = this.transform.pos;
            let size = this.transform.size;
            ctx.drawImage(this.img, pos.x, pos.y, size.x, size.y);
        } else if (this.backgroundColor) {
            let pos = this.transform.pos;
            let size = this.transform.size;
            ctx.fillStyle = this.backgroundColor;
            ctx.fillRect(pos.x, pos.y, size.x, size.y);
            if (this.borderColor) {
                ctx.strokeStyle = this.borderColor;
                ctx.strokeWeight = this.borderSize || 1;
                ctx.strokeRect(pos.x, pos.y, size.x, size.y);
            }
        }
    }
}