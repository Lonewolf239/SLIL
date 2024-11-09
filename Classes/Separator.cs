namespace SLIL.Classes
{
    public class Separator : System.Windows.Forms.Panel
    {
        public Separator()
        {
            Cursor = Program.SLILCursor;
            Dock = System.Windows.Forms.DockStyle.Top;
            BackColor = System.Drawing.Color.White;
            Height = 2;
        }
    }
}