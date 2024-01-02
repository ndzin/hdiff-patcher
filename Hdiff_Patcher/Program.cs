using System;
using System.Diagnostics;
using System.IO;

namespace Hdiff_Patcher
{
    internal class Program
    {
        static void Main()
        {
            // No command output
            Console.WriteLine("Checking if all necessary files for Patch are present...");

            // Set working directory to current directory
            string workingDirForAdmin = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // Check if all files are present
            bool fileMissing = false;
            foreach (string line in File.ReadLines(Path.Combine(workingDirForAdmin, "hdifffiles.txt")))
            {
                string filePath = Path.Combine(workingDirForAdmin, line);
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"{filePath} is missing.");
                    fileMissing = true;
                }

                string diffFilePath = filePath + ".hdiff";
                if (!File.Exists(diffFilePath))
                {
                    Console.WriteLine($"{diffFilePath} is missing.");
                    fileMissing = true;
                }
            }

            string hpatchzPath = Path.Combine(workingDirForAdmin, "lib", "hpatchz.exe");
            if (!File.Exists(hpatchzPath))
            {
                Console.WriteLine($"{hpatchzPath} is missing.");
                fileMissing = true;
            }

            // Result of check if all files are present
            if (fileMissing)
            {
                Console.WriteLine("\nAt least one file is missing. Please extract/download the necessary files listed above and try again.");

                // Query if the user wants to try again
                Console.Write("Retry patch application now? (y/n): ");
                char selection = Console.ReadKey().KeyChar;
                Console.WriteLine();
                if (selection == 'y' || selection == 'Y')
                {
                    Main(); // Retry patch application
                }
                else
                {
                    Console.WriteLine("Aborted patch application. Exiting.");
                }
            }
            else
            {
                ApplyPatch(workingDirForAdmin);
            }
        }

        static void ApplyPatch(string workingDirForAdmin)
        {
            Console.WriteLine("All necessary files are present. Applying patch now...\n");

            foreach (string line in File.ReadLines(Path.Combine(workingDirForAdmin, "hdifffiles.txt")))
            {
                string originalFilePath = Path.Combine(workingDirForAdmin, line);
                string patchFilePath = originalFilePath + ".hdiff";

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = Path.Combine(workingDirForAdmin, "lib", "hpatchz.exe"),
                    Arguments = $"-f \"{originalFilePath}\" \"{patchFilePath}\" \"{originalFilePath}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process process = new Process { StartInfo = psi };
                process.Start();
                process.WaitForExit();
            }

            // Delete obsolete files after patch application
            foreach (string line in File.ReadLines(Path.Combine(workingDirForAdmin, "deletefiles.txt")))
            {
                string filePath = Path.Combine(workingDirForAdmin, line);
                if (line == "mhypbase.dll")
                {
                    File.SetAttributes(filePath, File.GetAttributes(filePath) & ~FileAttributes.ReadOnly);
                }
                File.Delete(filePath);
            }

            // Delete source file for deletefiles.txt and the file itself
            File.Delete(Path.Combine(workingDirForAdmin, "deletefiles.txt"));
            File.Delete(Path.Combine(workingDirForAdmin, "deletefiles.txt"));

            // Delete obsolete .hdiff files after patch application
            foreach (string line in File.ReadLines(Path.Combine(workingDirForAdmin, "hdifffiles.txt")))
            {
                File.Delete(Path.Combine(workingDirForAdmin, line + ".hdiff"));
            }

            // Delete source file for hdifffiles.txt and the file itself
            File.Delete(Path.Combine(workingDirForAdmin, "hdifffiles.txt"));
            File.Delete(Path.Combine(workingDirForAdmin, "hdifffiles.txt"));

            // Delete patch application
            File.Delete(Path.Combine(workingDirForAdmin, "lib", "hpatchz.exe"));

            // Delete diff application if someone extracted it too
            string hdiffzPath = Path.Combine(workingDirForAdmin, "lib", "hdiffz.exe");
            if (File.Exists(hdiffzPath))
            {
                File.Delete(hdiffzPath);
            }

            Console.WriteLine("Deleted all obsolete files after patch. Patch application is finished now. Enjoy the game.\n");

            // Pause
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }

}
