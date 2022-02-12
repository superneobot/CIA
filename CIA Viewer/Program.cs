using System;
using System.Windows.Forms;

namespace CIA_Viewer
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Viewer Main;

            if (args != null && args.Length > 0)
            {
                Main = new Viewer(args[0]);
            }
            else
            {
                Main = new Viewer();
            }

            Application.Run(Main);
        }
    }
}
