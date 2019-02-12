import { PieceKind } from "../api/model";

export interface Point {
    x : number,
    y : number
}

export interface Line {
    a : Point,
    b : Point
}

export interface Polygon {
    vertices : Point[]
}

export enum CellState {
    Default,
    Selected,
    Selectable
}

export enum CellType {
    Even,
    Odd,
    Center
}

export interface CellHighlight {
    color : string,
    intensity : number
}

export interface PieceView {
    kind : PieceKind,
    colorId : number
}

export interface CellView {
    id : number,
    type : CellType,
    state : CellState,
    piece : PieceView,
    polygons : Polygon[]
}

export interface BoardView {
    regionCount : number,
    cellCountPerSide : number,
    polygon : Polygon,
    cells : CellView[]
}