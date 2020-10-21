using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WAL.Static.Enums;

namespace WAL.Models
{
    public class GameDetectionHintModel
    {
        public int Id { get; set; }

        public GameDetectionHintType HintType { get; set; }

        public string HintPath { get; set; }

        public string HintKey { get; set; }

        public GameDetectionHintOption HintOptions { get; set; }

        public int GameId { get; set; }

    }
}
