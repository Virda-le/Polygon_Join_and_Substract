using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drawing
{
    public class Point2D //vector3
    {
        public double X { get; set; }
        public double Y { get; set; }
        //public double Z { get; set; }
        
        public Point2D(double x, double y)
        {
            X = x; Y = y;  
        }
        //public Point2D(double x, double y, double z) : this(x, y)
        //{
        //    Z = z;
        //}
        public System.Drawing.PointF ToPoinF
        {
            get { return new System.Drawing.PointF((float)X,(float)Y); }
        }
        public double DistatnceTo(Point2D p)
        {
            return Math.Sqrt(Math.Pow(X - p.X, 2) + Math.Pow(Y - p.Y, 2));
        }
        public static Point2D Zero
        {
            get
            { return new Point2D(0.0, 0.0); }
        }
        

        
    }
}
