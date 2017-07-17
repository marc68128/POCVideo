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
    }
}