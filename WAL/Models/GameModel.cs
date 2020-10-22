using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WAL.Static.Enums;

namespace WAL.Models
{
    public class GameModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public DateTime DateModified { get; set; }

        public List<GameFileModel> GameFiles { get; set; }

        public List<GameDetectionHintModel> GameDetectionHints { get; set; }

        public List<GameFileParsingRuleModel> FileParsingRules { get; set; }

        public List<CategorySectionModel> CategorySections { get; set; }

        public long MaxFreeStorage { get; set; }

        public long MaxPremiumStorage { get; set; }

        public long MaxFileSize { get; set; }

        public string AddonSettingsFolderFilter { get; set; }

        public string AddonSettingsStartingFolder { get; set; }

        public string AddonSettingsFileFilter { get; set; }

        public string AddonSettingsFileRemovalFilter { get; set; }

        public bool SupportsAddons { get; set; }

        public bool SupportsPartnerAddons { get; set; }

        public SupportedClientConfiguration SupportedClientConfiguration { get; set; }

        public bool SupportsNotifications { get; set; }

        public int ProfilerAddonId { get; set; }

        public int TwitchGameId { get; set; }

        public int ClientGameSettingsId { get; set; }
    }
}
