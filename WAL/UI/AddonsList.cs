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
using WAL.UI.Controls.Models;
using WAL.UI.Controls.Static.Enums;
using WAL.Static.Const;

namespace WAL.UI
{
    public partial class AddonsList : Form
    {
        private FingerprintMatchResultModel Addons{ get; set; }
        private List<CategoryAvatar> CategoryAvatars { get; set; }
        private readonly string PageType;

        public AddonsList(string pageType)
        {
            InitializeComponent();

            PageType = pageType;
            Addons = null;
            CategoryAvatars = new List<CategoryAvatar>();

            SearchAddons();
        }

        private async void SearchAddons()
        {
            var fingerprints = new List<string>();
            var searchFolders = IOHelper.GetAddonsDirectories(WoWVersion.Retail);
            var game = Program._game;

            foreach (var addonFolder in searchFolders)
            {
                var matchingFiles = new List<string>();

                var fileInfoList = addonFolder.GetFileSystemInfos()
                    .Where(x => x.Extension.Equals(".toc"))
                    .Select(x => x.FullName).ToList();

                matchingFiles.AddRange(addonFolder.GetFileSystemInfos()
                    .Where(x => x.Extension.Equals(".xml") && x.Name.ToLowerInvariant().Contains("bindings."))
                    .Select(x => x.FullName).ToList());

                foreach (string path in fileInfoList)
                {
                    ProcessIncludeFile(matchingFiles, new FileInfo(path), game);
                }

                matchingFiles.Sort();

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

            Addons = await new TwitchApiService().GetAddonsByFingerprint(fingerprints);

            if (Addons != null && Addons.ExactMatches.Count != 0)
            {
                var addonsServerInfo = await new TwitchApiService().GetAddonsInfo(Addons.ExactMatches.Select(x => x.Id.ToString()).ToList());

                await LoadCategoryAvatars(addonsServerInfo.Select(x => x.Categories.First()).ToList());

                gridContainer1.Clear();

                var rowsModel = new List<RowItemsModel>();
                Addons.ExactMatches.ForEach(addon => 
                {
                    var addonModel = addonsServerInfo.Where(m => m.Id == addon.Id).First();

                    var model = new RowItemsModel
                    {
                        Id = rowsModel.Count()
                    };

                    var imageModel = new RowItemModel
                    {
                        Name = addonModel.Categories.First().AvatarUrl,
                        Bitmap = CategoryAvatars.Where(a => a.Id == addonModel.Categories.First().AvatarId).First().Bitmap,
                        PanelType = PanelTypes.Image,
                    };

                    var nameModel = new RowItemModel
                    {
                        Name = addonModel.Name +
                                   Environment.NewLine +
                                   addon.File.FileName,
                        ContentAlignment = ContentAlignment.TopLeft
                    };

                    var lastfileModel = new RowItemModel
                    {
                    Name = addonModel.LatestFiles
                                        .Where(l => l.FileStatus == ProjectFileStatus.Approved)
                                        .Where(l => l.ReleaseType == ProjectFileReleaseType.Release)
                                        .Where(l => l.GameVersionFlavor.Equals(PageType))
                                        .Where(l => !l.IsAlternate)
                                        .OrderBy(o => o.FileDate)
                                        .Last().FileName,
                    };

                    var versionModel = new RowItemModel
                    {
                        Name = addonModel.LatestFiles
                                        .Where(l => l.ReleaseType == ProjectFileReleaseType.Release)
                                        .Where(l => l.GameVersionFlavor.Equals(PageType))
                                        .OrderBy(o => o.FileDate)
                                        .Last().GameVersion.FirstOrDefault() ?? string.Empty,
                        ContentAlignment = ContentAlignment.MiddleCenter
                    };

                    var authorModel = new RowItemModel
                    {
                        Name = addonModel.Authors.First().Name,
                        ContentAlignment = ContentAlignment.MiddleCenter
                    };

                    var status = addon.File.FileName == lastfileModel.Name
                            ? "Up to date"
                            : "Update";

                    var statusModel = new RowItemModel
                    {
                        Name = status,
                        ContentAlignment = ContentAlignment.MiddleCenter
                    };

                    model.PriorityOrder = status == "Update";

                    model.RowItems = new List<RowItemModel>
                    {
                        imageModel,
                        nameModel,
                        statusModel,
                        lastfileModel,
                        versionModel,
                        authorModel
                    };

                    rowsModel.Add(model);
                });

                gridContainer1.AddRange(rowsModel);
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

        private async Task<bool> LoadCategoryAvatars(IEnumerable<CategoryModel> categoryModels)
        {
            foreach (var item in categoryModels)
            {
                if (!CategoryAvatars.Any(x => x.Id == item.AvatarId))
                {
                    var bitmap = await new TwitchApiService().GetCategoryBitmap(new Uri(item.AvatarUrl));
                    CategoryAvatars.Add(new CategoryAvatar { Id = item.AvatarId, Url = item.AvatarUrl, Bitmap = bitmap });
                }
            }

            return true;
        }

        private void RefreshAddondButton_Click(object sender, EventArgs e)
        {
            SearchAddons();
        }
    }
}
