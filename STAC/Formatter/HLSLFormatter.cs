using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Formatter
{
    internal class HLSLFormatter
    {
        public static string Format(string input)
        {
            if (!(Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.Is64BitOperatingSystem))
            {
                Debug.WriteLine("Formatter only implemented for Windows 64 bit");
                return input;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                FileName = "formatter/uncrustify.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = "-c formatter/config.txt -l C",
            };
            using (Process? uncrustifyProcess = Process.Start(startInfo))
            {
                ArgumentNullException.ThrowIfNull(uncrustifyProcess);
                uncrustifyProcess.StandardInput.Write(input);
                uncrustifyProcess.StandardInput.Close();
                uncrustifyProcess.ErrorDataReceived += ExeProcess_ErrorDataReceived;
                var output = uncrustifyProcess.StandardOutput.ReadToEnd();
                uncrustifyProcess.WaitForExit();
                uncrustifyProcess.ErrorDataReceived -= ExeProcess_ErrorDataReceived;

                return output;
            }
        }

        private static void ExeProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine($"Uncrustify err: {e.Data}");
        }
    }
}
