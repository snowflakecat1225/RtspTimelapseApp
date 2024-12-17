using System;
using System.IO;

namespace RTSP_Timelapse_App
{
    public class Temp
    {
        private static readonly string tempPath = $"/home/{Environment.UserName}/.cache/timelaps_app";
        public static string Path { get { return tempPath; } }
        
        public static void Create()
        {
            if (!Directory.Exists(Path))
                Directory.CreateDirectory(Path);
        }

        public static void Delete()
        {
            try
            {  
                while (File.Exists(Directory.GetFiles(Path)[0]))
                    File.Delete(Directory.GetFiles(Path)[0]);
            }
            catch {}
            Directory.Delete(Path);
        }
    }
}