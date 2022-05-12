using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class QuickHull : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {

            //List<Point> tmp = new List<Point>();
            //quickHull(points , ref tmp);
            quickHull(points, ref outPoints);
        }

        private void quickHull(List<Point> points ,ref List<Point> outpoints)
        {
            //[1] get min_x & max_x
            Point mini = (Point)points[0].Clone();
            Point mx = (Point)points[0].Clone();
            CommonMethods._find_min_max_X_value(points, ref mini, ref mx);

            

            //[2] get upper points list & lower points list
            List<Point> upper_points_list = get_left_points(mini, mx, points);
            List<Point> lower_points_list = get_left_points(mx, mini, points);

            //[3] quickHullRec
            upper_points_list = quickHullRec(upper_points_list, mini, mx);
            lower_points_list = quickHullRec(lower_points_list, mx, mini);

            //[4] Convex Hull list
            outpoints.AddRange(upper_points_list);
            outpoints.AddRange(lower_points_list);
            outpoints.Add(mx);
            if(!mx.Equals(mini))
                outpoints.Add(mini);

            

        }

        private List<Point> quickHullRec(List<Point> points , Point start , Point end)
        {
            if(points.Count == 0)
            {
                return new List<Point>();
            }
            //[1] get max distance point (Pmax) 
            Point Pmax = get_max_distance_point_from_line(start , end, points);

            //[2] get outer points from left and right of Pmax
            List<Point> left_points_list = get_left_points(start, Pmax, points);
            List<Point> right_points_list = get_left_points(Pmax, end, points);

            //[3] Recurse
            List<Point> R1 = quickHullRec(left_points_list, start, Pmax);
            List<Point> R2 = quickHullRec(right_points_list, Pmax, end);

            //concatenate 
            var conc_list = new List<Point>(); 
            R1.AddRange(R2);
            R1.Add(Pmax);
            return R1;
        }

        private Point get_max_distance_point_from_line(Point start , Point end , List<Point>points)
        {
            Point Pmax = new Point(0,0);
            double max_distance = 0;
            int sz = points.Count;
            for (int i = 0; i < sz; i++)
            {
                double distance = CommonMethods.calculate_perpendicular_distance(start, end, points[i]);
                if (distance > max_distance)
                {
                    max_distance = distance;
                    Pmax = points[i];
                }
            }
            return Pmax;
        }
        private List<Point> get_left_points(Point a , Point b , List<Point>points)
        {
            List<Point> left_points = new List<Point>();
            int sz = points.Count;
            for (int i = 0; i < sz; i++)
            {
                if(a.Equals(points[i]) || b.Equals(points[i]))
                {
                    continue;
                }
                Point v1 = a.Vector(b);
                Point v2 = a.Vector(points[i]);
                //var s = HelperMethods.CheckTurn(v1, v2);
                if (HelperMethods.CheckTurn(v1, v2).Equals(Enums.TurnType.Left))
                {
                    left_points.Add(points[i]);

                }

            }
            int dummy = 2;
            return left_points;
        }
        

        private List<Point> remove_duplicates(List<Point> points) 
        {
            List<Point> outpoints = new List<Point>();
            int sz = points.Count;
            for (int i = 0; i < sz; i++)//
            {
                bool duplicated = false;
                for (int j = i + 1; j < sz; j++)
                {
                    if (points[i].Equals(points[j]))
                    {
                        duplicated = true;
                    }
                }
                if (!duplicated)
                {
                    outpoints.Add(points[i]);
                }
            }

            return outpoints;
        }

        public override string ToString()
        {
            return "Convex Hull - Quick Hull";
        }
    }
}
