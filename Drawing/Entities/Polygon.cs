using Drawing.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drawing.Entities
{
    public class Polygon: EntityObject
    {
        public Point2D[] Nodes;
        public Line[] Sides;
        private double MaxX
        {
            get
            {
                double max = Nodes[0].X;
                for (int i = 1; i < Nodes.Length; i++)
                    max = Math.Max(max, Nodes[i].X);
                return max;
            }
        }
        public Polygon() : this(new Point2D[] { new Point2D(0, 0), new Point2D(0, 5), new Point2D(5, 5), new Point2D(5, 0) }) { }
        public Polygon(Line[] sides):base(EntityType.Polygon)
        {
            Nodes = new Point2D[sides.Length];
            if (sides.Length <= 2)
                throw new ArgumentException("Not enough sides!");
            for (int c = 0; c < sides.Length; c++) // совпадение точек(одинаковые)
            {
                for (int k = 0; k < sides.Length; k++)
                {
                    if (k == c && k != sides.Length - 1) k++;

                    if (sides[k].P1.X == sides[c].P1.X && sides[k].P1.Y == sides[c].P1.Y && k != c)
                        throw new ArgumentException("Same nodes!");
                }
                Nodes[c] = sides[c].P1;
            }
            Nodes[sides.Length - 1] = sides[sides.Length - 1].P2;
            Sides = sides;
            if (GetSquare() > 0) //ориентированная площадь с положительным знаком - против часовой
                ClockWise(); //-меняем направление(по часовой стрелке)

        }
        public Polygon(Point2D[] nodes):base(EntityType.Polygon)
        {
            if (nodes.Length < 3) throw new ArgumentException("Not enough nodes to create a polygon-!");
            for (int c = 0; c < nodes.Length; c++) // совпадение точек(одинаковые)
            {
                for (int k = 0; k < nodes.Length; k++)
                {
                    if (k == c && k != nodes.Length - 1) k++;

                    if (nodes[k].X == nodes[c].X && nodes[k].Y == nodes[c].Y && k != c)
                        throw new ArgumentException("Same nodes!");
                }
            }
            Sides = new Line [nodes.Length];
            Nodes = nodes;
            if (GetSquare() > 0) //ориентированная площадь с положительным знаком - против часовой
                ClockWise(); //-меняем направление(по часовой стрелке)
            for (int j = 0; j < Nodes.Length - 1; j++)
            {
                Sides[j] = new Line(Nodes[j], Nodes[j + 1]);
            } // массив сторон 
            Sides[Nodes.Length - 1] = new Line(Nodes[Nodes.Length - 1], Nodes[0]);
        }
        private double GetLenght(Point2D start, Point2D finish)
        {
            return Math.Sqrt(Math.Pow(start.X - finish.X, 2) + Math.Pow(start.Y - finish.Y, 2));
        }
        public double Perimetr
        {
            get
            {
                double res = 0;
                for (int c = 0; c < Sides.Length; c++)
                    res += Sides[c].Lenght * 2;
                return res;
            }
        }
        public double Square
        {
            get
            {
                return Math.Abs(GetSquare());
            }
        }
        private double GetSquare() //площадь со знаком
        {
            double res = 0;
            int len = Nodes.Length - 1;

            for (int c = 0; c < len; c++)
            {
                res += (Nodes[c].X * Nodes[c + 1].Y
                    - Nodes[c].Y * Nodes[c + 1].X);
            }
            res += (Nodes[len].X * Nodes[0].Y
                    - Nodes[len].Y * Nodes[0].X);

            return res * 0.5;
        }
        public void Shift(double x, double y)
        {
            for (int c = 0; c < Nodes.Length; c++)
            {
                Nodes[c].X += x;
                Nodes[c].Y += y;
            }
        }
        public void Shift(double x)
        {
            Shift(x, x);
        }
        public static bool operator <(Polygon p1, Polygon p2)
        {
            return p1.Perimetr < p2.Perimetr;
        }
        public static bool operator >(Polygon p1, Polygon p2)
        {
            return p1.Perimetr > p2.Perimetr;
        }
        
        //public Point3D this[int index]
        //{
        //    get { return Nodes[index]; }
        //}
        public Line this[int index]
        {
            get { return Sides[index]; }
        }
        public override string ToString()
        {
            StringBuilder str = new StringBuilder(Nodes[0].ToString());
            for (int c = 1; c < Nodes.Length; c++)
                str.Append("; \n" + Nodes[c]);
            return str.ToString();
        }
        private void ClockWise()
        { //меняем направление многоугольника по часовой стрелке
            for (int c = 0, k = Nodes.Length - 1; c < Nodes.Length / 2; c++, k--)
            {
                //0,1,2,3,4,5 => 5,4,3,2,1,0
                Point2D temp = Nodes[c];
                Nodes[c] = Nodes[k];
                Nodes[k] = temp;
            }

        }
        public Polygon Substract(Polygon subP)
        {
            Polygon polygon = null;
            List<Point2D> points = new List<Point2D>();

            for (int c = 0; c < Sides.Length; c++)
            {
                Line side1 = Sides[c];

                if (subP.IsInside(side1.P1)) //если точка внутри добавляем ее в конец списка
                    points.Add(side1.P1);
                List<Point2D> temp = CrossAndInsidePoint(side1, subP);
                if (temp.Count > 0)
                    points.AddRange(temp); //добавляем в конец списка цепочку точек пересечения и/или внутренних точек

            }
            if (points.Count > 0) //если пересекаются образуем новый полигон, иначе возвращаем null
                polygon = new Polygon(points.ToArray());
            return polygon;
        }
        private List<Point2D> CrossAndInsidePoint(Line side1, Polygon subP)
        {
            List<Point2D> result = new List<Point2D>();
            List<List<Point2D>> temp = new List<List<Point2D>>();
            for (int k = 0; k < subP.Sides.Length; k++)
            {
                Line side2 = subP.Sides[k];
                double t2 = Parametric.Get_T2(side1, side2);
                double t1 = Parametric.Get_T1(t2, side1, side2);
                if (Parametric.CrossPoint(side1, side2, out Point2D cross)) //проверка на пересечение
                {
                    temp.Add(new List<Point2D> { cross });  //новая цепочка точек                 

                    int n = k;
                    bool flag = true;
                    //цепочка внутренних точек после пересечения
                    do
                    {
                        if (IsInside(subP.Sides[n].P2))
                        {
                            temp[temp.Count - 1].Add(subP.Sides[n].P2);
                            n = n >= subP.Nodes.Length - 1 ? 0 : n + 1; // меняем индекс                             
                        }
                        else
                            flag = false;
                    }
                    while (flag); //цикл завершится при условии что точка снаружи
                    k = k < n ? n : k; //если имелись внутренние точки то линии не пересекаются с базовой
                }
            }
            if (temp.Count > 0) //количество пересечений
            {
                if (temp.Count > 1)
                    result = SortPoints(temp, side1.P1); // сортируем по расстоянию от начала базового отрезка
                else
                    result.AddRange(temp[0]);
            }
            return result;
        }
        private static List<Point2D> SortPoints(List<List<Point2D>> list, Point2D p)
        {
            var distancelist =
                list.Select((t, idx) => new
                {
                    Index = idx,
                    Distance = p.DistatnceTo(t.First()) //расстояние от начала отрезка до пересечения
                }).OrderBy(n => n.Distance).ToList();
            List<Point2D> result = new List<Point2D>();

            foreach (var dc in distancelist)
                result.AddRange(list[dc.Index]); // добавляем по порядку
            return result;
        }
        public Polygon Join(Polygon subP)
        {
            Polygon polygon = null;
            int count = 0;
            List<Point2D> points = new List<Point2D>();

            for (int c = 0; c < Sides.Length; c++)
            {
                Line side1 = Sides[c];

                if (!subP.IsInside(side1.P1)) //если точка снаружи добавляем ее в конец списка
                    points.Add(side1.P1);
                List<Point2D> temp = CrossAndOutsidePoint(c, subP,out count);
                if (temp.Count > 0)
                    points.AddRange(temp); //добавляем в конец списка цепочку точек пересечения и/или внешних точек

            }
            if (points.Count > 0) //если пересекаются образуем новый полигон, иначе возвращаем null
                polygon = new Polygon(points.ToArray());
            if (count == 0) 
                return null; // no cross
            return polygon;
        }
        private List<Point2D> CrossAndOutsidePoint(int idx, Polygon subP,out int count)
        {
            List<Point2D> result = new List<Point2D>();
            List<List<Point2D>> temp = new List<List<Point2D>>();
            Line side1 = Sides[idx];
            count = 0;
            for (int k = 0; k < subP.Sides.Length; k++)
            {
                Line side2 = subP.Sides[k];
                double t2 = Parametric.Get_T2(side1, side2);
                double t1 = Parametric.Get_T1(t2, side1, side2);
                if (Parametric.CrossPoint(side1, side2, out Point2D cross)) //проверка на пересечение
                {
                    temp.Add(new List<Point2D> { cross });  //новая цепочка точек                 
                    count = count + 1;
                    int n = k;

                    //цепочка внешних точек после пересечения
                    if (!IsInside(subP.Sides[k].P2)) //точка снаружи
                    {
                        if (!IsOtherCrossBetween(idx, temp[temp.Count - 1].First(), subP.Sides[k]))
                        { //нету точек пересечения с базовым многоугольником м/у ними у линии с пересечением
                            temp[temp.Count - 1].Add(subP.Sides[k].P2);
                            n = n >= subP.Nodes.Length - 1 ? 0 : n + 1; // меняем индекс
                            bool flag = !IsInside(subP.Sides[n].P2);
                            while (flag && n != k) //
                            {
                                if (!IsOtherCross(subP.Sides[n]))
                                { //нету точек пересечения с базовым многоугольником 
                                    temp[temp.Count - 1].Add(subP.Sides[n].P2);
                                    n = n >= subP.Nodes.Length - 1 ? 0 : n + 1; // меняем индекс
                                    flag = !IsInside(subP.Sides[n].P2);
                                }
                                else flag = false;
                            }
                        }
                    }
                }
            }
            if (temp.Count > 0) //количество пересечений
            {
                if (temp.Count > 1)
                    result = SortPoints(temp, side1.P1); // сортируем по расстоянию от начала базового отрезка
                else
                    result.AddRange(temp[0]);
            }
            return result;
        }
        private bool IsOtherCross(Line side)
        {
            foreach (var baseside in Sides)
            {
                if (Parametric.CrossOrNot(side, baseside))
                    return true;
            }
            return false;
        }
        private bool IsOtherCrossBetween(int bside, Point2D cross, Line side)
        {
            for (int k = 0; k < Sides.Length; k++)
            {
                if (k != bside)
                {
                    if (Parametric.CrossPoint(side, Sides[k], out Point2D point))
                    {
                        //лежит ли точка между точкой пересечения и концом линии
                        double d = cross.DistatnceTo(side.P2);
                        double len = cross.DistatnceTo(point) + point.DistatnceTo(side.P2);
                        return Math.Round(d,4) == Math.Round(len,4);
                    }

                }
            }
            return false;
        }
        public bool IsInside(Point2D p)
        {
            int count = 0; //счетчик пересечений            
            Point2D p1 = new Point2D(MaxX + 100, p.Y);            
            foreach (var side in Sides)
            {
                if (Parametric.IsRayCrossing(p, p1, side)) //пересекает ли луч отрезок
                    count++;

            }
            return count % 2 != 0; //нечетное число пересечений - точка внутри, четное снаружи
        }
        public override object Clone()
        {
            return new Polygon(Sides);
            
        }



    }
}
