using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ImmersionMirror
{
    public static class ShellHelper
    {
        public static string Bash(this string cmd, bool dryRun = false)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                }
            };

            Console.WriteLine($"> {cmd}");
            if (!dryRun)
            {
                process.Start();
                string result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                return result;
            }
            else
            {
                return "[DR CMD] " + cmd;
            }
        }
    }
}
