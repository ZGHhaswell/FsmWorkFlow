using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

namespace FlowChartDemo
{
    public  class PaintPanel:Control
    {
        private int m_gridspace=10;
        private bool m_showgrid = true;
        private Color m_gridcolor = Color.Blue;
        private List<IPaintItem> m_painttiems = new List<IPaintItem>();

        private IPaintItem m_ClickedItem = null;

        private Pen m_pointedBorderPen = new Pen(Color.Gray);
        private Point m_PointInItem;
        internal List<IPaintItem> PaintItems
        {
            get { return m_painttiems; }
            set
            {
                m_painttiems = value;
                Refresh();
            }
        }
        private Bitmap buffermap = null;
        private Graphics buffergrp = null;
        //[DefaultValueAttribute(Color.Blue)]
        public Color GridColor
        {
            get { return m_gridcolor; }
            set { m_gridcolor = value; }
        }

        [DefaultValueAttribute(true)]
        public bool ShowGrid
        {
            get { return m_showgrid; }
            set
            {
                m_showgrid = value;
                Refresh();
            }
        }

        [DefaultValueAttribute(10)]
        public int GridSpace
        {
            get { return m_gridspace; }
            set
            {
                m_gridspace = value;
                Refresh();
            }
        }

        private bool m_drawingLink = false;

        public bool DrawingLink
        {
            get { return m_drawingLink; }
            set { m_drawingLink = value; }
        }

        public PaintPanel()
        {
            Graphics gp = Graphics.FromImage(DefaultImage);
            gp.DrawEllipse(new Pen(Color.Red), 0,0,DefaultImage.Width, DefaultImage.Height);
            buffermap = new Bitmap(100, 100);
            buffergrp = Graphics.FromImage(buffermap);
        }

        private Image DefaultImage = new Bitmap(20, 20);
        private Pen m_anchorpen = new Pen(Color.Gray);

        protected override void OnPaint(PaintEventArgs e)
        {
           buffergrp.Clear(this.BackColor);
            DrawGird();
            DrawItems();
            e.Graphics.DrawImage(buffermap,new Point(0,0));

        }
        protected override void OnSizeChanged(EventArgs e)
        {
            if (this.Width== 0 &&this.Height == 0)
            {
            }
            else
            {
                buffermap = new Bitmap(this.Width, this.Height);
                buffergrp = Graphics.FromImage(buffermap);
                base.OnSizeChanged(e);
            }
        }

        private void DrawGird()
        {
            if (this.ShowGrid)
            {
                Pen gridpen = new System.Drawing.Pen(m_gridcolor);
                for (int column = 0; column < (int)Math.Ceiling((double)this.Width / (double)m_gridspace); column++)
                {
                    buffergrp.DrawLine(gridpen, new Point(column * m_gridspace, 0), new Point(column * m_gridspace, this.Height));
                }
                for (int row = 0; row < (int)Math.Ceiling((double)this.Height / (double)m_gridspace); row++)
                {
                    buffergrp.DrawLine(gridpen, new Point(0, row * m_gridspace), new Point(this.Width, row * m_gridspace));
                }
            }
        }

        private void DrawItems()
        {
            foreach (IPaintItem item in m_painttiems)
            {
                if (item.ItemImage == null)
                {
                    item.ItemImage = DefaultImage;
                }
                item.DrawSelf(buffergrp, m_pointedBorderPen);
            }
        }

       
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                m_ClickedItem = GetItemAtPoint(e.Location);
                if (m_ClickedItem != null)
                {
                    m_PointInItem = new Point(e.X - m_ClickedItem.ItemLocate.X, e.Y - m_ClickedItem.ItemLocate.Y);
                }
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                m_ClickedItem = null;
            }
            base.OnMouseUp(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!m_drawingLink)
                {
                    if (m_ClickedItem != null)
                    {
                        m_ClickedItem.ItemLocate = new Point(e.Location.X - m_PointInItem.X, e.Location.Y - m_PointInItem.Y);
                        Refresh();
                    }
                }
            }
            else
            {
                m_ClickedItem = GetItemAtPoint(e.Location);
                if (m_ClickedItem != null)
                {
                    m_ClickedItem.ItemStatus = ItemStatus.Pointed;
                }
                else
                {
                    foreach (IPaintItem item in m_painttiems)
                    {
                        item.ItemStatus = ItemStatus.Watting;
                    }
                }
                this.Refresh();

            }
        }
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            
        }

        internal IPaintItem GetItemAtPoint(Point point)
        {
            foreach (IPaintItem item in m_painttiems)
            {
                if ((item.ItemLocate.X < point.X && (item.ItemLocate.X + item.ItemImage.Width) > point.X)
                    && (item.ItemLocate.Y < point.Y && (item.ItemLocate.Y + item.ItemImage.Height) > point.Y))
                {
                    return item;
                }
            }
            return null;
        }
    }
    [Flags]
    public  enum ItemStatus 
    {
        Finished = 1 ,
        Actived =2 ,
        Watting = 4,
        Pointed = 8 

    }

    [Serializable]
    class PaintUnit : FlowChartDemo.IPaintItem
    {
        private string m_itemname = "未命名";

        public string ItemName
        {
            get { return m_itemname; }
            set { m_itemname = value; }
        }
        private Image m_itemImage = null;

        public Image ItemImage
        {
            get { return m_itemImage; }
            set { m_itemImage = value; }
        }
        private ItemStatus m_itemstatus;

        public ItemStatus ItemStatus
        {
            get { return m_itemstatus; }
            set { m_itemstatus = value; }
        }
        private Point m_itemlocate;

        public Point ItemLocate
        {
            get { return m_itemlocate; }
            set { m_itemlocate = value; }
        }


        private Font m_itemFont;

        public Font ItemFont
        {
            get { return m_itemFont; }
            set { m_itemFont = value; }
        }

        public PaintUnit()
        {
        }
        public PaintUnit(string itemname, Image itemImage, ItemStatus itemstatus, Point itemlocate,Pen itemPen,Font itemFont)
        {
            m_itemname = itemname;
            m_itemImage = itemImage;
            m_itemstatus = itemstatus;
            m_itemlocate = itemlocate;
            m_itemFont = itemFont;
        }
        public void DrawSelf(Graphics grp,Pen pen)
        {
            grp.DrawImage(ItemImage, ItemLocate);
            SizeF namesize = grp.MeasureString(ItemName, this.ItemFont);
            PointF namepoint = new PointF();
            namepoint.Y = ItemLocate.Y + ItemImage.Height + 5;
            namepoint.X = ItemLocate.X + (ItemImage.Width - namesize.Width) / 2;
            grp.DrawString(ItemName, this.ItemFont, new SolidBrush(Color.Black), namepoint);
            if ((ItemStatus & ItemStatus.Pointed) == ItemStatus.Pointed)
            {
                grp.DrawRectangle(pen, ItemLocate.X - 3, ItemLocate.Y - 3, ItemImage.Width + 6, ItemImage.Height + 6);

            }

        }
    }

    [Serializable]
    public class PaintLink : FlowChartDemo.IPaintItem
    {
         private string m_itemname = "未命名";

        public string ItemName
        {
            get { return m_itemname; }
            set { m_itemname = value; }
        }
        private Image m_itemImage = null;

        public Image ItemImage
        {
            get { return m_itemImage; }
            set { m_itemImage = value; }
        }
        private ItemStatus m_itemstatus;

        public ItemStatus ItemStatus
        {
            get { return m_itemstatus; }
            set { m_itemstatus = value; }
        }
        private Point m_itemlocate;

        public Point ItemLocate
        {
            get { return m_itemlocate; }
            set { m_itemlocate = value; }
        }

        private Font m_itemFont;

        public Font ItemFont
        {
            get { return m_itemFont; }
            set { m_itemFont = value; }
        }

        private PaintUnit m_startPoint;

        internal PaintUnit StartPoint
        {
            get { return m_startPoint; }
            set { m_startPoint = value; }
        }

        private PaintUnit m_endPoint;

        internal PaintUnit EndPoint
        {
            get { return m_endPoint; }
            set { m_endPoint = value; }
        }

        public PaintLink()
        {
        }

       
        public PaintLink(string itemname, ItemStatus itemstatus, Point itemlocate, Pen itemPen, Font itemFont)
        {
            m_itemname = itemname;
            m_itemstatus = itemstatus;
            m_itemlocate = itemlocate;
            m_itemFont = itemFont;
        }

        public void DrawSelf(Graphics grp,Pen pen)
        {
            if (m_startPoint != null && m_endPoint != null)
            {

                grp.DrawLine(pen, m_startPoint.ItemLocate, m_endPoint.ItemLocate);
            }

        }
    }

    
}
