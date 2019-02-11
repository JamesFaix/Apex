import { Point, Line, Polygon } from "./model";

export default class Geometry {

    //---POINT---

    public static pointTranslate(p : Point, offset : Point) : Point {
        return {
            x: p.x + offset.x,
            y: p.y + offset.y
        }
    }

    public static pointTransform(p : Point, matrix : number[][]) : Point {
        return {
            x: (p.x * matrix[0][0]) + (p.y * matrix[0][1]),
            y: (p.x * matrix[1][0]) + (p.y * matrix[1][1])
        };
    }

    public static pointToString(p : Point) : string {
        return "(" + p.x + ", " + p.y + ")";
    }

    //---LINE---

    public static lineLength(l : Line) : number {
        const dX = l.a.x - l.b.x;
        const dY = l.a.y - l.b.y;
        return Math.abs(Math.sqrt(Math.pow(dX, 2) + Math.pow(dY, 2)));
    }

    public static lineMidPoint(l : Line) : Point {
        return {
            x: (l.a.x + l.b.x) / 2,
            y: (l.a.y + l.b.y) / 2
        }
    }

    //Point a fraction of the way down a line.
    //Generalization of midpoint.
    //midpoint(L) = fractionPoint(L, 0.5)
    public static lineFractionPoint(l : Line, fraction : number) : Point {
        fraction = Math.max(0, Math.min(1, fraction)); //Limit to between 0 and 1
        const compliment = 1 - fraction;

        return {
            x: (l.a.x * compliment) + (l.b.x * fraction),
            y: (l.a.y * compliment) + (l.b.y * fraction)
        };
    }

    //---POLYGON---

    public static polygonEdges(p : Polygon) : Line[] {
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

    public static polygonContains(p : Polygon, point : Point) : boolean {
        function getSideOfLine(pt: Point, line: Line) {
            return ((pt.x - line.a.x) * (line.b.y - line.a.y))
                - ((pt.y - line.a.y) * (line.b.x - line.a.x));
        }

        let pos = 0;
        let neg = 0;

        const ls = this.polygonEdges(p);
        for (var i = 0; i < ls.length; i++) {
            const side = getSideOfLine(point, ls[i]);
            if (side < 0) { neg++; }
            else if (side > 0) { pos++; }
        }

        return pos === ls.length || neg === ls.length;
    }

    public static polygonCentroid(p : Polygon) : Point {
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

    public static polygonTranslate(p : Polygon, offset : Point) : Polygon {
        return { vertices : p.vertices.map((v : Point) => this.pointTranslate(v, offset)) };
    }

    public static polygonTransform(p : Polygon, matrix : number[][]) : Polygon {
        return { vertices : p.vertices.map((v : Point) => this.pointTransform(v, matrix)) };
    }

    public static regularPolygonRadius(numberOfSides : number, sideLength : number) : number {
        /*
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
                - sin(180/N) = (L/2)/R
                - R = L/(2 * sin(180/N))

            Convert degrees to radians for JS trig functions
        */

        return sideLength / (2 * Math.sin(Math.PI/numberOfSides));
    }

    //---TRANSFORMS---

    public static transformIdentity() : number[][] {
        return [
            [1,0],
            [0,1]
        ];
    }

    public static transformInverse() : number[][] {
        return [
            [0,1],
            [1,0]
        ];
    }

    public static transformRotation(degrees : number) : number[][] {
        const radians = degrees / 180 * Math.PI;
        const sin = Math.sin(radians);
        const cos = Math.cos(radians);

        return [
            [cos,sin],
            [-sin,cos]
        ];
    }

    public static transformScale(x : number, y : number) : number[][] {
        return [
            [x,0],
            [0,y]
        ];
    }

    public static transformCompose(a : number[][], b : number[][]) : number[][] {
        return [
            [
                (a[0][0] * b[0][0]) + (a[0][1] * b[1][0]),
                (a[0][0] * b[0][1]) + (a[0][1] * b[1][1])
            ],
            [
                (a[1][0] * b[1][1]) + (a[1][1] * b[1][0]),
                (a[1][0] * b[0][1]) + (a[1][1] * b[1][1])
            ]
        ];
    }
}