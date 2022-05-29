using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremeSegments : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            int extreme = -2;

            for (int i = 0; i < points.Count; i++)
            { // 3 , 3
                
                for (int j = 0; j < points.Count; j++)
                {


                    extreme = -2;
                    if (!points[j].Equals(points[i]))
                        for (int k = 0; k < points.Count; k++)
                        {

                            extreme = 1; 
                            if (!points[k].Equals(points[i]) && !points[k].Equals(points[j]))
                            {

                                Point v1 = points[i].Vector(points[j]);
                                Point v2 = points[i].Vector(points[k]);

                                // var s = HelperMethods.CheckTurn(v1, v2);

                                if ( HelperMethods.CheckTurn(v1, v2).Equals(Enums.TurnType.Right))
                                {

                                    extreme = 0;
                                    break;


                                }

                                if (HelperMethods.CheckTurn(v1, v2).Equals(Enums.TurnType.Colinear))
                                {
                                    double phat_to_p_dist_II = Math.Sqrt((points[i].X - points[j].X) * (points[i].X - points[j].X) + (points[i].Y - points[j].Y) * (points[i].Y - points[j].Y));
                                    double phat_to_target_dist_II = Math.Sqrt((points[k].X - points[j].X) * (points[k].X - points[j].X) + (points[k].Y - points[j].Y) * (points[k].Y - points[j].Y));
                                    double p_to_target_II = Math.Sqrt((points[k].X - points[i].X) * (points[k].X - points[i].X) + (points[k].Y - points[i].Y) * (points[k].Y - points[i].Y));
                                    
                                    if ((phat_to_target_dist_II + p_to_target_II > phat_to_p_dist_II)) {

                                        extreme = 0;
                                        break;

                                    }
                                }

                            }


                        }

                    if (extreme == 1 && !outPoints.Contains(points[i]))
                    {

                        outPoints.Add(points[i]);
                    }


                }


            }

            if (points.Count == 1)
                outPoints.Add(points[0]);

        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Segments";
        }
    }
}
