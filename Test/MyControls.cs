using System.Drawing;
using System.Windows.Forms;

namespace MyControls
{
    public class MyPanel : Panel
    {

        public MyPanel()
        {

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);

            DrawBorder(g, rect);
        }

        public void DrawBorder(Graphics graph, Rectangle rect)
        {
            graph.DrawRectangle(new Pen(Color.FromArgb(42, 71, 94)), rect);
        }
    }

    public class MyCloseButton : Button
    {

        public MyCloseButton()
        {

        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            Graphics g = pevent.Graphics;
            Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);

            DrawBorder(g, rect);
            DrawX(g);
        }

        public void DrawBorder(Graphics graph, Rectangle rect)
        {
            graph.DrawRectangle(new Pen(Color.FromArgb(42, 71, 94)), rect);
        }

        public void DrawX(Graphics graph)
        {
            graph.DrawLine(new Pen(Color.FromArgb(42, 71, 94), 2), new Point(6, 6), new Point(18, 18));
            graph.DrawLine(new Pen(Color.FromArgb(42, 71, 94), 2), new Point(18, 6), new Point(6, 18));
        }
    }

    public class MyListView : ListView
    {
        public MyListView()
        {
            //Activate double buffering
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            // Windows messages before they get to the form's WndProc
            this.SetStyle(ControlStyles.EnableNotifyMessage, true);
        }

        protected override void OnNotifyMessage(Message m)
        {
            //Filter out the WM_ERASEBKGND message
            if (m.Msg != 0x14)
            {
                base.OnNotifyMessage(m);
            }
        }
    }
}
