import { PieceKind } from "../api/model";
import { memoize } from "./functions";

function getPieceImagePath(kind : PieceKind) : string {
    switch (kind) {
        case PieceKind.Assassin: return "../../resources/daggerEmoji.png";
        case PieceKind.Chief: return "../../resources/crownEmoji.png";
        case PieceKind.Corpse: return "../../resources/skullEmoji.png";
        case PieceKind.Diplomat: return "../../resources/doveEmoji.png";
        case PieceKind.Gravedigger: return "../../resources/pickEmoji.png";
        case PieceKind.Reporter: return "../../resources/newspaperEmoji.png";
        case PieceKind.Thug: return "../../resources/fistEmoji.png";
        default: throw "Invalid piece kind.";
    }
}

function createPieceImage(kind : PieceKind) : HTMLImageElement {
    const image = new (window as any).Image() as HTMLImageElement;
    image.src = getPieceImagePath(kind);
    return image;
}

export const getPieceImage = memoize(createPieceImage);