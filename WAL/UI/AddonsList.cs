using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WAL.Static.Enums;
using WAL.Helpers;
using WAL.Models;
using WAL.Service;
using System.Text.RegularExpressions;

namespace WAL.UI
{
    public partial class AddonsList : Form
    {
        private FingerprintMatchResultModel _addons{ get; set; }
        private readonly string _pageType;

        public AddonsList(string pageType)
        {
            InitializeComponent();

            _pageType = pageType;
            _addons = null;

            SearchAddons();

            //Debug Grid
            //AddonsListGrid.Rows.Add(new string[] { "123", "addon1", "Update", "Verson", "1000", "Auther" });
            //AddonsListGrid.Rows.Add(new string[] { "123", "addon2", "Update", "", "" });
            //AddonsListGrid.Rows.Add(new string[] { "123", "addon3", "Update", "", "" });
            //AddonsListGrid.Rows.Add(new string[] { "123", "addon4", "Update", "", "" });
            //AddonsListGrid.Rows.Add(new string[] { "123", "addon5", "Update", "", "" });
            //AddonsListGrid.Rows.Add(new string[] { "123", "addon6", "Update", "", "" });
            //AddonsListGrid.Rows.Add(new string[] { "123", "addon7", "Update", "", "" });

            //panelWait.Hide();
        }

        private async void SearchAddons()
        {
            var fingerprints = new List<string>();
            var searchFolders = IOHelper.GetAddonsDirectories(WoWVersion.Retail);

            foreach (var addonFolder in searchFolders)
            {
                var matchingFiles = new List<string>();

                var fileInfoList = addonFolder.GetFileSystemInfos()
                    .Where(x => x.Extension.Equals(".toc", StringComparison.CurrentCultureIgnoreCase) ||
                                x.Extension.Equals(".xml;", StringComparison.CurrentCultureIgnoreCase))
                    .Select(x => x.FullName).ToList();

                foreach (string path in fileInfoList)
                {
                    ProcessIncludeFile(matchingFiles, new FileInfo(path), Program._game);
                }

                var longList = new List<long>();
                foreach (string path in matchingFiles)
                {
                    long normalizedFileHash = MurmurHash2Helper.ComputeNormalizedFileHash(path);
                    longList.Add(normalizedFileHash);
                }

                longList.Sort();

                var empty = string.Empty;

                foreach (long num in longList)
                    empty += num.ToString();

                var bytes = Encoding.ASCII.GetBytes(empty);
                var fingerprint = (long)MurmurHash2Helper.ComputeHash(bytes, false);
                fingerprints.Add(fingerprint.ToString());
            }

            _addons = await new TwitchApiService().GetAddonsByFingerprint(fingerprints);

            AddonsListGrid.Rows.Clear();

            if (_addons != null && _addons.ExactMatches.Count != 0)
            {
                var addonsServerInfo = new List<SearchResponceModel>();
                var tasks = _addons.ExactMatches.Select(async x => addonsServerInfo.Add(await new TwitchApiService().GetAddonInfo(x.Id)));
                await Task.WhenAll(tasks);

                //var gg = _addons.ExactMatches
                //    .Select(x => new DataGridViewRow().
                //    {
                //        x.Id.ToString(),
                //        string.Format("{0}{1}{2}", addonsServerInfo.Where(a => a.Id == x.Id).First().Name, Environment.NewLine, x.File.FileName)
                //    });

                //AddonsListGrid.Rows.AddRange(gg);
            }
        }

        public void ProcessIncludeFile(List<string> matchingFileList, FileInfo pIncludeFile, GameModel game)
        {
            if (!pIncludeFile.Exists || matchingFileList.Contains(pIncludeFile.FullName.ToLowerInvariant()))
                return;

            matchingFileList.Add(pIncludeFile.FullName.ToLowerInvariant());

            if (game.FileParsingRules.Count == 0)
                return;

            var input = (string)null;
            using (StreamReader streamReader = new StreamReader(pIncludeFile.FullName))
            {
                input = streamReader.ReadToEnd();
                streamReader.Close();
            }

            var gameFileParsingRule = game.FileParsingRules.FirstOrDefault(p => p.FileExtension == pIncludeFile.Extension.ToLowerInvariant());

            if (gameFileParsingRule == null)
                return;
            if (gameFileParsingRule.CommentStripRegex != null)
                input = gameFileParsingRule.CommentStripRegex.Replace(input, string.Empty);

            foreach (Match match in gameFileParsingRule.InclusionRegex.Matches(input))
            {
                var fileName = string.Empty;

                try
                {
                    var str = match.Groups[1].Value;

                    if (IOHelper.FilePathHasInvalidChars(str))
                        break;

                    fileName = Path.Combine(pIncludeFile.DirectoryName, str);
                }
                catch { break; }

                ProcessIncludeFile(matchingFileList, new FileInfo(fileName), game);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            SearchAddons();
        }
    }
}
