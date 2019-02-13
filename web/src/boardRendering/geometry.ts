import { Point, Line, Polygon, CellView, BoardView } from "./model";
import * as MathJs from 'mathjs';

export default class Geometry {

    public static Point = class {
        public static addScalar(p : Point, n : number) : Point {
            return {
                x: p.x + n,
                y: p.y + n
            };
        }

        public static multiplyScalar(p : Point, n : number) : Point {
            return {
                x: p.x * n,
                y: p.y * n
            };
        }

        public static transform(p : Point, matrix : MathJs.Matrix) : Point {
            const pointVector = MathJs.matrix([p.x, p.y, 1]);
            const resultMatrix = MathJs.multiply(matrix, pointVector);
            const resultArray = (resultMatrix as any)._data as number[]; //Breaking MathJs's encapsulation here for efficiency
            return {
                x: resultArray[0] / resultArray[2],
                y: resultArray[1] / resultArray[2]
            };
        }

        public static translate(p : Point, offset : Point) : Point {
            return {
                x: p.x + offset.x,
                y: p.y + offset.y
            }
        }
    }

    public static Line = class {
        //Point a fraction of the way down a line.
        //Generalization of midpoint.
        //midpoint(L) = fractionPoint(L, 0.5)
        public static fractionPoint(l : Line, fraction : number) : Point {
            fraction = Math.max(0, Math.min(1, fraction)); //Limit to between 0 and 1
            const compliment = 1 - fraction;

            return {
                x: (l.a.x * compliment) + (l.b.x * fraction),
                y: (l.a.y * compliment) + (l.b.y * fraction)
            };
        }

        public static len(l : Line) : number { //TS compiler won't let you use `length` because of `Function.length`
            const dX = l.a.x - l.b.x;
            const dY = l.a.y - l.b.y;
            return Math.abs(Math.sqrt(Math.pow(dX, 2) + Math.pow(dY, 2)));
        }

        public static midPoint(l : Line) : Point {
            return {
                x: (l.a.x + l.b.x) / 2,
                y: (l.a.y + l.b.y) / 2
            }
        }
    }

    public static Polygon = class {
        public static centroid(p : Polygon) : Point {
            let sumX = 0;
            let sumY = 0;

            const count = p.vertices.length;

            for (var i = 0; i < count; i++) {
                const c = p.vertices[i];
                sumX += c.x;
                sumY += c.y;
            }

            return {
                x: sumX / count,
                y: sumY / count
            };
        }

        public static edges(p : Polygon) : Line[] {
            const verticesOffset = p.vertices.slice(1);
            verticesOffset.push(p.vertices[0]);

            const result = [];
            for (var i = 0; i < p.vertices.length; i++) {
                const line = {
                    a: p.vertices[i],
                    b: verticesOffset[i]
                };
                result.push(line);
            }
            return result;
        }

        public static height(p : Polygon) : number {
            return this.size(p, v => v.y);
        }

        private static size(p : Polygon, projection : (p : Point) => number) : number {
            const ys = p.vertices.map(v => projection(v));
            const min = Math.min(...ys);
            const max = Math.max(...ys);
            return Math.abs(max - min);
        }

        public static transform(p : Polygon, matrix : MathJs.Matrix) : Polygon {
            return { vertices : p.vertices.map((v : Point) => Geometry.Point.transform(v, matrix)) };
        }

        public static width(p : Polygon) : number {
            return this.size(p, v => v.x);
        }
    }

    public static RegularPolygon = class {
        //Values are pre-calculated by the Polygon Data Generator utility

        public static apothem(numberOfSides : number, sideLength : number) : number {
            function getBaseValue(nSides : number) {
                switch (nSides) {
                    case 3: return 0.2886751;
                    case 4: return 0.5000000;
                    case 5: return 0.6881910;
                    case 6: return 0.8660254;
                    case 7: return 1.0382607;
                    case 8: return 1.2071068;
                    default: throw "Unsupported number of sides: " + nSides;
                }
            }
            return getBaseValue(numberOfSides) * sideLength;
        }

        public static radius(numberOfSides : number, sideLength : number) : number {
            function getBaseValue(nSides : number) {
                switch (nSides) {
                    case 3: return 0.5773503;
                    case 4: return 0.7071068;
                    case 5: return 0.8506508;
                    case 6: return 1.0000000;
                    case 7: return 1.1523824;
                    case 8: return 1.3065630;
                    default: throw "Unsupported number of sides: " + nSides;
                }
            }
            return getBaseValue(numberOfSides) * sideLength;
        }

        /*
            Regarding Height, Width, and Centroid

            These methods assume the polygon is oriented such that:
                - Directions are orinted in "canvas space" where (1,1) is is bottom-right quadrant
                - The bottom-most point(s) are coincident with with x-axis
                - The left-most point(s) are coincident with the y-axis
         */

        public static height(numberOfSides : number, sideLength : number) : number {
            function getBaseValue(nSides : number) {
                switch (nSides) {
                    case 3: return 0.8660254;
                    case 4: return 1.0000000;
                    case 5: return 1.5388418;
                    case 6: return 1.7320508;
                    case 7: return 2.1906431;
                    case 8: return 2.4142136;
                    default: throw "Unsupported number of sides: " + nSides;
                }
            }
            return getBaseValue(numberOfSides) * sideLength;
        }

        public static width(numberOfSides : number, sideLength : number) : number {
            function getBaseValue(nSides : number) {
                switch (nSides) {
                    case 3: return 1.0000000;
                    case 4: return 1.0000000;
                    case 5: return 1.5388418;
                    case 6: return 2.0000000;
                    case 7: return 2.1906431;
                    case 8: return 2.4142136;
                    default: throw "Unsupported number of sides: " + nSides;
                }
            }
            return getBaseValue(numberOfSides) * sideLength;
        }

        public static centroid(numberOfSides : number, sidelength : number) : Point {
            function getBaseXValue(nSides : number) {
                switch (nSides) {
                    case 3: return 0.5000000;
                    case 4: return 0.7071068;
                    case 5: return 0.8090170;
                    case 6: return 0.8660254;
                    case 7: return 1.1234898;
                    case 8: return 1.3065630;
                    default: throw "Unsupported number of sides: " + nSides;
                }
            }

            function getBaseYValue(nSides : number) {
                switch (nSides) {
                    case 3: return 0.5773503;
                    case 4: return 0.7071068;
                    case 5: return 0.8506508;
                    case 6: return 1.0000000;
                    case 7: return 1.1523824;
                    case 8: return 1.3065630;
                    default: throw "Unsupported number of sides: " + nSides;
                }
            }

            return {
                x: getBaseXValue(numberOfSides) * sidelength,
                y: getBaseYValue(numberOfSides) * sidelength
            };
        }
    }

    public static Transform = class {
        //https://www.mathworks.com/help/images/matrix-representation-of-geometric-transformations.html

        public static compose(transforms : MathJs.Matrix[]) : MathJs.Matrix {
            switch (transforms.length) {
                case 0:
                    return this.identity();

                case 1:
                    return transforms[0];

                default:
                    let t = transforms[0];
                    for (var i=1; i<transforms.length; i++) {
                        t = MathJs.multiply(t, transforms[i]) as MathJs.Matrix;
                    }
                    return t;
            }
        }

        public static identity() : MathJs.Matrix {
            return MathJs.matrix([
                [1, 0, 0],
                [0, 1, 0],
                [0, 0, 1]
            ]);
        }

        public static rotation(degrees : number) : MathJs.Matrix {
            const radians = degrees / 180 * Math.PI;
            const sin = Math.sin(radians);
            const cos = Math.cos(radians);

            return MathJs.matrix([
                [ cos, sin, 0],
                [-sin, cos, 0],
                [   0,   0, 1]
            ]);
        }

        public static scale(x : number, y : number) : MathJs.Matrix {
            return MathJs.matrix([
                [x, 0, 0],
                [0, y, 0],
                [0, 0, 1]
            ]);
        }

        public static translate(x : number, y : number) : MathJs.Matrix {
            return MathJs.matrix([
                [1, 0, x],
                [0, 1, y],
                [0, 0, 1]
            ]);
        }
    }

    public static Cell = class {
        public static centroid(c : CellView) : Point {
            let sumX = 0;
            let sumY = 0;
            let n = c.polygons.length;

            for (var i = 0; i < n; i++){
                let p = Geometry.Polygon.centroid(c.polygons[i]);
                sumX += p.x;
                sumY += p.y;
            }

            return { x: sumX/n, y: sumY/n };
        }

        public static transform(c: CellView, matrix : MathJs.Matrix) : CellView {
            return {
                id: c.id,
                type: c.type,
                state: c.state,
                piece: c.piece,
                polygons: c.polygons.map(p => Geometry.Polygon.transform(p, matrix))
            };
        }
    }

    public static Board = class {
        public static size(b : BoardView) : Point {
            const h = Geometry.Polygon.height(b.polygon);
            const w = Geometry.Polygon.width(b.polygon);
            return { x: w, y: h };
        }

        public static transform(b : BoardView, matrix : MathJs.Matrix) : BoardView {
            return {
                regionCount: b.regionCount,
                cellCountPerSide: b.cellCountPerSide,
                polygon: Geometry.Polygon.transform(b.polygon, matrix),
                cells: b.cells.map(c => Geometry.Cell.transform(c, matrix))
            };
        }
    }
}