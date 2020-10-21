using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WAL.Static.Enums;

namespace WAL.Models
{
    public class AddonFileModel
    {
        private static List<ProjectFileStatus> _legacyDeletedFileStatues = new List<ProjectFileStatus>()
        {
          ProjectFileStatus.Rejected,
          ProjectFileStatus.Deleted,
          ProjectFileStatus.Archived
        };
        private string _fileNameOnDisk;

        public int Id { get; set; }

        public string DisplayName { get; set; }

        public string FileName { get; set; }

        public DateTime FileDate { get; set; }

        public long FileLength { get; set; }

        public ProjectFileReleaseType ReleaseType { get; set; }

        public ProjectFileStatus FileStatus { get; set; }

        public string DownloadUrl { get; set; }

        public bool IsAlternate { get; set; }

        public int AlternateFileId { get; set; }

        public List<AddonFileDependencyModel> Dependencies { get; set; }

        public bool IsAvailable { get; set; }

        public List<AddonFileModuleModel> Modules { get; set; }

        public long PackageFingerprint { get; set; }

        public List<string> GameVersion { get; set; }

        public List<SortableGameVersionModel> SortableGameVersion { get; set; }

        public string InstallMetadata { get; set; }

        public string Changelog { get; set; }

        public bool HasInstallScript { get; set; }

        public bool IsCompatibleWithClient { get; set; }

        public GameSectionPackageMapPackageType CategorySectionPackageType { get; set; }

        public ProjectRestrictProjectFileAccess RestrictProjectFileAccess { get; set; }

        public ProjectStatus ProjectStatus { get; set; }

        public int? RenderCacheId { get; set; }

        public int? FileLegacyMappingId { get; set; }

        public int ProjectId { get; set; }

        public int? ParentProjectFileId { get; set; }

        public int? ParentFileLegacyMappingId { get; set; }

        public int? FileTypeId { get; set; }

        public bool? ExposeAsAlternative { get; set; }

        public int? PackageFingerprintId { get; set; }

        public DateTime? GameVersionDateReleased { get; set; }

        public int? GameVersionMappingId { get; set; }

        public int? GameVersionId { get; set; }

        public int GameId { get; set; }

        public bool IsServerPack { get; set; }

        public int? ServerPackFileId { get; set; }

        public string GameVersionFlavor { get; set; }

        public string FileNameOnDisk
        {
            get
            {
                return this._fileNameOnDisk ?? this.FileName;
            }
            set
            {
                this._fileNameOnDisk = value;
            }
        }

        public ProjectFileStatus ConvertToLegacyStatus()
        {
            if (this.FileStatus == ProjectFileStatus.MalwareDetected)
                return ProjectFileStatus.MalwareDetected;
            if (this.ProjectStatus == ProjectStatus.Rejected || _legacyDeletedFileStatues.Contains(this.FileStatus) || this.RestrictProjectFileAccess == ProjectRestrictProjectFileAccess.Alpha && this.ReleaseType == ProjectFileReleaseType.Alpha || this.RestrictProjectFileAccess == ProjectRestrictProjectFileAccess.AlphaAndBeta && (this.ReleaseType == ProjectFileReleaseType.Alpha || this.ReleaseType == ProjectFileReleaseType.Beta))
                return ProjectFileStatus.Deleted;
            if (this.FileStatus == ProjectFileStatus.Approved)
                return this.IsCompatibleWithClient ? ProjectFileStatus.Approved : ProjectFileStatus.Released;
            if (this.FileStatus == ProjectFileStatus.Processing)
                return ProjectFileStatus.Processing;
            return this.FileStatus == ProjectFileStatus.ChangesRequired || this.FileStatus == ProjectFileStatus.UnderReview ? ProjectFileStatus.UnderReview : ProjectFileStatus.Deleted;
        }

        public bool IsLegacyDeleted()
        {
            return this.ConvertToLegacyStatus() == ProjectFileStatus.Deleted;
        }
    }
}
