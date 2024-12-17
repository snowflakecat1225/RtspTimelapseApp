using System.Diagnostics;

namespace RTSP_Timelapse_App
{
    public class FFmpeg
    {
        private static string appPath = @"/usr/bin/ffmpeg";
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

        public static void Execute(string inputFilesPath, FFmpegArguments args, string outputFilesPath)
        {
            //-vf drawtext=text={i}:fontcolor=white:fontsize=50:x=10:y=10:font=Arial
            var shortest = inputFilesPath.Contains(".mp3") ? "-shortest " : "";
            string arguments = 
                $"{shortest}" +
                args.Resolution +
                args.FPS +
                args.Time +
                args.VideoBitrate +
                args.AudioBitrate +
                args.PixelFormat +
                args.VideoCodec +
                args.AudioCodec +
                args.Frames +
                args.Acceleration +
                args.Deceleration +
                args.NoAudio;

            startInfo.Arguments =
                args.Y +
                args.RTSP +
                args.Concat +
                args.Framerate +
                inputFilesPath +
                arguments +
                outputFilesPath;

            var process = Process.Start(startInfo);
            process?.WaitForExit();
        }
    }
}