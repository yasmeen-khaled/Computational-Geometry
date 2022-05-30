using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGUtilities
{
    public static class CommonMethods
    {
        public static List<Point> get_points(List<Line> lines)
        {
            List<Point> points = new List<Point>();
            int sz = lines.Count;
            for (int i = 0; i < sz; i++)
            {
                points.Add(lines[i].Start);
            }

            return points;
        }
        public static Point _find_min_max_X_value(List<Point> points, ref Point mini, ref Point mx)//get  upper most X
        {




            for (int i = 1; i < points.Count; i++)
            {

                if (mini.X > points[i].X)
                {

                    mini = (Point)points[i].Clone();
                }
                else if (mx.X < points[i].X)
                {

                    mx = (Point)points[i].Clone();
                }
                //
                if (mini.X.Equals(points[i].X))
                {
                    if (mini.Y < points[i].Y)
                    {

                        mini = (Point)points[i].Clone();

                    }

                }
                else if (mx.X.Equals(points[i].X))
                {
                    if (mx.Y < points[i].Y)//get  upper most mx X
                    {

                        mx = (Point)points[i].Clone();

                    }

                }

            }


            return mini;

        }

        public static Point _find_min_max_X_value(List<Point> points , bool get_min ,ref int index)//get  upper most X
        {

            Point mini = (Point)points[0].Clone();
            Point mx = (Point)points[0].Clone();


            for (int i = 1; i < points.Count; i++)
            {

                if (mini.X > points[i].X)
                {

                    mini = (Point)points[i].Clone();
                }
                else if (mx.X < points[i].X)
                {

                    mx = (Point)points[i].Clone();
                }
                //
                if (mini.X.Equals(points[i].X))
                {
                    if (mini.Y < points[i].Y)
                    {

                        mini = (Point)points[i].Clone();

                    }

                }
                else if (mx.X.Equals(points[i].X))
                {
                    if (mx.Y < points[i].Y)//get  upper most mx X
                    {

                        mx = (Point)points[i].Clone();

                    }

                }

            }

            if (!get_min)
                return mx;
            return mini;

        }
        public static int _find_min_Y_index(List<Point> points)//graham [edited]
        {

            Point target = (Point)points[0].Clone();
            int indx = 0;

            for (int i = 1; i < points.Count; i++)
            {

                if (target.Y > points[i].Y)
                {

                    target = (Point)points[i].Clone();
                    indx = i;
                }

                if (target.Y.Equals(points[i].Y))
                {
                    if (target.X < points[i].X)
                    {

                        target = (Point)points[i].Clone();
                        indx = i;
                    }

                }

            }
            return indx;
        }

        public static int _find_max_Y_index(List<Point> points)//graham [edited]
        {

            Point target = (Point)points[0].Clone();
            int indx = 0;

            for (int i = 1; i < points.Count; i++)
            {

                if (target.Y < points[i].Y)
                {

                    target = (Point)points[i].Clone();
                    indx = i;
                }

                if (target.Y.Equals(points[i].Y))
                {
                    if (target.X > points[i].X) //upper rightmost
                    {

                        target = (Point)points[i].Clone();
                        indx = i;
                    }

                }

            }
            return indx;
        }

        public static double calculate_perpendicular_distance(Point a, Point b, Point c)//c --> point && a,b --> line
        {
            double distance = Math.Abs((a.X - b.X) * (a.Y - c.Y) - (a.Y - b.Y) * (a.X - c.X)) / Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
            return distance;
        }
    }
}
