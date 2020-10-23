using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WAL.Models;
using WAL.Static.Const;

namespace WAL.Service
{
    public class AddonsService
    {
        public async Task<GameModel> LoadData(int GameId) => await new TwitchApiService().GetGame(GameId);
    }
}
