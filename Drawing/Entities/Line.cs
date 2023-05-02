using System;
using Drawing.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drawing.Entities
{
    public class Line: EntityObject
    {
        public Point2D P1 { get; set; }
        public Point2D P2 { get; set; }
        public double Thickness { get; set; }
        public Vector2D Vector
        {
            get
            {
                return new Vector2D(P1, P2);
            }
        }
        public double Lenght
        {
            get
            {
                return Math.Sqrt(Math.Pow(P1.X - P2.X, 2)
                    + Math.Pow(P1.Y - P2.Y, 2));
            }
        }
        public Line():this(Point2D.Zero,Point2D.Zero)
        {
            Thickness = 0.0;
        }
        public Line(Point2D p1, Point2D p2)
            :base(EntityType.Line)
        {
            P1 = p1;
            P2 = p2;
            Thickness = 0.0;
        }
        public override object Clone()
        {
            return new Line
            {
                P1 = this.P1,
                P2 = this.P2,
                Thickness = this.Thickness,
                Visible = this.Visible

            };
        }
    }
}
