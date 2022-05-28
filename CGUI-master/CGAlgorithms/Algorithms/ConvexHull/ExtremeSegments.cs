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

                                if (! HelperMethods.CheckTurn(v1, v2).Equals(Enums.TurnType.Left))
                                {

                                    extreme = 0;
                                    break;


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
