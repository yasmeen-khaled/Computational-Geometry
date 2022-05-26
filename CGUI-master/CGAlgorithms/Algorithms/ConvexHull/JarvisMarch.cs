using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class JarvisMarch : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            outPoints = new List<Point>();
            jarvis(points, ref outPoints);
        }

        void jarvis(List<Point> points , ref List<Point>outPoints)
        {
            Point seed = _find_min_Y_value(points);
            outPoints.Add(seed);
            int sz = points.Count();
            Point ref_point = new Point(seed.X - 1, seed.Y);
            Point current = (Point)seed.Clone();
            int hull_length = 1;
            while(true)//O(n)
            {
                Point next_point = getNextPoint(current, points, ref_point);//O(n)
                if (next_point.Equals(seed))
                    break;
                if ((HelperMethods.CheckTurn(ref_point , current , next_point).Equals(Enums.TurnType.Colinear)) && !current.Equals(seed))
                {
                    outPoints.RemoveAt(hull_length - 1);
                    hull_length--;
                }
                
                outPoints.Add(next_point);
                hull_length++;
                ref_point = (Point)current.Clone();
                current = (Point)next_point.Clone();
            }
            

            
            //outpoints.Add(next_point);
        }
        
        Point getNextPoint(Point current , List<Point>points , Point ref_point)//O(n)
        {
            int sz = points.Count();
            //Point ref_point = new Point(current.X + 1, current.Y);
            double max_angle = -1;
            Point next_point = (Point)current.Clone();
            for (int i = 0; i < sz; i++)
            {
                if (points[i].Equals(current))
                    continue;
                var angle = GetAngleBetweenPoints(ref_point, current, points[i]);
                if (angle > max_angle)
                {
                    max_angle = angle;
                    next_point = (Point)points[i].Clone();
                }
            }
            return next_point;
        }
        double GetAngleBetweenPoints(Point endPt1, Point connectingPt, Point endPt2)
        {
            double x1 = endPt1.X - connectingPt.X; //Vector 1 - x
            double y1 = endPt1.Y - connectingPt.Y; //Vector 1 - y

            double x2 = endPt2.X - connectingPt.X; //Vector 2 - x
            double y2 = endPt2.Y - connectingPt.Y; //Vector 2 - y

            double angle = Math.Atan2(y1, x1) - Math.Atan2(y2, x2);
            angle = angle * 360 / (2 * Math.PI);

            if (angle < 0)
            {
                angle += 360;
            }

            return angle;
        }
        private Point _find_min_Y_value(List<Point> points)
        {

            Point target = (Point)points[0].Clone();

            for (int i = 1; i < points.Count; i++)
            {

                if (target.Y > points[i].Y)
                {

                    target = (Point)points[i].Clone();
                }

                if (target.Y.Equals(points[i].Y))
                {
                    if (target.X < points[i].X)
                    {

                        target = (Point)points[i].Clone();

                    }

                }

            }


            return target;

        }

        public override string ToString()
        {
            return "Convex Hull - Jarvis March";
        }
    }
}
