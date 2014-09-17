using System;
using System.Drawing;
namespace FlowChartDemo
{
    interface IPaintItem
    {
        System.Drawing.Image ItemImage { get; set; }
        System.Drawing.Point ItemLocate { get; set; }
        string ItemName { get; set; }
        ItemStatus ItemStatus { get; set; }
 
        Font ItemFont { get; set; }

        void DrawSelf(Graphics grp,Pen pen);
    }
}
