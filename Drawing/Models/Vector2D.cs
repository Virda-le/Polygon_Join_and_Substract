using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drawing.Models
{
    public class Vector2D
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        //public double Z { get; private set; }
        /// <summary>
        /// Takes vectors value
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vector2D(double x, double y)
        {
            X = x; Y = y; 
        }
        //public Vector2D(double x, double y, double z) : this(x, y)
        //{
        //    Z = z;
        //}

        /// <summary>
        /// Takes line's coordinates and sets vectors value
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public Vector2D(Point2D a, Point2D b)
        {
            this.X = b.X - a.X;
            this.Y = b.Y - a.Y;
            
        }
        /// <summary>
        /// Change Vectors value
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public void ChangeVector2D(Point2D a, Point2D b)
        {
            X = b.X - a.X;
            Y = b.Y - a.Y;
            
        }

    }
}
