using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace POCVideo
{
    class Program
    {
        static void Main(string[] args)
        {
            var t1 = FFmpeg.Cut("GOPR3145.MP4", "test.mp4", 30, 10);
            var t2 = FFmpeg.Cut("GOPR3145.MP4", "test2.mp4", 50, 10);
            Task.WaitAll(t1, t2);

            Process.Start("copy", "test.mp4 /b + test2.mp4 /b test3.mp4 /b");
            Console.WriteLine("END");
            Console.Read();
        }

        static bool Cut(string path, string outPath, int start, int duration)
        {
            Process p = new Process();
            p.StartInfo.FileName = "ffmpeg.exe";
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.Arguments = $"-ss {start} -i {path} -t {duration} -c copy {outPath}";

            p.Exited += (sender, args) =>
            {
                Console.WriteLine("End");
            };

            p.OutputDataReceived += (sender, args) =>
            {
                Console.WriteLine(args.Data);
            };

            p.ErrorDataReceived += (sender, args) =>
            {
                Console.WriteLine(args.Data);
            };
            p.Start();
            p.BeginErrorReadLine();
            p.BeginOutputReadLine();
            p.WaitForExit();
            return true;
        }
    }
}
