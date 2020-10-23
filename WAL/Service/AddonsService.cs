using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WAL.Helpers;
using WAL.Models;
using WAL.Static.Const;
using WAL.Static.Enums;
using WAL.UI.Controls.Models;
using WAL.UI.Controls.Static.Enums;

namespace WAL.Service
{
    public class AddonsService
    {
        public async Task<GameModel> LoadData(int GameId) => await new TwitchApiService().GetGame(GameId);

        public async Task<List<RowItemsModel>> SearchSupportedInstalledAddons(string addonType, GameModel game)
        {
            var fingerprints = new List<string>();
            var searchFolders = IOHelper.GetAddonsDirectories(addonType.Equals(TwitchConstants.WoWRetail) ? WoWVersion.Retail : WoWVersion.Classic);

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

            var addons = await new TwitchApiService().GetAddonsByFingerprint(fingerprints);

            if (addons != null && addons.ExactMatches.Count != 0)
            {
                var addonsServerInfo = await new TwitchApiService().GetAddonsInfo(addons.ExactMatches.Select(x => x.Id.ToString()).ToList());

                var categoryAvatars = await LoadCategoryAvatars(addonsServerInfo.Select(x => x.Categories.First()).ToList());

                var rowsModel = new List<RowItemsModel>();
                addons.ExactMatches.ForEach(addon =>
                {
                    var addonModel = addonsServerInfo.Where(m => m.Id == addon.Id).First();

                    var model = new RowItemsModel
                    {
                        Id = rowsModel.Count()
                    };

                    var imageModel = new RowItemModel
                    {
                        Name = addonModel.Categories.First().AvatarUrl,
                        Bitmap = categoryAvatars.Where(a => a.Id == addonModel.Categories.First().AvatarId).First().Bitmap,
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
                                        .Where(l => l.GameVersionFlavor.Equals(addonType))
                                        .Where(l => !l.IsAlternate)
                                        .OrderBy(o => o.FileDate)
                                        .Last().FileName,
                    };

                    var versionModel = new RowItemModel
                    {
                        Name = addonModel.LatestFiles
                                        .Where(l => l.ReleaseType == ProjectFileReleaseType.Release)
                                        .Where(l => l.GameVersionFlavor.Equals(addonType))
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
                            ? AddonStatusType.UpToDate
                            : AddonStatusType.Update;

                    var statusModel = new RowItemModel
                    {
                        Name = EnumHelper.GetEnumDescription(status),
                        ContentAlignment = ContentAlignment.MiddleCenter,
                        PanelType = PanelTypes.Status,
                        AddonStatusType = status
                    };

                    model.PriorityOrder = status == AddonStatusType.Update;

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

                return rowsModel;
            }
            return null;
        }

        private void ProcessIncludeFile(List<string> matchingFileList, FileInfo pIncludeFile, GameModel game)
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

        private async Task<List<CategoryAvatar>> LoadCategoryAvatars(List<CategoryModel> categoryModels)
        {
            var result = new List<CategoryAvatar>();

            var tasks = categoryModels.Select(async item => 
            {
                var bitmap = await new TwitchApiService().GetCategoryBitmap(new Uri(item.AvatarUrl));
                result.Add(new CategoryAvatar { Id = item.AvatarId, Url = item.AvatarUrl, Bitmap = bitmap });
            });
            await Task.WhenAll(tasks);

            return result;
        }
    }
}
