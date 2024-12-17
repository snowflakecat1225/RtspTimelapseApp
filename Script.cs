using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace RTSP_Timelapse_App
{
    public class Script
    {
        public static void Create(string rtsp, string resultPath, int days)
        {
            Temp.Create();
            
            using (StreamWriter sw = new($"{Temp.Path}/script.sh"))
                sw.Write($"#!/bin/bash\n{Environment.CurrentDirectory}/{Process.GetCurrentProcess().ProcessName} 1 {days} 0");
            Bash.SudoExecute($"--stdin chmod u=rwx {Temp.Path}/script.sh <<< 1225");

            using (StreamWriter sw = new($"{Temp.Path}/config.txt"))
                sw.Write($"{rtsp}\n{resultPath}");

            Bash.SudoExecute($"--stdin chown {Environment.UserName} /var/spool/cron <<< 1225");
            if (Crontab.Get($"{Temp.Path}/script.sh") == string.Empty)
                Crontab.Add($"0 7 * * * {Temp.Path}/script.sh");
            else
                Crontab.Change(Crontab.Get($"{Temp.Path}/script.sh"), $"0 7 * * * {Temp.Path}/script.sh");
        }

        public static void Run(string[] args)
        {
            if (args[0].IsNumber() && args[1].IsNumber() && args[2].IsNumber())
            {
                string config = string.Empty;
                using (StreamReader sr = new($"{Temp.Path}/config.txt"))
                    config = sr.ReadToEnd();
                string videoPath = config.Split("\n", StringSplitOptions.RemoveEmptyEntries)[0];
                string resultPath = config.Split("\n", StringSplitOptions.RemoveEmptyEntries)[1];

                int day = int.Parse(args[0]);
                int days = int.Parse(args[1]);
                if (day <= days)
                {
                    Task.Delay(int.Parse(args[2]) - 3 * 1000 > 0 ? int.Parse(args[2]) - 3 * 1000 : 0).Wait();

                    Temp.Create();
                
                    int allTime = 16 * 60 * 60;
                    int videoTime = allTime / days + Convert.ToInt32(Math.Round((allTime - allTime / days * days) / Convert.ToDouble(days)));
                    int minutes = videoTime / 60;
                    int seconds = videoTime - minutes * 60;

                    var resolution = FFprobe.GetInfo.Resolution(videoPath);
                    var fps = FFprobe.GetInfo.Framerate(videoPath).ToString();

                    FFmpegArguments arguments = new()
                    {
                        Y = "true",
                        RTSP = "true",
                        Resolution = resolution,
                        FPS = fps,
                        Time = videoTime.ToString(),
                    };
                    FFmpeg.Execute($"-i {videoPath} ", arguments, $"{Temp.Path}/video{day}.mp4");

                    Task.Delay(5 * 1000).Wait();
                
                    arguments = new()
                    {
                        Y = "true",
                        Resolution = resolution,
                        Acceleration = 98.ToString(),
                        NoAudio = "true"
                    };
                    FFmpeg.Execute($"-i {Temp.Path}/video{day}.mp4 ", arguments, Temp.Path + $"/shortVideo{day}.mp4");
                
                    Task.Delay(5 * 1000).Wait();

                    File.Delete($"{Temp.Path}/video{day}.mp4");

                    Task.Delay(5 * 1000).Wait();
                
                    string addFile = $"file \'shortVideo{day}.mp4\'";
                    if (day == 1)
                    {
                        Bash.Execute($"touch {Temp.Path}/concatConfig.txt");
                        using (var sw = new StreamWriter($"{Temp.Path}/concatConfig.txt", true))
                            sw.WriteLine(addFile);
                    }
                    else if (day == 2)
                    {
                        if (!File.Exists($"{Temp.Path}/concatConfig.txt"))
                            Bash.Execute($"touch {Temp.Path}/concatConfig.txt");
                        using (var sw = new StreamWriter($"{Temp.Path}/concatConfig.txt", true))
                            sw.WriteLine(addFile);
                        arguments = new()
                        {
                            Y = "true",
                            Concat = "true"
                        };
                        FFmpeg.Execute($"-i {Temp.Path}/concatConfig.txt ", arguments, $"{Temp.Path}/videoTemp.mp4");

                        Task.Delay(10 * 1000).Wait();

                        Bash.Execute($"mv -f {Temp.Path}/videoTemp.mp4 {Temp.Path}/video.mp4");
    
                        Task.Delay(5 * 1000).Wait();
                    
                        File.Delete($"{Temp.Path}/shortVideo{1}.mp4");
                        File.Delete($"{Temp.Path}/shortVideo{2}.mp4");
                    }
                    else
                    {
                        if (!File.Exists($"{Temp.Path}/concatConfig.txt"))
                            Bash.Execute($"touch {Temp.Path}/concatConfig.txt");
                        using (var sw = new StreamWriter($"{Temp.Path}/concatConfig.txt", false))
                            sw.WriteLine("file \'video.mp4\'\n" + addFile);
                        arguments = new()
                        {
                            Y = "true",
                            Concat = "true"
                        };
                        FFmpeg.Execute($"-i {Temp.Path}/concatConfig.txt ", arguments, $"{Temp.Path}/videoTemp.mp4");

                        Task.Delay(10 * 1000).Wait();

                        Bash.Execute($"mv -f {Temp.Path}/videoTemp.mp4 {Temp.Path}/video.mp4");
    
                        Task.Delay(5 * 1000).Wait();
                    
                        File.Delete(Temp.Path + $"/shortVideo{day}.mp4");
                    }

                    Task.Delay(5 * 1000).Wait();

                    string cron = $"0 7 * * * {Temp.Path}/script.sh";
                    string script = $"#!/bin/bash\n{Environment.CurrentDirectory}/{Process.GetCurrentProcess().ProcessName} 1 {days} 0";

                    var cronParts = cron.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    string newCron = string.Empty;
                    int extraMinutes = seconds * day / 60;
                    var cronMinutes = minutes * day + extraMinutes;
                    var cronHours = int.Parse(cronParts[1]);
                    while (cronMinutes > 59)
                    {
                        cronMinutes -= 60;
                        cronHours++;
                        if (cronHours > 23)
                            cronHours -= 24;
                    }
                    cronParts[0] = cronMinutes.ToString();
                    cronParts[1] = cronHours.ToString();
                    foreach (var cronPart in cronParts)
                        newCron += cronPart + " ";
                    Crontab.Change(Crontab.Get($"{Temp.Path}/script.sh"), newCron);

                    var scriptParts = script.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                    var scriptPart2 = scriptParts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    string newScript = string.Empty;
                    string newScriptPart2 = string.Empty;
                    var delay = (seconds * day - extraMinutes * 60) % 60;
                    scriptPart2[^1] = delay.ToString();
                    scriptPart2[^3] = (day + 1).ToString();
                    foreach (var scriptPartPart2 in scriptPart2)
                        newScriptPart2 += scriptPartPart2 + " ";
                    scriptParts[1] = newScriptPart2;
                    foreach (var scriptPart in scriptParts)
                        newScript += scriptPart + "\n";
                    using (StreamWriter sw = new($"{Temp.Path}/script.sh"))
                        sw.Write(newScript);
                }
                else
                {
                    Crontab.Remove($"{Temp.Path}/script.sh");
                    Bash.Execute($"mv {Temp.Path}/video.mp4 {resultPath}/output.mp4");
                    Bash.Execute($"rm {Temp.Path}/script.sh");
                    Bash.Execute($"{Temp.Path}/concatConfig.txt");
                    Bash.Execute($"rmdir {Temp.Path}");
                }
            }
        }
    }
}