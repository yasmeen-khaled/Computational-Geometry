using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremePoints : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            for (int i = 0; i < points.Count(); i++)
            {
                bool extreme = true;
                for (int j = 0; j < points.Count(); j++)
                {
                    if(!points[j].Equals(points[i]))
                    for (int k = 0; k < points.Count(); k++)
                    {
                        if (!points[k].Equals(points[i]) && !points[k].Equals(points[j]))
                        for (int l = 0; l < points.Count(); l++)
                        {
                            if(!points[l].Equals(points[i]) && !points[l].Equals(points[j]) && !points[l].Equals(points[k]))
                            {
                                if(HelperMethods.PointInTriangle(points[i] , points[j], points[k], points[l]) != Enums.PointInPolygon.Outside)
                                {
                                            extreme = false;
                                            break;
                                }
                            }
                                 
                        }
                            if (extreme == false)
                                break;
                    }
                    if (extreme == false)
                        break;
                }
                if (extreme == true && !outPoints.Contains(points[i]))

                    outPoints.Add(points[i]);
            }
        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Points";
        }
    }
}
