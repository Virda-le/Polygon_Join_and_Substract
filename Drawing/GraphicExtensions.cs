using Drawing.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = Drawing.Entities.Point;

namespace Drawing
{
    public static class GraphicExtensions
    {
        public static float Height;
        public static float XScroll;
        public static float YScroll;
        public static float ScaleF;
        private static Pen select_pen = new Pen(Color.Aqua);
        public static void SetParametrs(this System.Drawing.Graphics g, float height,float xscroll,float yscroll,float scale)
        {
            Height = height;
            XScroll = xscroll;
            YScroll = yscroll;
            ScaleF = scale;
        }
        public static void SetTransforms(this System.Drawing.Graphics g)
        {
            g.PageUnit = System.Drawing.GraphicsUnit.Millimeter; // в миллиметры
            g.TranslateTransform(0, Height); //сдвигаем начало координат
            g.ScaleTransform(ScaleF, -ScaleF); //масштаб
            g.TranslateTransform(-XScroll/ScaleF, YScroll/ScaleF); //сдвиг
        }
        public static void DrawPoint(this System.Drawing.Graphics g, System.Drawing.Pen pen,Entities.Point point)
        {
            g.SetTransforms();
            System.Drawing.PointF p = point.Position.ToPoinF;
            g.DrawEllipse(pen,p.X-1,p.Y-1,2,2);
            g.ResetTransform();
        }
        public static void DrawLine(this System.Drawing.Graphics g, System.Drawing.Pen pen,Entities.Line line)
        {
            g.SetTransforms();           
            g.DrawLine(pen, line.P1.ToPoinF, line.P2.ToPoinF);
            g.ResetTransform();
        }
        public static void DrawPolygon(this System.Drawing.Graphics g, System.Drawing.Pen pen, Entities.Polygon polygon)
        {
            g.SetTransforms();
            PointF [] points = new PointF[polygon.Nodes.Length];
            for (int c = 0; c < polygon.Nodes.Length; c++)
                points[c] = polygon.Nodes[c].ToPoinF;
            if(!polygon.Selected)
                g.DrawPolygon(pen, points);
            else
                g.DrawPolygon(pen, points);
            g.ResetTransform();
        }
        public static void DrawEntity(this System.Drawing.Graphics g, System.Drawing.Pen pen, Entities.EntityObject entity)
        {
            switch(entity.Type)
            {
                case EntityType.Point:
                    g.DrawPoint(pen, entity as Point);
                    break;
                case EntityType.Line:
                    g.DrawLine(pen, entity as Line);
                    break;
                case EntityType.Polygon:
                    g.DrawPolygon(pen, entity as Polygon);
                    break;
            }
        }
    }
}
