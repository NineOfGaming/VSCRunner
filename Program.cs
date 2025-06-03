//This is an application created for my school to save and load Visual Studio Code extensions,
//as the school computers reset after each restart.
//Copyright(C) 2024  NineOfGaming
//
//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.
//
//You should have received a copy of the GNU General Public License
//along with this program.If not, see<https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
//using Microsoft.Win32;

namespace VSRunner
{
    class Program
    {
        static void Main()
        {
            //Title
            Console.Title = "VSCode Runner v." + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
            Console.WriteLine("VSCRunner v." + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion + "  Copyright (C) 2024  NineOfGaming\nThis program comes with ABSOLUTELY NO WARRANTY;\nThis is free software, and you are welcome to redistribute it under certain conditions;\n");

            //Directories
            string vscodeSettingsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Code\User\settings.json");
            string vscodeExeDir = @"C:\Program Files\Microsoft VS Code\Code.exe";
            //string vscodeExeDir = GetVSCodeInstallPath() + "Code.exe";
            string vscodeExtDirL = @"H:\VSCodeExtensions";
            string vscodeExtDir = @"C:\Users\technik\.vscode\extensions";
            string settingsFile = Path.Combine(vscodeExtDirL, "settings.json");
            string extensionsDir = Path.Combine(vscodeExtDirL, "extensions");

            //Display VSCode Runner main Paths
            Console.WriteLine("VSCRunner Paths:");
            Console.WriteLine("VSCode Setting Dir:   " + vscodeSettingsDir);
            Console.WriteLine("VSCode exe Dir:       " + vscodeExeDir);
            Console.WriteLine("VSCode Ext Dir Local: " + vscodeExtDirL);
            Console.WriteLine("VSCode Ext Dir:       " + vscodeExtDir);
            Console.WriteLine();

            //Check if VSCode executable exists
            Console.WriteLine("Looking for VSCode Executable");
            if (File.Exists(vscodeExeDir))
            {
                Console.WriteLine("VSCode found");
            }
            else
            {
                Console.WriteLine("VSCode not found");
                Console.WriteLine("Please make sure VSCode is installed on the device");
                Console.WriteLine("Exiting...");
                Thread.Sleep(5000);
                Environment.Exit(0);
            }

            //Check if VSCodeExtensions directory exists, if not, create it
            Console.WriteLine("Looking for VSCodeExtensions directory");
            if (!Directory.Exists(vscodeExtDirL))
            {
                Console.WriteLine("VSCodeExtensions directory not found");
                Directory.CreateDirectory(vscodeExtDirL);
                Console.WriteLine("Created VSCodeExtensions directory");
            }
            else
            {
                Console.WriteLine("VSCodeExtensions directory found");
            }

            //Check if VSCodeExtensions\extensions directory exists, if not, create it
            Console.WriteLine("Looking for VSCodeExtensions\\extensions directory");
            if (!Directory.Exists(extensionsDir))
            {
                Console.WriteLine("extensions directory not found");
                Directory.CreateDirectory(extensionsDir);
                Console.WriteLine("Created extensions directory");
            }
            else
            {
                Console.WriteLine("extensions directory found");
            }

            //Replace Extensions
            Console.WriteLine("Syncing extensions to VSCode extensions");
            Console.WriteLine("This may take a moment");
            Thread.Sleep(2000);
            Directory.CreateDirectory(vscodeExtDir);
            DirectoryCopy(extensionsDir, vscodeExtDir).GetAwaiter().GetResult();
            Console.WriteLine("Extensions synced");

            //Copy user settings to VSCode settings directory
            Console.WriteLine("Looking for VSCodeExtensions Settings");
            if (File.Exists(settingsFile))
            {
                Console.WriteLine("VSCodeExtensions Settings found");
                File.Copy(settingsFile, vscodeSettingsDir, true);
                Console.WriteLine("Copied Settings from VSCodeExtentions to VSCode");
            }
            else
            {
                Console.WriteLine("VSCodeExtensions Settings not found");
                File.WriteAllText(settingsFile, string.Empty);
                Console.WriteLine("Created VSCodeExtensions Settings");
            }

            //Please do not the Cat
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nDo not close this window!\nIt will not be able to save you extensions and settings!");
            Console.ResetColor();

            // Launch VSCode
            Console.WriteLine("\nLaunching VSCode");
            Process vscodeProcess = new Process();
            vscodeProcess.StartInfo.FileName = vscodeExeDir;
            vscodeProcess.StartInfo.UseShellExecute = false;
            vscodeProcess.StartInfo.RedirectStandardOutput = true;
            vscodeProcess.StartInfo.RedirectStandardError = true;

            // Set up event handlers to capture and display output
            vscodeProcess.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Console.WriteLine("[VSCode Output] " + e.Data);
                }
            };

            vscodeProcess.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Console.WriteLine("[VSCode Error] " + e.Data);
                }
            };

            vscodeProcess.Start();
            vscodeProcess.BeginOutputReadLine();
            vscodeProcess.BeginErrorReadLine();

            vscodeProcess.WaitForExit();

            //Copy modified settings and extensions back to VSCodeExtensions directory
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nDo not close this window!\nIt will not be able to save you extensions and settings!");
            Console.ResetColor();

            Console.WriteLine("\nSaving settings");
            File.Copy(vscodeSettingsDir, settingsFile, true);
            Console.WriteLine("Saved settings");

            Console.WriteLine("Saving extensions back to VSCodeExtensions directory");
            Console.WriteLine("This may take a moment");
            Thread.Sleep(2000);
            Directory.CreateDirectory(extensionsDir);
            DirectoryCopy(vscodeExtDir, extensionsDir).GetAwaiter().GetResult();
            Console.WriteLine("Extensions saved");
            Console.WriteLine("Exiting...");
            Thread.Sleep(4000);
            Environment.Exit(0);

        }

        /// <summary>
        /// Copies the contents of a directory to a new location.
        /// </summary>
        /// <param name="sourceDirString">The path of the source directory to copy from.</param>
        /// <param name="destDirString">The path of the destination directory to copy to.</param>
        /// <param name="deleteMissing">If true, deletes files and subdirectories in the destination that are missing from the source. Default is true.</param>
        /// <param name="copySubDirs">If true, recursively copies all subdirectories. Default is true.</param>
        /// <param name="consoleOutput">If true, logs activity to console. Default is true.</param>
        /// <exception cref="DirectoryNotFoundException">Thrown when the source directory does not exist.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the source or destination directory is Null or Empty.</exception>
        /// <returns>A task that represents the asynchronous operation of copying a directory and its contents.</returns>
        private static async Task DirectoryCopy(string sourceDirString, string destDirString, bool deleteMissing = true, bool copySubDirs = true, bool consoleOutput = true)
        {
            // Null checks for input parameters to prevent null reference exceptions
            if (string.IsNullOrEmpty(sourceDirString))
            {
                throw new ArgumentNullException(nameof(sourceDirString), "Source directory path cannot be null or empty.");
            }
            if (string.IsNullOrEmpty(destDirString))
            {
                throw new ArgumentNullException(nameof(destDirString), "Destination directory path cannot be null or empty.");
            }

            string fullSourcePath = Path.GetFullPath(sourceDirString);
            string fullDestPath = Path.GetFullPath(destDirString);

            // Check if the source directory exists
            DirectoryInfo sourceDir = new DirectoryInfo(fullSourcePath);
            if (!sourceDir.Exists)
            {
                throw new DirectoryNotFoundException($"Source directory does not exist: {fullSourcePath}");
            }

            if (consoleOutput)
            {
                Console.WriteLine($"Syncing {sourceDirString} to {destDirString}");
            }

            // Ensure destination directory exists
            Directory.CreateDirectory(fullDestPath);

            // Get directories and files
            DirectoryInfo[] sourceDirs = sourceDir.GetDirectories();
            FileInfo[] sourceFiles = sourceDir.GetFiles();

            DirectoryInfo destDir = new DirectoryInfo(fullDestPath);
            FileInfo[] destFiles = destDir.GetFiles();
            DirectoryInfo[] destDirs = destDir.GetDirectories();

            // Collect all tasks
            List<Task> tasks = new List<Task>();

            // If deleteMissing is true, delete files and subdirectories in the destination that don't exist in the source
            if (deleteMissing)
            {
                Parallel.ForEach(destFiles, destFile =>
                {
                    string sourceFilePath = Path.Combine(fullSourcePath, destFile.Name);
                    if (!File.Exists(sourceFilePath))
                    {
                        if (consoleOutput)
                        {
                            Console.WriteLine($"Deleting file {destFile.Name}");
                        }
                        File.Delete(destFile.FullName);
                    }
                });

                Parallel.ForEach(destDirs, destSubdir =>
                {
                    string sourceSubdirPath = Path.Combine(fullSourcePath, destSubdir.Name);
                    if (!Directory.Exists(sourceSubdirPath))
                    {
                        if (consoleOutput)
                        {
                            Console.WriteLine($"Deleting directory {destSubdir.Name}");
                        }
                        Directory.Delete(destSubdir.FullName, true);
                    }
                });
            }

            // Copy new and updated files from source to destination
            foreach (FileInfo sourceFile in sourceFiles)
            {
                string destFilePath = Path.Combine(fullDestPath, sourceFile.Name);

                // If the destination file exists, compare hashes
                bool needsCopy = !File.Exists(destFilePath);
                if (!needsCopy)
                {
                    using (SHA256 sha256 = SHA256.Create())
                    {
                        string sourceFileHash;
                        using (Stream stream = File.OpenRead(sourceFile.FullName))
                        {
                            byte[] hash = sha256.ComputeHash(stream);
                            sourceFileHash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                        }

                        string destFileHash;
                        using (Stream stream = File.OpenRead(destFilePath))
                        {
                            byte[] hash = sha256.ComputeHash(stream);
                            destFileHash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                        }

                        needsCopy = sourceFileHash != destFileHash;
                    }
                }

                if (needsCopy)
                {
                    if (consoleOutput)
                    {
                        Console.WriteLine($"Copying {sourceFile.Name} to {fullDestPath}");
                    }

                    tasks.Add(Task.Run(async () =>
                    {
                        using (FileStream sourceStream = new FileStream(sourceFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true))
                        using (FileStream destStream = new FileStream(destFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
                        {
                            await sourceStream.CopyToAsync(destStream);
                        }
                    }));
                }
            }

            // If copying subdirectories, sync them too
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in sourceDirs)
                {
                    string destSubdirPath = Path.Combine(destDirString, subdir.Name);
                    tasks.Add(Task.Run(() => DirectoryCopy(subdir.FullName, destSubdirPath, deleteMissing, copySubDirs, consoleOutput)));
                }
            }

            // Wait for all tasks to complete
            await Task.WhenAll(tasks);

            Console.WriteLine($"Finished syncing {sourceDirString}");
        }



        // y u no work >:(

        /*static string GetVSCodeInstallPath()
        {
            string[] registryKeys = new string[]
            {
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\",
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\",
            @"Software\Microsoft\Windows\CurrentVersion\Uninstall\"
            };

            foreach (string key in registryKeys)
            {
                string path = SearchRegistryKey(Registry.LocalMachine, key) ?? SearchRegistryKey(Registry.CurrentUser, key);
                if (!string.IsNullOrEmpty(path))
                {
                    return path;
                }
            }

            return null;
        }

        static string SearchRegistryKey(RegistryKey rootKey, string subKeyPath)
        {
            using (RegistryKey registryKey = rootKey?.OpenSubKey(subKeyPath))
            {
                if (registryKey == null) return null;

                foreach (string subKeyName in registryKey.GetSubKeyNames())
                {
                    string installPath = CheckVSCodeInSubKey(registryKey, subKeyName);
                    if (!string.IsNullOrEmpty(installPath)) return installPath;
                }
            }

            return null;
        }

        static string CheckVSCodeInSubKey(RegistryKey parentKey, string subKeyName)
        {
            using (RegistryKey subKey = parentKey?.OpenSubKey(subKeyName))
            {
                if (subKey == null) return null;
                string displayName = subKey.GetValue("DisplayName") as string;
                if (displayName?.Contains("Visual Studio Code") == true)
                {
                    return subKey.GetValue("InstallLocation") as string;
                }
            }

            return null;
        }*/
    }
}
