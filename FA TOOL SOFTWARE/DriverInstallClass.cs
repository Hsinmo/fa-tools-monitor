using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace FA_TOOL_SOFTWARE
{
    class DriverInstallClass
    {
        private string gCurrentProjectDirectory;

        string fileName_devcon = "devcon.exe";
        string fileName_inf = "MSP430_CDC.inf";
        string fileName_bat = "deviceSet.bat";
        string sourcePath = @".\driver";
        string targetPath = @"C:\DP_FATOOL\SetUp_Driver";
        string SysInfPath = @"C:\windows\inf";
        string Sys32Path = @"C:\windows\System32";

        // Use Path class to manipulate file and directory paths.
        string sourceFile;
        string destFile;
        //string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
        //string destFile = System.IO.Path.Combine(targetPath, fileName);

        public DriverInstallClass()
        {
            gCurrentProjectDirectory = System.Windows.Forms.Application.StartupPath;
        }

        public void copysysFile()
        {
            // To copy a folder's contents to a new location:
            // Create a new target folder, if necessary.
            if (!System.IO.Directory.Exists(targetPath))
            {
                System.IO.Directory.CreateDirectory(targetPath);
            }

            // To copy a file to another location and 
            // overwrite the destination file if it already exists.
            //=============copy fileName_devcon ====================
            sourceFile = System.IO.Path.Combine(sourcePath, fileName_devcon);
            destFile = System.IO.Path.Combine(targetPath, fileName_devcon);
            if (!System.IO.File.Exists(destFile))
            {
                //System.IO.File.Copy(sourceFile, destFile, true);
            }
            sourceFile = System.IO.Path.Combine(sourcePath, fileName_devcon);
            destFile = System.IO.Path.Combine(Sys32Path, fileName_devcon);
            if (!System.IO.File.Exists(destFile))
            {
                System.IO.File.Copy(sourceFile, destFile, true);
            }

            //=============copy fileName_inf ====================
            sourceFile = System.IO.Path.Combine(sourcePath, fileName_inf);
            destFile = System.IO.Path.Combine(targetPath, fileName_inf);
            if (!System.IO.File.Exists(destFile))
            {
                System.IO.File.Copy(sourceFile, destFile, true);
            }

            sourceFile = System.IO.Path.Combine(sourcePath, fileName_inf);
            destFile = System.IO.Path.Combine(SysInfPath, fileName_inf);
            if (!System.IO.File.Exists(destFile))
            {
                System.IO.File.Copy(sourceFile, destFile, true);
            }
            //=============copy fileName_bat ====================
            sourceFile = System.IO.Path.Combine(sourcePath, fileName_bat);
            destFile = System.IO.Path.Combine(targetPath, fileName_bat);
            if (!System.IO.File.Exists(destFile))
            {
                System.IO.File.Copy(sourceFile, destFile, true);
            }

        }
    }
}
