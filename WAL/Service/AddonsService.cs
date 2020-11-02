using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
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

        public async Task<List<AddonListModel>> SearchSupportedInstalledAddons(string addonType, GameModel game)
        {
            var fingerprints = new List<string>();
            var searchFolders = IOHelper.GetAddonsDirectories(addonType.Equals(TwitchConstants.WoWRetail) ? WoWVersion.Retail : WoWVersion.Classic);

            var tasks = searchFolders.Select(async item => 
            {
                var matchingFiles = new List<string>();

                var fileInfoList = item.GetFileSystemInfos()
                    .Where(x => x.Extension.Equals(".toc"))
                    .Select(x => x.FullName).ToList();

                matchingFiles.AddRange(item.GetFileSystemInfos()
                    .Where(x => x.Extension.Equals(".xml") && x.Name.ToLowerInvariant().Contains("bindings."))
                    .Select(x => x.FullName).ToList());

                foreach (string path in fileInfoList)
                {
                    matchingFiles = await ProcessIncludeFile(matchingFiles, new FileInfo(path), game);
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
            });

            await Task.WhenAll(tasks);

            var addons = await new TwitchApiService().GetAddonsByFingerprint(fingerprints);

            if (addons != null && addons.ExactMatches.Count != 0)
            {
                var addonsServerInfo = await new TwitchApiService().GetAddonsInfo(addons.ExactMatches.Select(x => x.Id.ToString()).ToList());

                var categoryAvatars = await LoadCategoryAvatars(addonsServerInfo.Where(x => x.Categories.Count != 0).Select(x => x.Categories.Where(c => c.CategoryId == x.PrimaryCategoryId).First()).ToList());

                var resultModel = new List<AddonListModel>();
                addons.ExactMatches.ForEach(addon =>
                {
                    var addonModel = addonsServerInfo.Where(m => m.Id == addon.Id).First();

                    var currentFileName = string.Empty;

                    var currentAddonFileModel = addon.LatestFiles
                                            .Where(x => x.ProjectStatus == ProjectStatus.Approved)
                                            .Where(x => x.ReleaseType == ProjectFileReleaseType.Release)
                                            .OrderByDescending(x => x.FileDate)
                                            .FirstOrDefault();

                    if (addon.File.ReleaseType != ProjectFileReleaseType.Release && addon.File.ProjectStatus == ProjectStatus.Approved)
                    {
                        currentFileName = currentAddonFileModel.FileName;
                    }
                    else
                    {
                        currentFileName = addon.File.FileName;
                        currentAddonFileModel = addon.File;
                    }
                        

                    var displayName = addonModel.Name + Environment.NewLine + currentFileName;

                    var latestFile = addonModel.LatestFiles
                                        .Where(l => l.FileStatus == ProjectFileStatus.Approved)
                                        .Where(l => l.ReleaseType == ProjectFileReleaseType.Release)
                                        .Where(l => l.GameVersionFlavor.Equals(addonType))
                                        .Where(l => !l.IsAlternate)
                                        .OrderBy(o => o.FileDate)
                                        .Last();

                    var latestFileName = latestFile.FileName;

                    var status = currentFileName == latestFileName
                            ? AddonStatusType.UpToDate
                            : AddonStatusType.Update;

                    var avatarId = addonModel.Categories
                                            .Where(c => c.CategoryId == addonModel.PrimaryCategoryId)
                                            .FirstOrDefault()?.AvatarId;

                    var avatar = avatarId.HasValue 
                        ? categoryAvatars
                                    .Where(a => a.Id == avatarId)
                                    .First().Bitmap
                        : Properties.Resources.defaultAddonAvatar;

                    var item = new AddonListModel()
                    {
                        Id = addon.Id,
                        DisplayName = displayName,
                        FileDate = addon.File.FileDate,
                        FileId = addon.File.Id,
                        File = currentAddonFileModel,
                        LatestFile = latestFile,
                        LatestFileVersion = latestFileName,
                        LatestFileVesionFileId = latestFile.Id,
                        Name = addonModel.Name,
                        Author = addonModel.Authors.First().Name,
                        GameVersion = latestFile.GameVersion.FirstOrDefault() ?? string.Empty,
                        AddonAvatar = avatar,
                        StatusType = status,
                        StatusName = EnumHelper.GetEnumDescription(status)
                    };

                    resultModel.Add(item);
                });

                return resultModel;
            }
            return null;
        }

        private async Task<List<string>> ProcessIncludeFile(List<string> matchingFileList, FileInfo pIncludeFile, GameModel game)
        {
            if (!pIncludeFile.Exists || matchingFileList.Contains(pIncludeFile.FullName.ToLowerInvariant()))
                return matchingFileList;

            matchingFileList.Add(pIncludeFile.FullName.ToLowerInvariant());

            if (game.FileParsingRules.Count == 0)
                return matchingFileList;

            var input = (string)null;
            using (StreamReader streamReader = new StreamReader(pIncludeFile.FullName))
            {
                input = await streamReader.ReadToEndAsync();
                streamReader.Close();
            }

            var gameFileParsingRule = game.FileParsingRules.FirstOrDefault(p => p.FileExtension == pIncludeFile.Extension.ToLowerInvariant());

            if (gameFileParsingRule == null)
                return matchingFileList; 

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

                matchingFileList = await ProcessIncludeFile(matchingFileList, new FileInfo(fileName), game);
            }
            return matchingFileList;
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

        public async Task<AddonListModel> UpdateAddon(AddonListModel addon)
        {
            var result = addon;

            var downloadUrl = new Uri(addon.LatestFile.DownloadUrl);

            var wowAddonsFolder = IOHelper.GetAddonsFolder(addon.LatestFile.GameVersionFlavor == TwitchConstants.WoWRetail
                ? WoWVersion.Retail
                : WoWVersion.Classic);

            var foldesToDelete = addon.File.Modules.Select(x => Path.Combine(wowAddonsFolder, x.Foldername)).ToList();
            var foldesBackup = addon.File.Modules.Select(x => Path.Combine(wowAddonsFolder, $"_{x.Foldername}")).ToList();


            var zipPath = Path.GetTempFileName();

            try
            {
                await new TwitchApiService().DownloadAddon(downloadUrl, zipPath);

                IOHelper.MarkAsBackup(foldesToDelete);

                ZipFile.ExtractToDirectory(zipPath, wowAddonsFolder);

                result.DisplayName = addon.Name + Environment.NewLine + addon.LatestFile.FileName;
                result.StatusType = AddonStatusType.UpToDate;
                result.StatusName = EnumHelper.GetEnumDescription(addon.StatusType);
            }
            catch
            {
                IOHelper.MarkFromBackup(foldesBackup);
                return null;
            }
            finally
            {
                IOHelper.DeleteDirectory(foldesBackup);
            }

            return result;
        }
    }
}
