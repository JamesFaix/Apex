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
        /*
            Regarding Apothem and Radius

            A regular polygon P with N sides of length L can be divided radially into N isocolese triangles.
            Given one of these triangles T,
                - The base of T is L
                - The side of T is the radius (R) of P
                - The height of T is the apothem (A) of P
                - The "top" angle of T is 360/N degrees
                - The base angles are (180 - (360/N))/2 = 90-(180/N) degrees

            T can be split vertically into two right triangles, so trig functions can be used.
            Given one of these two triangles U,
                - The base of U is L/2
                - The longer side of U is the R, and the shorter is A
                - Angles are 90, 180/N, and 90-(180/N) degrees
                - sin(180/N) = (L/2)/R   =>   R = L/(2 * sin(180/N))
                - cos(180/N) = (L/2)/A   =>   A = L/(2 * cos(180/N))

            Convert degrees to radians for JS trig functions

            https://www.mathopenref.com/polygonradius.html
        */

        public static apothem(numberOfSides : number, sideLength : number) : number {
            //TODO: Precompute values for numberOfSides = 1
            return sideLength / (2 * Math.cos(Math.PI/numberOfSides));
        }

        public static radius(numberOfSides : number, sideLength : number) : number {
            //TODO: Precompute values for numberOfSides = 1
            return sideLength / (2 * Math.sin(Math.PI/numberOfSides));
        }

        /*
            Regarding Height, Width, and Centroid

            These methods assume the polygon is oriented such that:
                - Directions are orinted in "canvas space" where (1,1) is is bottom-right quadrant
                - The bottom-most point(s) are coincident with with x-axis
                - The left-most point(s) are coincident with the y-axis
         */

        public static height(numberOfSides : number, sideLength : number) : number {
            function getUnitHeight(nSides : number) {
                //TODO: Calculate these to more significant digits

                /*
                    For odd nSides, this will always be (radius + apothem)
                    For even nSides, this will always be (2 x apothem)
                 */
                switch (nSides) {
                    case 3: return 0.87;
                    case 4: return 1.00;
                    case 5: return 1.54;
                    case 6: return 1.73;
                    case 7: return 2.19;
                    case 8: return 2.41;
                    default: throw "Unsupported number of sides: " + nSides;
                }
            }

            return getUnitHeight(numberOfSides) * sideLength;
        }

        public static width(numberOfSides : number, sideLength : number) : number {
            //TODO: Calculate these to more significant digits

            /*
                When nSides is divisible by 4, this will be (2x apothem)
                When nSides is even but not divisble by 4, this will be (2x radius)
                When nSides is 3, this will be (sidelength)
                When nSides is odd and not 3, the solution requires a complex
                geometric construction that is slightly different for each case
            */
            function getUnitWidth(nSides : number) {
                switch (nSides) {
                    case 3: return 1.00;
                    case 4: return 1.00;
                    case 5: return 1.62;
                    case 6: return 2.00;
                    case 7: return 2.25;
                    case 8: return 2.41;
                    default: throw "Unsupported number of sides: " + nSides;
                }
            }

            return getUnitWidth(numberOfSides) * sideLength;
        }

        public static centroid(numberOfSides : number, sidelength : number) : Point {
            //TODO: Precompute values for numberOfSides = 1

            const x = this.width(numberOfSides, sidelength) / 2;
            const y = numberOfSides % 2 === 0
                ? this.apothem(numberOfSides, sidelength)
                : this.radius(numberOfSides, sidelength);

            return Geometry.Point.multiplyScalar({ x: x, y: y }, sidelength);
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