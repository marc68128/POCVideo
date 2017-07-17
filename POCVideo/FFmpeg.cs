using System;
using System.Collections.Generic;
using System.Diagnostics;
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

                var arguments = "";
                foreach (var video in videos)
                    arguments += $"-i {video} ";
                arguments += "-filter_complex \"";
                for (int i = 0; i < videos.Length; i++)
                {
                    arguments += $"[{i}:v:0] [{i}:a:0] ";
                }
                arguments += $"concat=n={videos.Length}:v=1:a=1 [v] [a]\" ";
                arguments += $"-map \"[v]\" -map \"[a]\" {outPath}";

                p.StartInfo.Arguments = arguments;

                p.Start();
                p.WaitForExit();
            });
        }
    }
}