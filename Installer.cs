using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Reflection;

[assembly: AssemblyTitle("GTA 5 Ultimate Optimizer Installer")]
[assembly: AssemblyDescription("Professional Setup by TrikozikGames")]
[assembly: AssemblyCompany("TrikozikGames")]
[assembly: AssemblyProduct("GTA 5 Ultimate Optimizer")]
[assembly: AssemblyCopyright("Copyright © 2026 TrikozikGames (Kirill)")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

namespace TrikozikGamesInstaller
{
    class Program
    {
        static void Main(string[] args)
        {
            try { Console.OutputEncoding = System.Text.Encoding.UTF8; } catch { }

            string rootDir = AppDomain.CurrentDomain.BaseDirectory;
            string sourceDir = Path.Combine(rootDir, "SetupFiles");
            string destDir = @"C:\GTA 5 Ultimate Optimizer";
            
            Console.Title = "Installer - GTA 5 Ultimate Optimizer v1.0.0";
            Console.Clear();
            Console.WriteLine("==========================================================");
            Console.WriteLine("           GTA 5 ULTIMATE OPTIMIZER INSTALLER");
            Console.WriteLine("                Разработчик: TrikozikGames");
            Console.WriteLine("==========================================================");
            
            Console.WriteLine("\nВЫБЕРИТЕ ЯЗЫК / SELECT LANGUAGE:");
            Console.WriteLine("[1] English");
            Console.WriteLine("[2] Русский");
            Console.Write("\n>>> ");
            
            string choice = Console.ReadLine();
            bool isRu = (choice == "2");

            Console.WriteLine(isRu ? "\n[!] Подготовка..." : "\n[!] Preparing...");

            try
            {
                if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);

                // ЗАПИСЬ ЯЗЫКА ДЛЯ ОПТИМИЗАТОРА
                File.WriteAllText(Path.Combine(destDir, "lang.ini"), isRu ? "ru" : "en");

                // Копирование всех файлов (включая папку Icon)
                foreach (string dirPath in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(sourceDir, destDir));

                foreach (string newPath in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
                    File.Copy(newPath, newPath.Replace(sourceDir, destDir), true);

                // Создание ярлыка
                string exePath = Path.Combine(destDir, "GTA5_Optimizer.exe");
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string lnkPath = Path.Combine(desktopPath, "GTA 5 Ultimate Optimizer.lnk");
                string finalIconPath = Path.Combine(destDir, @"Icon\GTA5.ico");

                string psCommand = "-Command \"$s=(New-Object -COM WScript.Shell).CreateShortcut('" + lnkPath + "'); " +
                                   "$s.TargetPath='" + exePath + "'; " +
                                   "$s.WorkingDirectory='" + destDir + "'; " +
                                   "$s.IconLocation='" + finalIconPath + "'; $s.Save()\"";

                ProcessStartInfo psInfo = new ProcessStartInfo("powershell.exe", psCommand) { WindowStyle = ProcessWindowStyle.Hidden, CreateNoWindow = true };
                Process.Start(psInfo).WaitForExit();

                Console.WriteLine(isRu ? "\n[ГОТОВО] Установка завершена!" : "\n[DONE] Installation complete!");
                Thread.Sleep(2000);

                // Самоудаление временной папки проекта
                ProcessStartInfo delInfo = new ProcessStartInfo("cmd.exe", "/c timeout /t 1 >nul & rd /s /q \"" + rootDir.TrimEnd('\\') + "\"") { WindowStyle = ProcessWindowStyle.Hidden };
                Process.Start(delInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.ReadLine();
            }
        }
    }
}