using System;

namespace RTSP_Timelapse_App
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Length == 0)
                new MainWindow();
            else if (args.Length == 3)
                Script.Run(args);
        }
    }
}
