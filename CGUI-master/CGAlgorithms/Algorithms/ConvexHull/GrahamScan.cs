using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class GrahamScan : Algorithm
    {

        private Point _find_min_Y_value(List<Point> points) {

            Point target = (Point)points[0].Clone();

            for (int i = 1; i < points.Count; i++) {

                if (target.Y > points[i].Y) {

                    target = (Point)points[i].Clone();  
                }

                if (target.Y.Equals(points[i].Y))
                {
                    if (target.X < points[i].X) {

                        target = (Point)points[i].Clone();

                    }

                }
            
            }


            return target; 
        
        }



        private List<Point> trial_1( List<Point> points ) {

            List<Point> outPoints = new List<Point>(); 

            // [1] find the vetex that has the lowest y - value

            Point middel = _find_min_Y_value(points);

            // [2] calculate the angles 

            //start point 
            // middel --> target
            // end --> our new point 

            // get start point 
            Point start = new Point(middel.X + 1, middel.Y);

            Dictionary<Point, double> dic_point = new Dictionary<Point, double>();

            for (int i = 0; i < points.Count; i++)
            {

                double angle = GetAngleBetweenPoints(points[i], middel, start);
                if (angle == 0)
                    angle = 360;

                if (!points[i].Equals(middel))
                    dic_point.Add(points[i], angle);
            }

            // [3] get the sorted list of points 
            List<Point> sortedpoints = new List<Point>();


            sortedpoints.Add(middel);
            foreach (KeyValuePair<Point, double> p in dic_point.OrderBy(key => key.Value))
            {
                sortedpoints.Add((Point)p.Key.Clone());

            }



            int d = 0;
            // [4] do graham logic 

            outPoints.Add(middel);
            sortedpoints.RemoveAt(0);

            if (sortedpoints.Count > 0)
            {
                outPoints.Add((Point)sortedpoints[0].Clone());
                sortedpoints.RemoveAt(0);
            }

            int counter = 0;
            while (sortedpoints.Count != counter)
            {

                Point p = (Point)outPoints[outPoints.Count - 1].Clone();
                Point p_hat = (Point)outPoints[outPoints.Count - 2].Clone();


                Point v1 = p_hat.Vector(p);
                Point v2 = p_hat.Vector(sortedpoints[counter]);



                if (HelperMethods.CheckTurn(v1, v2).Equals(Enums.TurnType.Left))
                {

                    if (!outPoints.Contains(sortedpoints[counter]))
                    {

                        outPoints.Add((Point)sortedpoints[counter].Clone());
                    }


                    counter++;


                }

                else if (HelperMethods.CheckTurn(v1, v2).Equals(Enums.TurnType.Colinear))
                {


                    double phat_to_p_dist = Math.Sqrt((p.X - p_hat.X) * (p.X - p_hat.X) + (p.Y - p_hat.Y) * (p.Y - p_hat.Y));
                    double phat_to_target_dist = Math.Sqrt((sortedpoints[counter].X - p_hat.X) * (sortedpoints[counter].X - p_hat.X) + (sortedpoints[counter].Y - p_hat.Y) * (sortedpoints[counter].Y - p_hat.Y));
                    double p_to_target = Math.Sqrt((sortedpoints[counter].X - p.X) * (sortedpoints[counter].X - p.X) + (sortedpoints[counter].Y - p.Y) * (sortedpoints[counter].Y - p.Y));




                    if ((phat_to_target_dist + p_to_target > phat_to_p_dist))
                    {

                        outPoints.RemoveAt(outPoints.Count - 1);

                        if (!outPoints.Contains(sortedpoints[counter]))
                        {

                            outPoints.Add((Point)sortedpoints[counter].Clone());

                        }

                    }

                    counter++;


                }

                else
                {
                    outPoints.RemoveAt(outPoints.Count - 1);
                }

            }


            if (sortedpoints.Count > 3)
            {

                Point p_II = (Point)outPoints[outPoints.Count - 1].Clone();
                Point p_hat_II = (Point)outPoints[outPoints.Count - 2].Clone();


                Point v1_II = p_hat_II.Vector(p_II);
                Point v2_II = p_hat_II.Vector(outPoints[0]);

                if (HelperMethods.CheckTurn(v1_II, v2_II).Equals(Enums.TurnType.Colinear))
                {


                    double phat_to_p_dist_II = Math.Sqrt((p_II.X - p_hat_II.X) * (p_II.X - p_hat_II.X) + (p_II.Y - p_hat_II.Y) * (p_II.Y - p_hat_II.Y));
                    double phat_to_target_dist_II = Math.Sqrt((outPoints[0].X - p_hat_II.X) * (outPoints[0].X - p_hat_II.X) + (outPoints[0].Y - p_hat_II.Y) * (outPoints[0].Y - p_hat_II.Y));
                    double p_to_target_II = Math.Sqrt((outPoints[0].X - p_II.X) * (outPoints[0].X - p_II.X) + (outPoints[0].Y - p_II.Y) * (outPoints[0].Y - p_II.Y));

                    if ((phat_to_target_dist_II + p_to_target_II > phat_to_p_dist_II))
                    {

                        outPoints.RemoveAt(outPoints.Count - 1);



                    }
                }
            }
            return outPoints; 
        }

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {

            List <Point> first = trial_1(points );
            outPoints = trial_1(first); 
        }
        public override string ToString()
        {
            return "Convex Hull - Graham Scan";
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
          

    }
}
