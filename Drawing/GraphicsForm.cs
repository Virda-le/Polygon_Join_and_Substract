using System;
using System.Collections.Generic;
using Drawing.Methods;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Drawing.Entities;

namespace Drawing
{
    public partial class GraphicsForm : Form
    {
        #region VARIABLES
        private Point2D currentPosition;
        private Point2D first_point;


        //private List<Entities.Point> points = new List<Entities.Point>();
        //private List<Entities.Line> lines = new List<Entities.Line>();
        private List<Entities.Polygon> polygons = new List<Entities.Polygon>();
        private List<Entities.EntityObject> entity = new List<Entities.EntityObject>();
        //private List<Entities.EntityObject> selected = new List<Entities.EntityObject>();
        
        private List<Entities.Line> tempLines = new List<Entities.Line>();
        private List<Point2D> point3Ds = new List<Point2D>();
        

        private int DrawIdx = -1;
        private int ClickNum = 1;
        private bool active_drawing = false;
        
        private Pen penPoint = new Pen(Color.Red,0);
        private Pen penLine = new Pen(Color.Blue, 0.1f);
        private Pen posLine = new Pen(Color.Gray, 0.1f);

        private float ScaleF = 1.0f;
        private float XScroll;
        private float YScroll;
        #endregion

        //screen dpi
        private float DPI
        {
            get
            {
                using (var g = CreateGraphics())
                    return g.DpiX;
            }
        }
        public GraphicsForm()
        {
            InitializeComponent();
            //--------- доделать
            btnPoint.Enabled = false;
            btnLine.Enabled = false;
            //--------
            #region EVENTS
            drawing.MouseMove += Drawing_MouseMove;
            drawing.MouseDown += Drawing_MouseDown;
            
            drawing.Paint += Drawing_Paint;
            btnPoint.Click += BtnPoint_Click;
            btnLine.Click += BtnLine_Click;
            btnPolygon.Click += BtnPolygon_Click;
            btnJoin.Click += BtnJoin_Click;
            btnSubstract.Click += BtnSubstract_Click;
            btnClear.Click += BtnClear_Click;

            hScrollBar.Scroll += HScrollBar_Scroll;
            vScrollBar.Scroll += VScrollBar_Scroll;
            #endregion

            #region Context Menu
            drawing.ContextMenuStrip = ctxMenu;
            ToolStripMenuItem cancel_tool = new ToolStripMenuItem("Cancel");
            ToolStripMenuItem close_tool = new ToolStripMenuItem("Close");
            ctxMenu.Items.AddRange(new[] {cancel_tool,close_tool});            
            cancel_tool.Click += (o, e) => CancelAll();
            close_tool.Click += (o, e) =>
            {
                if(DrawIdx == 2)
                {
                    try
                    {
                        polygons.Add(new Polygon(point3Ds.ToArray()));
                        //entity.Add(new Polygon(point3Ds.ToArray()));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }                    
                    tempLines.Clear();
                    point3Ds.Clear();
                    ClickNum = 1;
                    drawing.Refresh();
                }
                
            };
            #endregion
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            DrawIdx = -1;
            ClickNum = 1;
            active_drawing = false;
            polygons.Clear();
            entity.Clear();
            tempLines.Clear();
            point3Ds.Clear();
        }
        #region Button
        private void BtnSubstract_Click(object sender, EventArgs e)
        {
            if (polygons.Count >= 2) 
            {
                Polygon p1 = polygons[0];           
                Polygon p2 = polygons[1];           
                Polygon res = p1.Substract(p2);
                if (res != null)
                {
                    polygons.Clear();
                    polygons.Add(res);
                    drawing.Refresh();
                    MessageBox.Show("Square = " + res.Square.ToString());
                }                    
            }
            else
                MessageBox.Show("Draw 2 polygon");
            
        }

        private void BtnJoin_Click(object sender, EventArgs e)
        {
            if (polygons.Count >= 2) //ls.Count >= 2)
            {
                Polygon p1 = polygons[0];           //(Polygon)ls[0];
                Polygon p2 = polygons[1];           //(Polygon)ls[1];
                Polygon res = p1.Join(p2);
                if (res != null)
                {
                    polygons.Clear();
                    polygons.Add(res);
                    drawing.Refresh();
                    MessageBox.Show("Square = " + res.Square.ToString());
                }
                    //entity.Add(res);
                
            }
            else
                MessageBox.Show("Draw 2 polygon");
            //if (selected.Count > 0)
            //{
            //    var ls = selected.Where(en => en.Type == EntityType.Polygon).ToList();

            //    if (ls.Count >= 2)
            //    {
            //        Polygon p1 = (Polygon)ls[0];
            //        Polygon p2 = (Polygon)ls[1];
            //        Polygon res = p1.Join(p2);
            //        if (res != null)
            //            entity.Add(res);
            //    }
            //    else
            //        MessageBox.Show("Select 2 polygon");
            //}
        }

        private void BtnPolygon_Click(object sender, EventArgs e)
        {
            DrawIdx = 2;            
            active_drawing = true;
            drawing.Cursor = Cursors.Cross;
        }
        private void BtnLine_Click(object sender, EventArgs e)
        {
            DrawIdx = 1;
            active_drawing = true;
            drawing.Cursor = Cursors.Cross;

        }

        private void BtnPoint_Click(object sender, EventArgs e)
        {
            DrawIdx = 0;
            active_drawing = true;
            drawing.Cursor = Cursors.Cross;
        }
        #endregion
        #region Mouse
        private void Drawing_MouseMove(object sender, MouseEventArgs e)
        {
            currentPosition = ConvertPointPointF(e.Location);            
            statusLabel.Text = String.Format("X = {0,0:F4}  Y = {1,0:F4}", currentPosition.X, currentPosition.Y);
            drawing.Refresh();
        }
        private void Drawing_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (active_drawing)
                {
                    switch (DrawIdx)
                    {
                        //case -1: SelectEntity();
                        //    break;
                        case 0:
                            entity.Add(new Entities.Point(currentPosition));
                            break;
                        case 1:
                            switch (ClickNum)
                            {
                                case 1:
                                    first_point = currentPosition;
                                    entity.Add(new Entities.Point(currentPosition));
                                    ClickNum++;
                                    break;
                                case 2:
                                    entity.Add(new Entities.Line(first_point, currentPosition));
                                    entity.Add(new Entities.Point(currentPosition));
                                    first_point = currentPosition;
                                    
                                    break;
                            }
                            break;
                        case 2:
                            if (ClickNum > 1)
                            {
                                tempLines.Add(new Entities.Line(first_point, currentPosition));
                                point3Ds.Add(currentPosition);
                                first_point = currentPosition;
                            }
                            else
                            {
                                point3Ds.Add(currentPosition);
                                first_point = currentPosition;
                            }
                            ClickNum++;                            
                            break;
                    }
                    drawing.Refresh();
                }
            }
        }
        #endregion
        #region Methods
        //drawing
        private void Drawing_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SetParametrs(PixelToMillimiters(drawing.Height), XScroll, YScroll, ScaleF);
            //draw all
            if (entity.Count > 0)
            {
                foreach (var obj in entity)
                    e.Graphics.DrawEntity(penPoint, obj);
            }            
            if(tempLines.Count>0)
            {
                foreach(Entities.Line line in tempLines)
                    e.Graphics.DrawLine(penLine, line);
            }
            if(polygons.Count > 0)
            {
                foreach(var polygon in polygons)
                    e.Graphics.DrawPolygon(penLine, polygon);
            }
            
            switch (DrawIdx)
            {
                case 1:
                    if (ClickNum == 2)
                    {
                        Entities.Line line = new Entities.Line(first_point, currentPosition);
                        e.Graphics.DrawLine(posLine, line);
                    }
                    break;
                case 2:
                    if (ClickNum > 1)
                    {
                        Entities.Line line = new Entities.Line(first_point, currentPosition);
                        e.Graphics.DrawLine(posLine, line);
                    }
                    break;

            }
            // add crossing line
            //if (entity.Count > 0)
            //{
            //    foreach (Entities.Line line1 in entity)
            //    {
            //        foreach (Entities.Line line2 in entity)
            //        {
            //            if (Parametric.CrossPoint(line1, line2, out Point3D point))
            //                entity.Add(new Entities.Point(point));
            //        }
            //    }
            //}

        }
        
         
        //cancel
        private void CancelAll()
        {
            DrawIdx = -1;
            active_drawing = false;
            ClickNum = 1;
            drawing.Cursor = Cursors.Default;
            tempLines.Clear();
            point3Ds.Clear();
        }
        //convert picture system to float (world system)
        //-PointToCaseian-
        private Point2D ConvertPointPointF(System.Drawing.Point point)
            => new Point2D(PixelToMillimiters(point.X + XScroll)/ScaleF, (PixelToMillimiters(drawing.Height- point.Y)-YScroll)/ScaleF);
        //convert pixel to millimeters
        // -Pixel_to_Mn
        private float PixelToMillimiters(float pixel) => pixel * 25.4f / DPI;
        private void VScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            YScroll = (sender as VScrollBar).Value;
            drawing.Refresh();
        }

        private void HScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            XScroll = (sender as HScrollBar).Value;
            drawing.Refresh();
        }

        #endregion


    }
}
