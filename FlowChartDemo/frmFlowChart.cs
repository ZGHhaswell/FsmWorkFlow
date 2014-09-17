using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace FlowChartDemo
{
    public partial class frmFlowChart : Form
    {
        public frmFlowChart()
        {
            InitializeComponent();
            this.toolStripButton1.AllowDrop = true;
            this.toolStripButton2.AllowDrop = true;
            this.toolStripButton3.AllowDrop = true;

        }
        string m_itemtype = string.Empty;

        private IPaintItem m_currentitem = null;
        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void paintPanle1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            IPaintItem item = this.paintPanle1.GetItemAtPoint(e.Location);
            if (item != null && item is PaintUnit )
            {
                string itemname = Microsoft.VisualBasic.Interaction.InputBox("请输入新名称", "提示", "未命名", 100, 100);
                item.ItemName = itemname ;
            }
        }

      

        private void paintPanle1_DragDrop(object sender, DragEventArgs e)
        {
            string itemtype = e.Data.GetData(typeof(string)).ToString();
            if (itemtype == "A")
            {
                Point lefttoppoint = this.PointToScreen(this.paintPanle1.Location);
                PaintUnit item = new PaintUnit("未命名", FlowChartDemo.Properties.Resources.ContactFlow,
                    ItemStatus.Actived,
                    new Point(e.X - lefttoppoint.X - FlowChartDemo.Properties.Resources.ContactFlow.Width / 2,
                        e.Y - lefttoppoint.Y - FlowChartDemo.Properties.Resources.ContactFlow.Height / 2), new Pen(Color.Gray), this.Font);
                this.paintPanle1.PaintItems.Add(item);
                this.paintPanle1.Refresh();
            }
            else if (itemtype == "B")
            {
                Point lefttoppoint = this.PointToScreen(this.paintPanle1.Location);
                PaintUnit item = new PaintUnit("未命名", FlowChartDemo.Properties.Resources.GMail,
                    ItemStatus.Actived,
                    new Point(e.X - lefttoppoint.X - FlowChartDemo.Properties.Resources.GMail.Width / 2,
                        e.Y - lefttoppoint.Y - FlowChartDemo.Properties.Resources.GMail.Height / 2), new Pen(Color.Gray), this.Font);
                this.paintPanle1.PaintItems.Add(item);
                this.paintPanle1.Refresh();
            }
            else if (itemtype == "C")
            {
                Point lefttoppoint = this.PointToScreen(this.paintPanle1.Location);
                PaintUnit item = new PaintUnit("未命名", FlowChartDemo.Properties.Resources.Phone,
                    ItemStatus.Actived,
                    new Point(e.X - lefttoppoint.X - FlowChartDemo.Properties.Resources.Phone.Width / 2,
                        e.Y - lefttoppoint.Y - FlowChartDemo.Properties.Resources.Phone.Height / 2), new Pen(Color.Gray), this.Font);
                this.paintPanle1.PaintItems.Add(item);
                this.paintPanle1.Refresh();
            }
        }


        private void paintPanle1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Link;
            //m_itemtype = e.Data.GetData(typeof(string)).ToString();
        }

        private void toolStripButton1_MouseDown(object sender, MouseEventArgs e)
        {
            toolStrip1.DoDragDrop("A", DragDropEffects.Link);
        }

        private void toolStripButton2_MouseDown(object sender, MouseEventArgs e)
        {
            toolStrip1.DoDragDrop("B", DragDropEffects.Link);
        }

        private void toolStripButton3_MouseDown(object sender, MouseEventArgs e)
        {
            toolStrip1.DoDragDrop("C", DragDropEffects.Link);
        }
        private void 保存SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void 打开OToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Open();
        }
        private void Save()
        {
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                SerializeObjectToFile(this.saveFileDialog1.FileName);
            }
        }
        private void Open()
        {
            this.paintPanle1.PaintItems.Clear();
            this.paintPanle1.Refresh();
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.paintPanle1.PaintItems = UnSerializeObjectFromFile(this.openFileDialog1 .FileName);
                this.paintPanle1.Refresh();
            }
        }


        /// <summary>
        /// 从本地存储反序列化业务对象
        /// </summary>
        /// <param name="storefilename"></param>
        /// <returns></returns>
        internal void SerializeObjectToFile(string storefilename)
        {
            IFormatter formatter = new BinaryFormatter();


            // To write to a file, create a StreamWriter object.
            Stream writer = new FileStream(storefilename, FileMode.Create); //new StreamWriter( new IsolatedStorageFileStream(string.Concat(@"OBU\", obj.OffFileName), FileMode.OpenOrCreate, isoStore);

            formatter.Serialize(writer, paintPanle1.PaintItems);

            byte[] objbuffer = new byte[writer.Length];
            writer.Seek(0, SeekOrigin.Begin);
            writer.Read(objbuffer, 0, objbuffer.Length);
            writer.Close();
        }

        internal List<IPaintItem> UnSerializeObjectFromFile(string storefilename)
        {
            List<IPaintItem> offlineobject = null;

            IFormatter formatter = new BinaryFormatter();

            Stream writer = new FileStream(storefilename, FileMode.Open);// new IsolatedStorageFileStream(string.Concat(@"OBU\", storefilename), FileMode.OpenOrCreate, isoStore);
            writer.Seek(0, SeekOrigin.Begin);
            offlineobject = (List<IPaintItem>)formatter.Deserialize(writer);

            writer.Close();
            return offlineobject;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            this.toolStripButton4.Checked = !this.toolStripButton4.Checked;
        }

        private void paintPanle1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                IPaintItem selectedunit = this.paintPanle1.GetItemAtPoint(e.Location) ;
                if(selectedunit != null && selectedunit is PaintUnit )
                {
                    if (toolStripButton4.Checked)
                    {
                        PaintLink link = new PaintLink("", ItemStatus.Actived, e.Location, new Pen(Color.Black), this.Font);
                        link.StartPoint = (PaintUnit)selectedunit;
                        m_currentitem = link;
                        paintPanle1.DrawingLink = true;
                    }
                }
            }
        }

        private void paintPanle1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (toolStripButton4.Checked && (m_currentitem != null && m_currentitem is PaintLink))
                {
                    IPaintItem selectedunit = this.paintPanle1.GetItemAtPoint(e.Location);
                    if (selectedunit != null && selectedunit is PaintUnit)
                    {
                        ((PaintLink)m_currentitem).EndPoint = (PaintUnit)selectedunit;
                        ((PaintLink)m_currentitem).ItemStatus = ItemStatus.Finished;
                        this.paintPanle1.PaintItems.Add(m_currentitem);
                    }
                    this.toolStripButton4.Checked = false;
                    m_currentitem = null;


                }
                paintPanle1.DrawingLink = false;

            }
        }

        private void 退出XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }



       
    }
}
