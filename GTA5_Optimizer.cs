using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.Net; 
using System.Reflection;

[assembly: AssemblyTitle("GTA 5 Ultimate Optimizer")]
[assembly: AssemblyDescription("Utility to boost FPS in GTA 5")]
[assembly: AssemblyCompany("TrikozikGames")]
[assembly: AssemblyProduct("GTA 5 Ultimate Optimizer")]
[assembly: AssemblyCopyright("Copyright © 2026 TrikozikGames")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

namespace GTA5Optimizer
{
    class Program
    {
        static string currentVersion = "1.0.0";
        static string mode = "Обычный"; 
        static string lang = "ru";
        static bool superBoost = false;
        static bool networkFix = true; 
        static string configPath = "config.ini";
        static bool updateFound = false;

        [DllImport("psapi.dll")]
        static extern int EmptyWorkingSet(IntPtr hwProc);

        static void Main(string[] args)
        {
            try 
            { 
                Console.OutputEncoding = System.Text.Encoding.UTF8; 
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; 
            } 
            catch { }

            LoadSettings();

            // Фоновая проверка обновлений
            Thread updateThread = new Thread(() => { SilentCheckUpdate(); });
            updateThread.IsBackground = true;
            updateThread.Start();

            if (args.Length > 0 && args[0].ToLower() == "-start")
            {
                RunFullProcess(lang == "ru");
                return;
            }

            while (true)
            {
                bool isRu = (lang == "ru");
                Console.Title = "GTA 5 ULTIMATE OPTIMIZER - TrikozikGames";
                Console.Clear();
                
                if (updateFound)
                {
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(isRu ? " [!] ДОСТУПНО ОБНОВЛЕНИЕ! НАЖМИТЕ [6] " : " [!] UPDATE AVAILABLE! PRESS [6] ");
                    Console.ResetColor();
                    Console.WriteLine();
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("==========================================================");
                Console.WriteLine(string.Format("           GTA 5 ULTIMATE OPTIMIZER v{0}", currentVersion));
                Console.WriteLine(isRu ? "                Разработчик: TrikozikGames" : "                Developer: TrikozikGames");
                Console.WriteLine("==========================================================");
                Console.ResetColor();

                string langDisplay = isRu ? "Русский" : "English";
                Console.WriteLine(isRu ? string.Format("[1] Язык программы: {0}", langDisplay) : string.Format("[1] Language: {0}", langDisplay));
                Console.WriteLine(isRu ? string.Format("[2] Режим ускорения: {0}", mode) : string.Format("[2] Boost Mode: {0}", mode));
                Console.WriteLine(isRu ? string.Format("[3] Очистка кэша: {0}", (superBoost ? "ВКЛ" : "ВЫКЛ")) : string.Format("[3] Clean Cache: {0}", (superBoost ? "ON" : "OFF")));
                Console.WriteLine(isRu ? string.Format("[4] Стабильный пинг: {0}", (networkFix ? "ВКЛ" : "ВЫКЛ")) : string.Format("[4] Stable Ping: {0}", (networkFix ? "ON" : "OFF")));
                Console.WriteLine(isRu ? "[5] Создать ярлык" : "[5] Create Shortcut");
                Console.WriteLine(isRu ? "[6] Проверить обновления" : "[6] Check for Updates");
                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(isRu ? "\n[S] ОПТИМИЗИРОВАТЬ И ЗАПУСТИТЬ" : "\n[S] OPTIMIZE & LAUNCH");
                Console.ResetColor();
                Console.WriteLine(isRu ? "[X] Выход" : "[X] Exit");
                Console.Write("\n>>> ");

                string input = Console.ReadLine().ToUpper();

                if (input == "1") 
                { 
                    lang = (lang == "ru") ? "en" : "ru";
                    // Исправление: принудительно переводим режим при смене языка
                    if (lang == "ru") mode = (mode == "Gaming (Max)") ? "Игровой (Max)" : "Обычный";
                    else mode = (mode == "Игровой (Max)") ? "Gaming (Max)" : "Normal";
                    SaveSettings(); 
                }
                else if (input == "2") 
                { 
                    if (isRu) mode = (mode == "Обычный") ? "Игровой (Max)" : "Обычный";
                    else mode = (mode == "Normal") ? "Gaming (Max)" : "Normal";
                    SaveSettings(); 
                }
                else if (input == "3") { superBoost = !superBoost; SaveSettings(); }
                else if (input == "4") { networkFix = !networkFix; SaveSettings(); }
                else if (input == "5") { CreateShortcut(isRu); }
                else if (input == "6") { CheckUpdates(isRu); }
                else if (input == "X") break;
                else if (input == "S") { RunFullProcess(isRu); break; }
            }
        }

        static void SilentCheckUpdate()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("user-agent", "Mozilla/5.0");
                    string url = "https://raw.githubusercontent.com/TrikozikGames-offical/GTA-5-Ultimate-Optimizer/main/version.txt?t=" + DateTime.Now.Ticks;
                    string rawData = client.DownloadString(url);
                    string latest = rawData.Replace("currentVersion", "").Replace("=", "").Replace("\"", "").Replace(";", "").Trim();
                    if (latest != currentVersion) updateFound = true;
                }
            }
            catch { }
        }

        static void CheckUpdates(bool isRu)
        {
            Console.WriteLine(isRu ? "\n[!] Связь с GitHub..." : "\n[!] Connecting to GitHub...");
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("user-agent", "Mozilla/5.0");
                    string url = "https://raw.githubusercontent.com/TrikozikGames-offical/GTA-5-Ultimate-Optimizer/main/version.txt?t=" + DateTime.Now.Ticks;
                    string rawData = client.DownloadString(url);
                    string latest = rawData.Replace("currentVersion", "").Replace("=", "").Replace("\"", "").Replace(";", "").Trim();

                    if (latest != currentVersion)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(isRu ? "\n[!] Найдена версия " + latest : "\n[!] Found version " + latest);
                        Console.ResetColor();
                        Console.WriteLine(isRu ? "Перейти к загрузке? (Y/N)" : "Open download page? (Y/N)");
                        if (Console.ReadLine().ToUpper() == "Y")
                            Process.Start("https://github.com/TrikozikGames-offical/GTA-5-Ultimate-Optimizer/releases");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(isRu ? "\n[v] У вас актуальная версия." : "\n[v] Up to date.");
                        Console.ResetColor();
                    }
                }
            }
            catch { Console.WriteLine(isRu ? "\n[!] Ошибка сети." : "\n[!] Network error."); }
            Console.WriteLine(isRu ? "\nНажмите клавишу..." : "\nPress key...");
            Console.ReadKey();
        }

        static void CreateShortcut(bool isRu)
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string appPath = Process.GetCurrentProcess().MainModule.FileName;
                string shortcutPath = Path.Combine(desktopPath, "GTA 5 Optimizer.lnk");
                string script = string.Format("$s=(New-Object -COM WScript.Shell).CreateShortcut('{0}');$s.TargetPath='{1}';$s.Arguments='-start';$s.Save()", shortcutPath, appPath);
                ProcessStartInfo psi = new ProcessStartInfo("powershell", "-command \"" + script + "\"");
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(psi).WaitForExit();
                Console.WriteLine(isRu ? "\n[!] Готово!" : "\n[!] Done!");
                Thread.Sleep(1000);
            }
            catch { }
        }

        static void SaveSettings() { try { File.WriteAllLines(configPath, new string[] { lang, mode, superBoost.ToString(), networkFix.ToString() }); } catch { } }
        static void LoadSettings() { if (File.Exists(configPath)) { try { string[] lines = File.ReadAllLines(configPath); if (lines.Length >= 4) { lang = lines[0]; mode = lines[1]; bool.TryParse(lines[2], out superBoost); bool.TryParse(lines[3], out networkFix); } } catch { } } }

        static void RunFullProcess(bool isRu)
        {
            Console.Clear();
            Console.WriteLine(isRu ? ">>> ОПТИМИЗАЦИЯ..." : ">>> OPTIMIZING...");
            if (networkFix) RunSilentCommand("ipconfig /flushdns");
            if (superBoost) { try { ClearFolder(Path.GetTempPath()); } catch { } }
            foreach (Process process in Process.GetProcesses()) { try { EmptyWorkingSet(process.Handle); } catch { } }
            if (mode.Contains("Max")) { RunSilentCommand("powercfg -duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61"); RunSilentCommand("powercfg /setactive e9a42b02-d5df-448d-aa00-03f14749eb61"); }
            try { Process.Start("steam://rungameid/271590"); } catch { }
            for (int i = 0; i < 150; i++) { Process[] procs = Process.GetProcessesByName("GTA5"); if (procs.Length > 0) { foreach (var p in procs) { try { p.PriorityClass = ProcessPriorityClass.High; } catch { } } break; } Thread.Sleep(1000); }
            Thread.Sleep(2000);
        }

        static void ClearFolder(string folderPath) { try { DirectoryInfo di = new DirectoryInfo(folderPath); foreach (FileInfo file in di.GetFiles()) { try { file.Delete(); } catch { } } foreach (DirectoryInfo dir in di.GetDirectories()) { try { dir.Delete(true); } catch { } } } catch { } }
        static void RunSilentCommand(string cmd) { try { ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", "/c " + cmd); psi.WindowStyle = ProcessWindowStyle.Hidden; psi.CreateNoWindow = true; Process.Start(psi).WaitForExit(); } catch { } }
    }
}