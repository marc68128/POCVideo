using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCVideo
{
    public class FFmpeg
    {
        public static Task Cut(string path, string outPath, int start, int duration)
        {
            return Task.Run(() =>
            {
                Process p = new Process();
                p.StartInfo.FileName = "ffmpeg.exe";
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.Arguments = $"-ss {start} -i {path} -t {duration} -c copy {outPath}";

                p.Start();
                p.WaitForExit();
            });
        }

        public static Task Concat(string outPath, params string[] videos)
        {
            return Task.Run(() =>
            {
                Process p = new Process();
                p.StartInfo.FileName = "ffmpeg.exe";
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardError = true;

                p.ErrorDataReceived += (sender, args) =>
                {
                    Console.WriteLine(args.Data);
                };

                var arguments = "";
                foreach (var video in videos)
                    arguments += $"-i {video} ";
                arguments += "-filter_complex \"";
                for (int i = 0; i < videos.Length; i++)
                {
                    arguments += $"[{i}:0] ";
                }
                arguments += $"concat=n={videos.Length}:v=1:a=0 [v]\" ";
                arguments += $"-map \"[v]\" {outPath}";

                p.StartInfo.Arguments = arguments;

                p.Start();
                p.BeginErrorReadLine();
                p.WaitForExit();
            });
        }

        public static Task ChangeDuration(string path, TimeSpan desiratedDuration)
        {
            return Task.Run(() =>
            {
                var tmpPath = Path.GetFileNameWithoutExtension(path) + DateTime.Now.Ticks.ToString().Substring(0, 10) + Path.GetExtension(path);
                File.Move(path, tmpPath);

                var duration = GetDuration(tmpPath);
                var scale = (desiratedDuration.TotalMilliseconds / duration.TotalMilliseconds).ToString("0.0").Replace(',', '.');

                Process p = new Process();
                p.StartInfo.FileName = "ffmpeg.exe";
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardError = true;
                
                p.StartInfo.Arguments = $"-i {tmpPath} -an -filter:v \"setpts = {scale} * PTS\" {path}";

                p.ErrorDataReceived += (sender, args) =>
                {
                    Console.WriteLine(args.Data);
                };

                p.Start();
                p.BeginErrorReadLine();
                p.WaitForExit();
                
                File.Delete(tmpPath);
            });
        }

        public static TimeSpan GetDuration(string path)
        {
            StreamReader errorreader;

            Process ffmpeg = new Process();

            ffmpeg.StartInfo.UseShellExecute = false;
            ffmpeg.StartInfo.ErrorDialog = false;
            ffmpeg.StartInfo.RedirectStandardError = true;

            ffmpeg.StartInfo.FileName = "ffmpeg.exe";
            ffmpeg.StartInfo.Arguments = $"-i {path}";


            ffmpeg.Start();

            errorreader = ffmpeg.StandardError;

            ffmpeg.WaitForExit();

            string result = errorreader.ReadToEnd();

            string duration = result.Substring(result.IndexOf("Duration: ") + ("Duration: ").Length, ("00:00:00.00").Length);

            return TimeSpan.Parse(duration);
        }
    }
}