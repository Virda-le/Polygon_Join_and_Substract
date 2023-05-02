using System;
using Drawing.Entities;
using Drawing.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Drawing.Methods
{
    public static class Parametric
    {
        /// <summary>
        /// Checks if vectors of lines parallel
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool IsParallel(Vector2D a, Vector2D b)
        {
            return a.X * b.Y == b.X * a.Y;

            //if(Math.Round(a.X,4) == 0) // first vector on Y
            //{
            //    if (Math.Round(b.X, 4) == 0) //second vector on Y
            //    {
            //        // if v1/w1 == v2/w2, so a = kb
            //        double k = Math.Max(a.Y, b.Y) / Math.Min(a.Y, b.Y);
            //        return Math.Min(a.Y,b.Y)*k == Math.Max(a.Y,b.Y);
            //    }
            //    else 
            //        return false;   //isn't parallel cause of different angle                 
            //}
            //else if(Math.Round(a.Y, 4) == 0) //on X
            //{
            //    if (Math.Round(b.Y, 4) == 0)
            //    {
            //        double k = Math.Max(a.X, b.X) / Math.Min(a.X, b.X);
            //        return Math.Min(a.X, b.X) * k == Math.Max(a.X, b.X);
            //    }
            //    else
            //        return false;
            //}
            //else
            //{
            //    // if v1/w1 == v2/w2 so they are parallel
            //    return Math.Round(a.X/b.X,4) == Math.Round(a.Y/b.Y,4);
            //} 
        }
        /// <summary>
        /// Return true if vector is larger or less than zero, otherwise false
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        private static bool IsLine(Vector2D a)
        {
            //if a(0;0) it isn't line
            if (a.X == 0 && a.Y == 0)
                return false;
            else
                return true;
        }
        /// <summary>
        /// One coordinate of each line
        /// Calculate k for line
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double Get_T2(Line side1, Line side2)
        {
            //коэфициент для второго уравнения(р1 - точка начала прямой)

            double t2 = (side1.Vector.X * side2.P1.Y - side1.Vector.X * side1.P1.Y
                        - side1.Vector.Y * side2.P1.X + side1.Vector.Y * side1.P1.X)
                        / (side1.Vector.Y * side2.Vector.X - side1.Vector.X * side2.Vector.Y);
            return t2;
        }
        /* 
         * параметрическое уравнение прямой
             * x1 = x1+v1t      x1+v1t=x2+v2t    
             * y1 = y1+w1t     y1+w1t=y2+w2t2
             * x2 = x2+v2t2
             * y2 = y2+w2t2
             * t2 = (v1y2 -v1y1 -w1x2 +w1x1)/(w1v2 - v1w2) ---*Get_T2*---
             * t1 =(y2 - y1 + w2t2)/w1 = (x2 - x1 + v2t2)/v1 ---*Get_T1*---
             *  0 <= t <= 1 -line crossing ---*IsCrossing*---
        */
        /// <summary>
        /// One coordinate of each line
        /// Calculate k2 for line
        /// </summary>
        /// <param name="t"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double Get_T1(double t, Line side1, Line side2)
        {
            //коэфициент для первого уравнения(p0 - точка начала прямой)
            double t1;
            if (side1.Vector.X == 0)
                t1 = (side2.P1.Y - side1.P1.Y + side2.Vector.Y * t) / side1.Vector.Y;
            else
                t1 = (side2.P1.X - side1.P1.X + side2.Vector.X * t) / side1.Vector.X;
            return t1;
        }
        public static bool IsCrossing(double t)
        {
            return t <= 1 && t >= 0;
        }
        public static bool IsRayCross(double t)
        {
            return t >= 0;
        }
        //пересечение отрезков
        public static bool CrossPoint(Line side1, Line side2, out Point2D cross)
        {
            cross = Point2D.Zero;
            double x1, y1, x2, y2, t1, t2;
            if (MayCross(side1.Vector, side2.Vector))
            {
                t2 = Get_T2(side1, side2);
                t1 = Get_T1(t2, side1, side2);
                if (IsCrossing(t1) && IsCrossing(t2))
                {
                    //x = p0.X + a.X * t;
                    x1 = Math.Round((side1.P1.X + side1.Vector.X * t1), 4);
                    //y = p0.Y + a.Y * t;
                    y1 = Math.Round((side1.P1.Y + side1.Vector.Y * t1), 4);
                    //x1 = p1.X + b.X * t1;
                    x2 = Math.Round((side2.P1.X + side2.Vector.X * t2), 4);
                    ////y2 = p1.Y + b.Y * t1;
                    y2 = Math.Round((side2.P1.Y + side2.Vector.Y * t2), 4);
                    cross = new Point2D(x1, y1);
                    return x1 == x2 && y1 == y2;

                }
            }
            return false;
        }
        public static bool IsRayCrossing(Point2D p, Point2D p1, Line side)
        {
            //Vector2D v = new Vector2D(p, p1);

            double t2 = Get_T2(side, new Line(p, p1)); //коэфициент для луча
            double t1 = Get_T1(t2, side, new Line(p, p1)); //коэфициент для прямой(side)
            return (IsCrossing(t1) && IsRayCross(t2));
        }
        public static bool CrossOrNot(Line side1, Line side2)
        {
            double t2 = Get_T2(side1, side2);
            double t1 = Get_T1(t2, side1, side2);
            return IsCrossing(t1) && IsCrossing(t2);
        }
        public static bool MayCross(Vector2D a, Vector2D b) //проверка на параллельность и являются ли линиями
        {
            //check lines and if they are parallel
            if (Parametric.IsLine(a) && Parametric.IsLine(b))
                if (Parametric.IsParallel(a, b))
                    return false;
                else
                    return true;
            return false;

        }
    }
}
