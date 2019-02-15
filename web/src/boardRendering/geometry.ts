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
        private static readonly sideLength = 1;

        private static isDivisibleBy(divisor : number, dividend : number) : boolean {
            return dividend % divisor === 0;
        }

        private static isEven(n : number) : boolean {
            return this.isDivisibleBy(2, n);
        }

        private static internalAngle(numberOfSides : number) : number {
            //Divide a circle into n slices
            return 2 * Math.PI / numberOfSides;
        }

        private static externalAngle(numberOfSides : number) : number {
            /*
                Internal angle forms a "slice" with the internal angle and 2 other equal angles.
                Those two outer angles of the slice are each 1/2 of the external angle of the polygon.
                The sum of the angles of a triangle is PI radians.
                So, PI minus the internal angle is the sum of the other 2 angles of the "slice".
            */
            return Math.PI - this.internalAngle(numberOfSides);
        }

        public static sideToRadiusRatio(numberOfSides : number) : number {
            /*
                Radius connects center and a vertex.

                Start with a "slice" triangle, as described in the externalAngle derivation.
                One side of the "slice" is an edge of the polygon, the other two are radii.
                The "slice" can be divided in half by adding a line from the center of the polygon
                to the mid-point of the edge. (This line is an apothem.)

                This "half-slice" is a right triangle. Its sides are one radius, one apothem,
                and a half-edge. The angles that are PI/2, internalAngle/2 and externalAngle/2 radians.

                Thus:
                    1) sin(internalAngle/2) = (edge/2)/radius
                    2) cos(internalAngle/2) = apothem/radius
                    3) tan(internalAngle/2) = (edge/2)/apothem
                    4) sin(externalAngle/2) = apothem/radius
                    5) cos(externalAngle/2) = (edge/2)/radius
                    6) tan(externalAngle/2) = apothem/(edge/2)

                From 1),
                    sin(internalAngle/2) = (edge/2)/radius
                    radius * sin(internalAngle/2) = edge/2
                    radius = (edge/2)/sin(internalAngle/2)
            */
           return (this.sideLength/2) / Math.sin(this.internalAngle(numberOfSides)/2);
        }

        public static sideToApothemRatio(numberOfSides : number) : number {
            /*
                Apothem connects center and the mid-point of an edge.

                From 3) in the derivation of sideToRadiusRatio,
                    tan(internalAngle/2) = (edge/2)/apothem
                    apothem * tan(internalAngle/2) = edge/2
                    apothem = (edge/2)/tan(internalAngle/2)
            */
            return (this.sideLength/2) / Math.tan(this.internalAngle(numberOfSides)/2);
        }

        public static create(numberOfSides : number, sideLength : number) : Polygon {
            /*
                Creates a regular n-gon with the given side length.
                It's center will be (0,0).
                The bottom side will be parallel to, and below, the x-axis.
            */
            const radius = this.sideToRadiusRatio(numberOfSides) * sideLength;
            const internalAngle = this.internalAngle(numberOfSides);

            const vertices : Point[] = [];
            let angle = internalAngle/2;
            for (var i=1; i<=numberOfSides; i++) {
                const p = {
                    x: radius * Math.sin(angle),
                    y: radius * Math.cos(angle)
                };
                vertices.push(p);
                angle += internalAngle;
            }
            return { vertices: vertices };
        }

        public static sideToHeightRatio(numberOfSides : number) : number {
            /*
                This function assumes the polygon is positioned with at least 1 edge parallel to the x-axis.
                For even n, there will be two edges parallel to the x-axis, for odd n just one.
                Thus, for even n the height is (2 * apothem), and for odd n it is (apothem + radius).
            */
            const a = this.sideToApothemRatio(numberOfSides);

            return this.isEven(numberOfSides)
                ? a * 2
                : a + this.sideToRadiusRatio(numberOfSides);
        }

        public static sideToWidthRatio(numberOfSides : number) : number {
            /*
                This function assumes the polygon is positioned with at least 1 edge parallel to the x-axis.
                Width is a lot harder to come up with a general formula for than the previous functions.
            */
            if (numberOfSides === 3 || numberOfSides === 4) {
                //The width of an equilateral triangle or a square is obviously the length of its side.
                return 1;
            } else if (this.isDivisibleBy(4, numberOfSides)){
                //For squares, octogons, 12-gons, 16-gons, etc, the width will always equal the height.
                return this.sideToApothemRatio(numberOfSides) * 2;
            } else if (this.isEven(numberOfSides)) {
                //For hexagons, 10-gons, 14-gons, etc, the width will always be 2 * radius.
                return this.sideToRadiusRatio(numberOfSides) * 2;
            } else {
                //For all other odd n-gons, I do not know of a simple formula.
                //The next best heuristic is to generate an n-gon, and then check the min and max X values of its vertices.
                const polygon = this.create(numberOfSides, 1);
                const ys = polygon.vertices.map(p => p.y);
                const maxY = Math.max(...ys);
                const minY = Math.min(...ys);
                return Math.abs(maxY - minY);
            }
        }

        public static sideToCentroidDistanceFromTopRatio(numberOfSides : number) : number {
            /*
                This function assumes the polygon is positioned with at least 1 edge parallel to the x-axis.

                Since the polygon's bottom is parallel to the x-axis,
                    for even n, the top will also be parallel and there will be an apothem connecting the center to the top, and
                    for odd n, the top will be a vertex and there will be a radius connecting the center to the top.
            */
            return this.isEven(numberOfSides)
                ? this.sideToApothemRatio(numberOfSides)
                : this.sideToRadiusRatio(numberOfSides);
        }

        public static sideToCentroidDistanceFromLeftRatio(numberOfSides : number) : number {
            /*
                This function assumes the polygon is positioned with at least 1 edge parallel to the x-axis.

                Since the polygon's bottom is parallel to the x-axis, it is always symmetric across the y-axis.
                Thus, (width/2) can be used.
            */
            return this.sideToWidthRatio(numberOfSides) / 2;
        }

        public static largestScaleWithinBox(numberOfSides : number, boxSize : Point) : number {
            const widthRatio = this.sideToWidthRatio(numberOfSides);
            const heightRatio = this.sideToHeightRatio(numberOfSides);
            const maxXScale = boxSize.x / widthRatio;
            const maxYScale = boxSize.y / heightRatio;
            return Math.min(maxXScale, maxYScale);
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

        public static flipHorizontal() : MathJs.Matrix {
            return MathJs.matrix([
                [-1, 0, 0],
                [ 0, 1, 0],
                [ 0, 0, 1]
            ])
        }

        public static flipVertical() : MathJs.Matrix {
            return MathJs.matrix([
                [1,  0, 0],
                [0, -1, 0],
                [0,  0, 1]
            ])
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