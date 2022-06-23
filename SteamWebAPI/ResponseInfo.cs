using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace discordbot.SteamWebAPI
{
	[Serializable]
	public class ResponseInfo
	{
		public Response response;
	}

	[Serializable]
	public class Response
	{
		public int game_count;
		public List<Game> Games;
	}
}
