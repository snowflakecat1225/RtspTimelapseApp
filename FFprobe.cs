using System.Diagnostics;

namespace RTSP_Timelapse_App
{
    public class FFprobe
    {
        private static string appPath = @"/usr/bin/ffprobe";
        public static string AppPath
        { 
            get { return appPath; }
            set { appPath = value; }
        }

        private static readonly ProcessStartInfo startInfo = new()
        {
            FileName = AppPath,
            RedirectStandardOutput = true
        };

        private static string ProcessResult(ProcessStartInfo startInfo)
        {
            var process = Process.Start(startInfo);
            string result = string.Empty;
            if (process != null)
            {
                result = process.StandardOutput.ReadToEnd().Replace("\n", "");
                process.WaitForExit();
            }
            return result;
        }

        public class GetInfo
        {
            public static string All(string videoPath)
            {
                startInfo.Arguments = "-rtsp_transport tcp " + videoPath;
                return ProcessResult(startInfo);
            }

            public static int Duration(string videoPath)
            {
                startInfo.Arguments = "-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 " + videoPath;
                try { return int.Parse(ProcessResult(startInfo).Split('.')[0]); }
                catch { return 1; }
            }

            public static int Framerate(string videoPath)
            {
                startInfo.Arguments = "-rtsp_transport tcp -v error -select_streams v -of default=noprint_wrappers=1:nokey=1 -show_entries stream=r_frame_rate " + videoPath;
                var value = ProcessResult(startInfo).Replace("/1", "");
                if (value.Split('/').Length == 2)
                {
                    bool isNumber = true;
                    foreach (char ch in value.Split('/')[0])
                        if (!char.IsDigit(ch))
                        {
                            isNumber = false;
                            break;
                        }
                    if (isNumber)
                        foreach (char ch in value.Split('/')[1])
                            if (!char.IsDigit(ch))
                            {
                                isNumber = false;
                                break;
                            }
                    if (isNumber)
                        return int.Parse(value.Split('/')[0])/int.Parse(value.Split('/')[1]);
                }
                return int.Parse(FFmpegArguments.Default.Framerate);
            }

            public static double VideoBitrate(string videoPath)
            {
                startInfo.Arguments = "-rtsp_transport tcp -v error -select_streams v -of default=noprint_wrappers=1:nokey=1 -show_entries stream=bit_rate " + videoPath;
                try { return int.Parse(ProcessResult(startInfo)); }
                catch { return double.Parse(FFmpegArguments.Default.VideoBitrate); }
            }

            public static double AudioBitrate(string videoPath)
            {
                startInfo.Arguments = "-rtsp_transport tcp -v error -select_streams a -of default=noprint_wrappers=1:nokey=1 -show_entries stream=bit_rate " + videoPath;
                try { return int.Parse(ProcessResult(startInfo)); }
                catch { return double.Parse(FFmpegArguments.Default.AudioBitrate); }
            }

            public static string Resolution(string videoPath)
            {
                startInfo.Arguments = "-rtsp_transport tcp -v error -select_streams v -show_entries stream=width,height -of csv=s=x:p=0 " + videoPath;
                try { return ProcessResult(startInfo); }
                catch { return FFmpegArguments.Default.Resolution; }
            }

            public static string VideoCodec(string videoPath)
            {
                startInfo.Arguments = "-rtsp_transport tcp -v error -select_streams v -of default=noprint_wrappers=1:nokey=1 -show_entries stream=codec_name " + videoPath;
                try { return ProcessResult(startInfo); }
                catch { return FFmpegArguments.Default.VideoCodec; }
            }

            public static string AudioCodec(string videoPath)
            {
                startInfo.Arguments = "-rtsp_transport tcp -v error -select_streams a -of default=noprint_wrappers=1:nokey=1 -show_entries stream=codec_name " + videoPath;
                try { return ProcessResult(startInfo); }
                catch { return FFmpegArguments.Default.AudioCodec; }
            }

            public static string PixelFormat(string videoPath)
            {
                startInfo.Arguments = "-rtsp_transport tcp -v error -select_streams v -of default=noprint_wrappers=1:nokey=1 -show_entries stream=pix_fmt " + videoPath;
                try { return ProcessResult(startInfo); }
                catch { return FFmpegArguments.Default.PixelFormat; }
            }
        }
    }
}