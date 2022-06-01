using System;
using System.Collections.Generic;
using CGUtilities;
using CGUtilities.DataStructures;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.SegmentIntersection
{


    class element
    {

        public int index;
        public Line line;

        public element(int index, Line line)
        {
            this.index = index;
            this.line = line;
        }

        public object Clone()
        {
            return new element(index, (Line)line.Clone());
        }
    }


    class Event
    {

        public Point point;
        public String event_type;
        public Line line_I;
        public Line line_II;

        public int line_I_index;
        public int line_II_index;

        public Event(Point point, string event_type, Line line_I, Line line_II, int line_I_index, int line_II_index)
        {
            this.point = point;
            this.event_type = event_type;
            this.line_I = line_I;
            this.line_II = line_II;
            this.line_I_index = line_I_index;
            this.line_II_index = line_II_index;
        }

        public object Clone()
        {

            if (line_II != null)
                return new Event((Point)point.Clone(), event_type, line_I, line_II, line_I_index, line_II_index);

            return new Event((Point)point.Clone(), event_type, line_I, null, line_I_index, -1);


        }
    }


    class SweepLine : Algorithm
    {

        OrderedSet<element> sweep_line;
        OrderedSet<Event> events;
        List<Line> input_lines;
        List<Point> intersections;
        Event current;

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            sweep_line = new OrderedSet<element>(new Comparison<element>(CompareEventsY));
            events = new OrderedSet<Event>(new Comparison<Event>(CompareEventsX));
            input_lines = new List<Line>(); //original

            intersections = new List<Point>();

            //adjust the points 

            adjust_input(lines);
            IntializeEvents();


            while (events.Count != 0)
            {

                Event current_event = (Event)events[0].Clone();
                current = (Event)events[0].Clone();

                HandleEvent(current_event);
                events.Remove(current_event);

            }


            for (int i = 0; i < intersections.Count; i++)
            {

                if (!outPoints.Contains((Point)intersections[i].Clone()))
                    outPoints.Add((Point)intersections[i].Clone());


            }

        }

        public int CompareEventsY(element e1, element e2)
        {
            if (e1.line.Start.Y < e2.line.Start.Y)
                return -1;
            else if (e1.line.Start.Y > e2.line.Start.Y)
                return 1;
            else if (e1.line.Start.Y == e2.line.Start.Y)
            {

                if (e1.line.Start.X < e2.line.Start.X)
                    return -1;
                else if (e1.line.Start.X > e2.line.Start.X)
                    return 1;
                else
                {
                    if (e1.line.End.Y < e2.line.End.Y)
                        return -1;
                    else if (e1.line.End.Y == e2.line.End.Y)
                    {
                        if (e1.line.End.X < e2.line.End.X)
                            return -1;
                        else if (e1.line.End.X > e2.line.End.X)
                            return 1;

                        else return 0;

                    }
                    else
                        return 1;
                }
            }


            return -10000;
        }

        public int CompareEventsX(Event p1, Event p2)
        {

            if (p1.point.X < p2.point.X)
                return -1;
            else if (p1.point.X > p2.point.X)
                return 1;
            else
            {
                if (p1.point.Y < p2.point.Y)
                    return -1;
                else if (p1.point.Y > p2.point.Y)
                    return 1;

                else
                    return 0;
            }


        }

        void IntializeEvents()
        {
            for (int i = 0; i < input_lines.Count; i++)
            {
                events.Add(new Event((Point)input_lines[i].Start.Clone(), "S", (Line)input_lines[i].Clone(), null, i, -1));
                events.Add(new Event((Point)input_lines[i].End.Clone(), "E", (Line)input_lines[i].Clone(), null, i, -1));

            }

        }

        void adjust_input(List<Line> lines)
        {

            for (int i = 0; i < lines.Count; i++)
            {

                if (lines[i].End.X < lines[i].Start.X)
                {

                    input_lines.Add(new Line((Point)lines[i].End.Clone(), (Point)lines[i].Start.Clone()));
                }
                else
                {
                    input_lines.Add(new Line((Point)lines[i].Start.Clone(), (Point)lines[i].End.Clone()));
                }
            }


        }

        Point Intersection_point(Line line1, Line line2)
        {

            Point intersection;

            double slope1 = (line1.End.Y - line1.Start.Y) / (line1.End.X - line1.Start.X);
            double slope2 = (line2.End.Y - line2.Start.Y) / (line2.End.X - line2.Start.X);


            double b1 = line1.Start.Y - (slope1 * line1.Start.X);
            double b2 = line2.Start.Y - (slope2 * line2.Start.X);


            double x_intersection = (b2 - b1) / (slope1 - slope2);
            double y_intersection = slope1 * x_intersection + b1;

            intersection = new Point(x_intersection, y_intersection);

            bool l1 = HelperMethods.PointOnSegment(intersection, line1.Start, line1.End);
            bool l2 = HelperMethods.PointOnSegment(intersection, line2.Start, line2.End);

            if (l1 == true && l2 == true)
            {

                return intersection;

            }

            return null;

        }

        void HandleEvent(Event current_event)
        {

            if (current_event.event_type == "S")
                HandleStart(current_event);
            else if (current_event.event_type == "E")
                HandleEnd(current_event);
            else
                HandleIntersection(current_event);

        }

        void HandleStart(Event current_event)
        {

            sweep_line.Add((element)new element(current_event.line_I_index, (Line)input_lines[current_event.line_I_index]));

            //get prev and next in the sweep line 
            element prev = sweep_line.DirectUpperAndLower((element)new element(current_event.line_I_index, (Line)input_lines[current_event.line_I_index])).Value;
            element next = sweep_line.DirectUpperAndLower((element)new element(current_event.line_I_index, (Line)input_lines[current_event.line_I_index])).Key;

            if (prev != null)
            {

                Point prev_intersection = Intersection_point((Line)input_lines[current_event.line_I_index].Clone(), (Line)input_lines[prev.index].Clone());

                if (prev_intersection != null)
                {

                    Event intersection = new Event((Point)prev_intersection.Clone(), "I", (Line)input_lines[current_event.line_I_index], (Line)input_lines[prev.index], current_event.line_I_index, prev.index);

                    events.Add((Event)intersection.Clone());

                    intersections.Add((Point)prev_intersection.Clone());

                }

            }


            if (next != null)
            {

                Point next_intersection = Intersection_point((Line)input_lines[current_event.line_I_index].Clone(), (Line)input_lines[next.index].Clone());


                if (next_intersection != null)
                {

                    Event intersection = new Event((Point)next_intersection.Clone(), "I", (Line)input_lines[current_event.line_I_index], (Line)input_lines[next.index], current_event.line_I_index, next.index);

                    events.Add((Event)intersection.Clone());

                    intersections.Add((Point)next_intersection.Clone());

                }
            }


        }

        void HandleEnd(Event current_event)
        {


            element target = (element)new element(current_event.line_I_index, (Line)input_lines[current_event.line_I_index]);

            element prev = sweep_line.DirectUpperAndLower((element)target.Clone()).Value;
            element next = sweep_line.DirectUpperAndLower((element)target.Clone()).Key;

            if ((prev != null) && (next != null))
            {

                Point intersection_point = Intersection_point((Line)input_lines[next.index].Clone(), (Line)input_lines[prev.index].Clone());

                if (intersection_point != null)
                {

                    Event intersection = new Event((Point)intersection_point.Clone(), "I", (Line)input_lines[prev.index], (Line)input_lines[next.index], prev.index, next.index);


                    events.Add((Event)intersection.Clone());

                    intersections.Add((Point)intersection_point.Clone());

                }

            }

            sweep_line.Remove((element)target.Clone());

        }

        void HandleIntersection(Event current_event)
        {

            Line lower = (Line)input_lines[current_event.line_I_index].Clone();
            Line higher = (Line)input_lines[current_event.line_II_index].Clone();

            element e_lower = new element(current_event.line_I_index, lower);
            element e_higher = new element(current_event.line_II_index, higher);

            int index_lower = sweep_line.IndexOf(e_lower);
            int index_higher = sweep_line.IndexOf(e_higher);


            element prev;
            element next;

            if (index_lower < index_higher)
            {

                next = sweep_line.DirectUpperAndLower(e_higher).Key;
                prev = sweep_line.DirectUpperAndLower(e_lower).Value;

            }
            else
            {

                Line tmp = higher;
                higher = lower;
                lower = tmp;

                int tmpi = current_event.line_I_index;
                current_event.line_I_index = current_event.line_II_index;
                current_event.line_II_index = tmpi;


                Line tmp_line = current_event.line_I;
                current_event.line_I = current_event.line_II;
                current_event.line_II = tmp_line;

                element temp = (element)e_higher.Clone();
                e_higher = (element)e_lower.Clone();
                e_lower = (element)temp.Clone();


                next = sweep_line.DirectUpperAndLower(e_higher).Key;
                prev = sweep_line.DirectUpperAndLower(e_lower).Value;
            }


            if (prev != null)
            {

                Point prev_intersection = Intersection_point(higher, (Line)input_lines[prev.index].Clone());

                if (prev_intersection != null)
                {

                    Event intersection = new Event((Point)prev_intersection.Clone(), "I", (Line)higher.Clone(), (Line)input_lines[prev.index], current_event.line_II_index, prev.index);


                    events.Add((Event)intersection.Clone());

                    intersections.Add((Point)prev_intersection.Clone());

                }

            }


            if (next != null)
            {

                Point next_intersection = Intersection_point(lower, (Line)input_lines[next.index].Clone());


                if (next_intersection != null)
                {

                    Event intersection = new Event((Point)next_intersection.Clone(), "I", (Line)lower.Clone(), (Line)input_lines[next.index], current_event.line_I_index, next.index);

                    events.Add((Event)intersection.Clone());

                    intersections.Add((Point)next_intersection.Clone());

                }
            }

            sweep_line.Remove(new element(current_event.line_I_index, lower));
            sweep_line.Remove(new element(current_event.line_II_index, higher));

            input_lines[current_event.line_I_index].Start = current_event.point;
            input_lines[current_event.line_II_index].Start = current_event.point;

            sweep_line.Add(new element(current_event.line_I_index, new Line(current_event.point, lower.End)));
            sweep_line.Add(new element(current_event.line_II_index, new Line(current_event.point, higher.End)));
        }


        public override string ToString()
        {
            return "Sweep Line";
        }
    }
}
