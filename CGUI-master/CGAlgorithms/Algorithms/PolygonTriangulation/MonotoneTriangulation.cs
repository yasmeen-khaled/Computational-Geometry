using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities;
//added check turn(3 points) to helper methos

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class MonotoneTriangulation  :Algorithm
    {
        public override void Run(System.Collections.Generic.List<CGUtilities.Point> points, System.Collections.Generic.List<CGUtilities.Line> lines, System.Collections.Generic.List<CGUtilities.Polygon> polygons, ref System.Collections.Generic.List<CGUtilities.Point> outPoints, ref System.Collections.Generic.List<CGUtilities.Line> outLines, ref System.Collections.Generic.List<CGUtilities.Polygon> outPolygons)
        {
            if (points.Count == 0)
            {
                points = CommonMethods.get_points(lines);
            }
            int sz = points.Count;
            // [1]  make CCW
            int mn_ind_beforeSort = make_CCW(points);//O(n)
            // [2] todo: check monotone
            bool monotone = check_monotone(mn_ind_beforeSort, points); //O(n)
            if(! monotone)
            {
                Point p1 = new Point(0, 0);
                Point p2 = new Point(100, 100);
                outLines.Add(new Line(p1, p2));

                p1 = new Point(0, 100);
                p2 = new Point(100, 0);
                outLines.Add(new Line(p1, p2));
            }
            else
            {
                // [3] sort on max Y and max X on tie --> 0(n)
                int mx_index = 0;
                int[] indicesMap = new int[1];
                List<Point> sorted_points = sort_onMaxY(points, ref mx_index, ref indicesMap);//O(n^2)
                                                                                              // [ ] which side //0(n)
                List<int> left_side = which_side(points, sorted_points, mx_index, indicesMap);

                // [4] triangulate
                outLines = triangulate_monotone(sorted_points, left_side); //O(n)
                int dummy = 1;
            }
        }
        
        bool check_monotone(int mx_indx , List<Point> points)
        {
            int sz = points.Count;
            bool first_part = true;
            for (int i = mx_indx; i < sz+mx_indx; i++)
            {
                int indx = i % sz;
                int next = (i + 1) % sz;
                double diff = points[indx].Y - points[next].Y;
                bool negative = (diff < 0); //positive --> actual first part
                if (first_part && !negative)
                {
                    first_part = false;
                }
                else if (!first_part && negative)
                    return false;
            }
            return true;
        }

        int make_CCW(List<Point> p_points)//and check ccw
        {
            
            int sz = p_points.Count;
            int i = CommonMethods._find_min_Y_index(p_points);
            //check CCW
            int prev = (i - 1 + sz) % sz;
            int next = (i + 1) % sz;
            if (HelperMethods.CheckTurn(p_points[prev], p_points[i], p_points[next]) == Enums.TurnType.Right)
            {
                //call reverse
                p_points.Reverse();
                i = sz - i - 1;
            }
            return i;
        }

        void swap(Point a , Point b)
        {
            Point tmp = (Point)a.Clone();
            a = (Point)b.Clone();
            b = (Point)tmp.Clone();
        }
        List<Point> sort_onMaxY(List<Point> points , ref int mx_indx , ref int[] indicesMap)
        {
            List<Point> sorted_points = new List<Point>(points);
            int sz = points.Count;
            int[] arr = fill_array(sz);

            for (int i = 0; i < sz; i++)
            {
                Point mx= (Point)sorted_points[i].Clone();
                //bool assigned = false;
                //mx_indx = 0;
                int j_ = i;
                mx = (Point)sorted_points[i].Clone();
                for (int j = i + 1; j < sz; j++)
                {
                    //mx = (Point)points[i].Clone(); 
                    int dummy;
                    if (i == 3)
                        dummy = 1;
                    if ((mx.Y < sorted_points[j].Y)|| ((mx.Y == sorted_points[j].Y )&& (mx.X < sorted_points[j].X)))
                    {
                        j_ = j;
                        mx = (Point)sorted_points[j].Clone();
                    }

                }
                //sorted_points.Add((Point)points[i].Clone());
                if(j_ != i)
                {
                    //swap(sorted_points[i], sorted_points[j]);
                    Point tmp = (Point)sorted_points[i].Clone();
                    sorted_points[i] = (Point)sorted_points[j_].Clone();////
                    sorted_points[j_] = (Point)tmp.Clone();
                    //swap(i,j) in array
                    int tmp2 = arr[i];
                    arr[i] = arr[j_];
                    arr[j_] = tmp2;
                    //assigned = true;
                    if (i == 0)
                        mx_indx = j_;
                }
            }
            //create indices map
            indicesMap = create_indicesMap(ref arr);
            return sorted_points;
        }
        int[] create_indicesMap(ref int [] arr)
        {
            int sz = arr.Length;
            int[] indicesMap = new int[sz];

            for(int i=0;i<sz;i++)
            {
                indicesMap[arr[i]] = i;
            }

            return indicesMap;
        }

        List<int> which_side(List<Point> points , List<Point> sorted_points , int mx_index , int[] indicesMap)
        {
            int sz = points.Count;
            Point min = sorted_points[sz - 1];
            

            List<int> left_side = new List<int>(new int[sz]);
            bool right_side = false;
            for (int i = mx_index; i < sz + mx_index; i++) //todo: circular
            {
                int circular_index = i % sz;

                if (points[circular_index].Equals(min))
                {
                    right_side = true;
                    left_side[indicesMap[circular_index]] = 0;
                }
                else if (points[circular_index].Equals(points[mx_index])) left_side[indicesMap[circular_index]] = 0;
                else if (right_side) left_side[indicesMap[circular_index]] = -1;
                else left_side[indicesMap[circular_index]] = 1;
            }
            return left_side;
        }
        List<Line> triangulate_monotone(List<Point> points , List<int> left_side)//points -- > sorted
        {
            List<Line> output_diagonals = new List<Line>();

            Stack<Tuple<Point, int>> stk = new Stack<Tuple<Point, int>>();

            stk.Push(new Tuple<Point, int> (points[0], 0));
            stk.Push(new Tuple<Point, int>(points[1], 1));

            int sz = points.Count;
            int i = 2;
            while (i != sz)
            {
                Point p = (Point)points[i].Clone();
                Tuple<Point, int> top = stk.Peek();

                if (Math.Abs( left_side[i] - left_side[top.Item2]) <= 1|| i == sz - 1)//same side
                {
                    stk.Pop();
                    Tuple<Point, int> top2 = stk.Peek();
                    Enums.TurnType convex_turn = Enums.TurnType.Left;
                    if (left_side[top.Item2] == 1)
                        convex_turn = Enums.TurnType.Right;
                    if(HelperMethods.CheckTurn(p , top.Item1 , top2.Item1) == convex_turn)//convex
                    {
                        output_diagonals.Add(new Line(p, top2.Item1));
                        if(stk.Count == 1)
                        {
                            stk.Push(new Tuple<Point, int>(p, i));
                            i++;
                        }
                    }
                    else
                    {
                        stk.Push(new Tuple<Point, int>(top.Item1 , top.Item2));
                        stk.Push(new Tuple<Point, int>(p, i));
                        i++;
                    }
                }
                else//different sides
                {
                    while(stk.Count != 1)
                    {
                        var top2 = stk.Peek();
                        stk.Pop();
                        output_diagonals.Add(new Line(p, top2.Item1));
                    }
                    stk.Pop();
                    stk.Push(new Tuple<Point, int>(top.Item1, top.Item2));
                    stk.Push(new Tuple<Point, int>(p, i));
                    i++;
                }
            }
            output_diagonals.RemoveAt(output_diagonals.Count - 1);
            return output_diagonals;
        }

        int [] fill_array(int sz)
        {
            int [] filled_arr = new int [sz];

            for (int i = 0; i < sz; i++)
            {
                filled_arr[i] = i;
            }

            return filled_arr;
        }
        public override string ToString()
        {
            return "Monotone Triangulation";
        }
    }
}
