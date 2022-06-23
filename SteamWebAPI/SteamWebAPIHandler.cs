using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace discordbot.SteamWebAPI
{
	class SteamWebAPIHandler
	{
		const string STEAMID = "";
		//As always this should be saved to a config
		const string STEAMAPIKEY = "";
		

		//Grab a json from steamwebapi for a steamid and return a response object
		public Response GetSteamResponse(string steamId)
		{
			Response response;
			HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(String.Format
				("http://api.steampowered.com/IPlayerService/GetOwnedGames/v1/?key={0}&steamid={1}&include_appinfo=1&include_appinfo=1&include_played_free_games=1"
					, STEAMAPIKEY, steamId));
			//http://api.steampowered.com/IPlayerService/GetOwnedGames/v1/?key=D978D712803A5B78A29EBB942C66E0A6&steamid=76561198016211014&include_appinfo=1
			HttpWebResponse httpResponse = (HttpWebResponse) httpRequest.GetResponse();

			StreamReader sr = new StreamReader(httpResponse.GetResponseStream());
			
			string jsonResponse =  sr.ReadToEnd();
			ResponseInfo responseInfo = JsonConvert.DeserializeObject<ResponseInfo>(jsonResponse);
			response = responseInfo.response;
			httpResponse.Close();
			return response;
		}

		public List<string> SteamResponseToList(Response response)
		{
			List<Game> steamgames = response.Games;
			List<string> convertedList = new List<string>();
			
			foreach (Game game in steamgames)
			{
				convertedList.Add(game.name);
			}

			return convertedList;
		}

		public void PrepareAliases(string steamId)
		{
			//Clean response here into alias
			Response response = GetSteamResponse(steamId);
			List<string> games = SteamResponseToList(response);
			Dictionary<string, string> aliasTable = FileOps.ReadAliasFile();
			string alias;
			foreach (string game in games)
			{
				//Check our current alias table for already made games
				if (!aliasTable.ContainsValue(game))
				{
					//Prepare a clean alias for the game
					alias = game;
					//Remove all spaces
					alias = alias.Replace(" ", "");
					if (alias.Length <= 9)
					{
						//Is short enough, so just keep as alias.
						alias = alias.ToLower();
						alias.Insert(0, "s_");	//Mark as a steam add
						aliasTable.TryAdd(alias, game);
					}
					else
					{
						alias = Regex.Replace(alias, "([A-Z])(a-z)..", "$1$2");
						alias = alias.ToLower();
						alias.Insert(0, "s_");
						aliasTable.TryAdd(alias, game);
					}
				}
			}
			FileOps.SaveAliasFile(aliasTable);
		}

		//Returns the personalname associated with the steamID
		public string GetSteamPersonaName(string steamId)
		{
			SteamUser.Response response;
			HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(String.Format
				("http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={0}&steamids={1}"
					, STEAMAPIKEY, steamId));
			HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
			StreamReader sr = new StreamReader(httpResponse.GetResponseStream());
			

			string jsonResponse = sr.ReadToEnd();
			SteamUser.Rootobject rootobject = JsonConvert.DeserializeObject<SteamUser.Rootobject>(jsonResponse);

			response = rootobject.response;
			string id = response.players[0].personaname;
			httpResponse.Close();
			if (id == null)
			{
				return null;
			}
			else
			{
				return id;
			}
		}

		//Returns the steam id associated with the vanityurl steam name
		public string GetSteamID(string steamName)
		{
			steamName = steamName.ToLower();

			SteamUser.Response response;
			HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(String.Format
				("https://api.steampowered.com/ISteamUser/ResolveVanityURL/v1/?key={0}&vanityurl={1}"
					, STEAMAPIKEY, steamName));
			HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
			StreamReader sr = new StreamReader(httpResponse.GetResponseStream());
			

			string jsonResponse = sr.ReadToEnd();
			SteamUser.Rootobject rootobject = JsonConvert.DeserializeObject<SteamUser.Rootobject>(jsonResponse);
			response = rootobject.response;
			httpResponse.Close();
			if (response.success == 1)
			{
				//We've found a matching vanity url
				return response.steamid;
			}
			else
			{
				return null;
			}
		}



	}

}
