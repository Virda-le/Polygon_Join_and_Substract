using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drawing.Entities
{
    public class Point: EntityObject
    {
        public Point2D Position { get; set; }
        public double Thickness { get; set; }
        public Point():base(EntityType.Point)
        {
            Position = Point2D.Zero;
            Thickness = 0.0;
        }
        public Point(Point2D position):base(EntityType.Point)
        {
            Position = position;
            Thickness = 0.0;
        }
        public override object Clone()
        {
            return new Point(this.Position);
        }
    }
}
