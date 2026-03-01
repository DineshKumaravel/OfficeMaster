using NHotkey;
using NHotkey.WindowsForms;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OfficeHelper
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Hidden form to keep message loop alive
            var form = new Form();
            form.ShowInTaskbar = false;
            form.WindowState = FormWindowState.Minimized;
            form.Load += (s, e) => form.Hide();

            HotKeyHandler.Initialize();

            DotNetEnv.Env.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Environment.env"));

            if (!EventHandler.CreateDB()) { Application.Exit(); }

            var status = EventHandler.CreateCloudRecords();

            if (status)
                Application.Run();
            else
                Application.Exit();
        }
    }
}
