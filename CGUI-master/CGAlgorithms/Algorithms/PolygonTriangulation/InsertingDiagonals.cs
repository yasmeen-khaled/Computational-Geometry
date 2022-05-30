using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities;


namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class InsertingDiagonals : Algorithm
    {
        bool checked_ = false;
        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {
            if (points.Count == 0)
            {
                points = CommonMethods.get_points(lines);
            }
            insertingDiagonals(points , ref outLines);
            int dummy = -1 % 5;
        }

        int get_convex_point(List<Point>p_points)
        {
            int sz = p_points.Count;
            int i = 0;
            int prev = 0;
            int next = 0;
            for (i = 0; i < sz; i++)
            {
                prev = (i - 1 + sz) % sz;
                next = (i + 1) % sz;
                Line prev_pLine = new Line(p_points[prev], p_points[i]);
                if (HelperMethods.CheckTurn(prev_pLine, p_points[next]).Equals(Enums.TurnType.Left))
                {
                    break;
                }
            }
            return i;
        }

        int get_convex_point(List<Point> p_points , bool f)//and check ccw
        {
            if(f)
            {
                return get_convex_point(p_points);
            }
            checked_ = true;
            int sz = p_points.Count;
            int i = CommonMethods._find_min_Y_index(p_points);
            //check CCW
            int prev = (i - 1 + sz) % sz;
            int next = (i + 1) % sz;
            if(HelperMethods.CheckTurn(p_points[prev] , p_points[i] , p_points[next]) == Enums.TurnType.Right)
            {
                //call reverse
                p_points.Reverse();
                i = sz - i - 1;
            }
            return i;
        }

        void insertingDiagonals(List<Point>p_points , ref List<Line>outputDiagonals)//p_points-->polygon points
        {
            int sz = p_points.Count;
            if(sz > 3)
            {
                // --> check c is convex if not get MaxDistant Point
                int i = get_convex_point(p_points , checked_);
                Point c = p_points[i]; //assuming it is convex 
                int prev = (i - 1 + sz) % sz;
                int next = (i + 1) % sz;
                Point maxDistantPoint = new Point(0, 0);
                Line d = new Line(p_points[prev], p_points[next]);
                int pos_MaxDistsntPoint = 0;
                bool PointInTriangle = getMaxDistantPoint(ref maxDistantPoint, p_points, i ,ref pos_MaxDistsntPoint );
                if (PointInTriangle)
                {
                    d = new Line(p_points[i], maxDistantPoint);
                    prev = i;
                    //next = 
                }
                outputDiagonals.Add(d);

                //Point p1_points, p2_points;
                var p1_points = new List<Point>();
                var p2_points = new List<Point>();

                #region create two sub_diagonals
                bool first_sub = true;
                for (int j = prev; j < sz +prev; j++) {
                    int index = j % sz;

                    if(index == prev && first_sub){
                        p1_points.Add(p_points[index]);
                        //p2_points.Add(p_points[index]);
                    }
                    else if(index == next && first_sub){
                        p1_points.Add(p_points[index]);
                        p2_points.Add(p_points[index]);
                        first_sub = false;
                    }
                    else if (first_sub){
                        p1_points.Add(p_points[index]);
                    }
                    else{
                        p2_points.Add(p_points[index]);
                    }

                }
                p2_points.Add(p_points[prev]);
                #endregion

                insertingDiagonals(p1_points, ref outputDiagonals);
                insertingDiagonals(p2_points, ref outputDiagonals);

                int dummy = 1;

            }
        }

        bool getMaxDistantPoint(ref Point MaxDistsntPoint , List<Point> p_points ,int index , ref int pos_MaxDistsntPoint)
        {
            int sz = p_points.Count;
            int prev = (index - 1 + sz) % sz;
            int next = (index + 1) % sz;
            bool pointInTriangle = false;
            double max_perpendicular_distance = 0;

            for (int i = 0; i < sz; i++)
            {
                if (HelperMethods.PointInTriangle(p_points[i], p_points[index], p_points[prev], p_points[next]).Equals(Enums.PointInPolygon.Inside))
                {
                    if (i != index && i != prev && i != next)
                    {
                        pointInTriangle = true;
                        double perpendicular_distance = CommonMethods.calculate_perpendicular_distance(p_points[prev], p_points[next], p_points[i]);
                        if(perpendicular_distance > max_perpendicular_distance)
                        {
                            max_perpendicular_distance = perpendicular_distance;
                            MaxDistsntPoint = p_points[i];
                            pos_MaxDistsntPoint = i;
                        }
                        
                        // --> check farthest point
                    }
                }
            }
            return pointInTriangle;
        }
        

        public override string ToString()
        {
            return "Inserting Diagonals";
        }
    }
}
