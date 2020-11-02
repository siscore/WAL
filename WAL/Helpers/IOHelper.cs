using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WAL.Static.Enums;
using WAL.Properties;
using System.IO;

namespace WAL.Helpers
{
    public static class IOHelper
    {
        private const string _wowRetailPath = "_retail_\\Interface\\AddOns";
        private const string _wowClassicPath = "_classic_\\Interface\\AddOns";

        public static bool CheckWoWPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            if (!Directory.Exists(path))
                return false;

            var addonsPath = Path.Combine(path, "World of Warcraft Launcher.exe");

            if (!File.Exists(addonsPath))
                return false;

            return true;
        }

        public static string GetAddonsFolder(WoWVersion addonType)
        {
            var path = Settings.Default.WoWRetailPath;
            var addonsPath = Path.Combine(path, addonType == WoWVersion.Retail ? _wowRetailPath : _wowClassicPath);
            return addonsPath;
        }

        public static bool IfAddonExist(string name, WoWVersion addonType)
        {
            var addonPath = Path.Combine(GetAddonsFolder(addonType), name);
            return Directory.Exists(addonPath);
        }

        public static string WriteTempFile(byte[] data)
        {
            var tmpFile = Path.GetTempFileName();
            File.WriteAllBytes(tmpFile, data);
            return tmpFile;
        }

        public static List<DirectoryInfo> GetAddonsDirectories(WoWVersion addonType)
        {
            var info = new DirectoryInfo(GetAddonsFolder(addonType));

            return info.GetDirectories().ToList();
        }

        public static bool FilePathHasInvalidChars(string path)
        {
            if (!string.IsNullOrEmpty(path))
                return path.IndexOfAny(Path.GetInvalidPathChars()) >= 0;
            return false;
        }

        public static void MarkAsBackup(List<string> folders)
        {
            folders.ForEach(folder => 
            {
                var directory = new DirectoryInfo(folder);
                var newDirectory = Path.Combine(directory.Parent.FullName, $"_{directory.Name}");
                directory.MoveTo(newDirectory);
            });
        }

        public static void MarkFromBackup(List<string> folders)
        {
            folders.ForEach(folder =>
            {
                var directory = new DirectoryInfo(folder);
                var backupDirectory = new DirectoryInfo(Path.Combine(directory.Parent.FullName, $"_{directory.Name}"));

                if (backupDirectory.Exists)
                    backupDirectory.MoveTo(directory.FullName);
            });
        }

        public static void DeleteDirectory(List<string> folders)
        {
            folders.ForEach(folder => 
            {
                var di = new DirectoryInfo(folder);
                di.GetDirectories().ToList().ForEach(d => d.Delete(true));
                di.GetFiles().ToList().ForEach(f => f.Delete());
                di.Delete();
            });
        }
    }
}
