using CGUtilities;

using System;
using CGUtilities.DataStructures;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class Incremental : Algorithm
    {






        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {

            OrderedSet<Tuple<double , int >> CH = new OrderedSet<Tuple<double  , int >>();

            //CH.Add(Tuple.Create(1.1, new Point(1, 2))) ; 

            if (points.Count < 3)
            {

                for (int i = 0; i < points.Count; i++)
                {

                    if (!outPoints.Contains((Point)points[i].Clone()))
                    {

                        outPoints.Add((Point)points[i].Clone());
                    }
                }



            }
            else
            {


                //[1] get the first three points --> make sure that these points are not colinear 

                Point p1 = (Point)points[0].Clone();
                Point p2 = (Point)points[1].Clone();
                Point p3 = (Point)points[2].Clone();


                Point v1 = p1.Vector(p2);
                Point v2 = p1.Vector(p3);

                int counter = 2;
                while (HelperMethods.CheckTurn(v1, v2) == Enums.TurnType.Colinear)
                {

                    counter++;

                    p3 = (Point)points[counter].Clone();


                    v1 = p1.Vector(p2);
                    v2 = p1.Vector(p3);

                }




                //[2] get the middle point 
                // X  , Y
                Point middel_point = new Point((double)((p1.X + p2.X + p3.X) / 3), (double)((p1.Y + p2.Y + p3.Y) / 3));

                //[3] make the extention 
                Point NB = new Point(middel_point.X + 10, middel_point.Y);


                CH.Add(Tuple.Create(GetAngleBetweenPoints(points[0], middel_point, NB), 0));
                CH.Add(Tuple.Create(GetAngleBetweenPoints(points[1], middel_point, NB),1));
                CH.Add(Tuple.Create(GetAngleBetweenPoints(points[counter], middel_point, NB), counter));



                for (int i = 3; i < points.Count; i++)
                {

                    double angle = GetAngleBetweenPoints(points[i], middel_point, NB);
                    var prev = CH.DirectUpperAndLower(Tuple.Create( angle, i)).Value;

                    if (prev == null) {

                        prev = CH[CH.Count-1]; 
                    
                    }

                    var next = CH.DirectUpperAndLower(Tuple.Create(angle, i)).Key;

                    if (next == null)
                    {

                        next = CH[0];

                    }


                    v1 = points[i].Vector(points[prev.Item2]);
                    v2 = points[i].Vector(points[next.Item2]);

                    var s = HelperMethods.CheckTurn(v1, v2);

                    if (s == Enums.TurnType.Right) {

                        //upper tangent
                        var newprev = CH.DirectUpperAndLower(prev).Value;

                        if (newprev == null)
                        {

                            newprev = CH[CH.Count - 1];

                        }

                        while (HelperMethods.CheckTurn(points[i].Vector(points[prev.Item2]), points[i].Vector(points[newprev.Item2])) == Enums.TurnType.Left || HelperMethods.CheckTurn(points[i].Vector(points[prev.Item2]), points[i].Vector(points[newprev.Item2])) == Enums.TurnType.Colinear)
                        {
                            var oldprev_index = prev;
                           
                            CH.Remove(oldprev_index); 
                           
                            prev = newprev;
                            
                            newprev = CH.DirectUpperAndLower(prev).Value;

                            if (newprev == null)
                            {

                                newprev = CH[CH.Count - 1];

                            }

                        }

                        next = CH.DirectUpperAndLower(Tuple.Create(angle, i)).Key;

                        if (next == null)
                        {

                            next = CH[0];

                        }

                        var newnext = CH.DirectUpperAndLower(next).Key;

                        if (newnext == null)
                        {

                            newnext = CH[0];

                        }

                        while (HelperMethods.CheckTurn(points[i].Vector(points[next.Item2]), points[i].Vector(points[newnext.Item2])) == Enums.TurnType.Right || HelperMethods.CheckTurn(points[i].Vector(points[next.Item2]), points[i].Vector(points[newnext.Item2])) == Enums.TurnType.Colinear)
                        {
                            var oldnext_index = next;

                            CH.Remove(oldnext_index);

                            next = newnext;

                            newnext = CH.DirectUpperAndLower(next).Key;

                            if (newnext == null)
                            {

                                newnext = CH[0];

                            }


                        }

                        CH.Add(Tuple.Create(angle, i));

                    }
                      
                }


                for (int i = 0; i < CH.Count; i++)
                {
                    outPoints.Add((Point)points[CH.ElementAt(i).Item2].Clone() ) ;
                }

            }
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

        public override string ToString()
        {
            return "Convex Hull - Incremental";
        }
    }
}
