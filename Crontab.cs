using System;
using System.IO;

namespace RTSP_Timelapse_App
{
    public class Crontab
    {
        public static void Create()
        {
            if (!File.Exists("/var/spool/cron/" + Environment.UserName))
            {
                string command = "touch /var/spool/cron/" + Environment.UserName;
                Bash.SudoExecute(command);
            }
        }
        
        public static string Get(string fileName)
        {
            var crons = GetAll();
            foreach (var cron in crons)
                if (cron.Contains(fileName))
                    return cron;
            return string.Empty;
        }
        
        public static string[] GetAll()
        {
            return Bash.ExecuteWithResult("crontab -l").Split("\n", StringSplitOptions.RemoveEmptyEntries);
        }

        public static void Add(string cron)
        {
            string command = $"echo '{cron}' >> /var/spool/cron/" + Environment.UserName;
            Bash.Execute(command);
        }

        public static void Change(string cron, string newCron)
        {
            var crons = GetAll();
            var neededCron = Get(cron);
            for (int i = 0; i < crons.Length; i++)
                if (crons[i] == neededCron)
                {
                    crons[i] = newCron;
                    string command = "rm /var/spool/cron/" + Environment.UserName;
                    Bash.Execute(command);
                    for (int j = 0; j < crons.Length -1; j++)
                        Add(crons[j]);
                    Add(crons[^1]);
                    break;
                }
        }

        public static void Remove(string cron)
        {
            var crons = GetAll();
            var neededCron = Get(cron);
            for (int i = 0; i < crons.Length; i++)
                if (crons[i] == neededCron)
                {
                    crons[i] = "";
                    string command = "rm /var/spool/cron/" + Environment.UserName;
                    Bash.Execute(command);
                    for (int j = 0; j < crons.Length -1; j++)
                        Add(crons[j]);
                    Add(crons[^1]);
                    break;
                }
        }
    }
}