using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace discordbot
{
	public class GameLibrary
	{
		private readonly ulong _owner;
		[JsonProperty("games")]
		private List<string> _games = new List<string>();

		public GameLibrary(ulong owner)
		{
			_owner = owner;
		}

		public ulong Owner => _owner;

		public bool AddGame(string gameName)
		{
			//See if we already have proper name
			if (!_games.Contains(gameName))
			{
				//Dictionary<ulong, GameLibrary> libraryDic;
				//libraryDic = FileOps.MakeDictionary();
				////Check to see if read file makes sense
				//if (libraryDic == null)
				//{
				//	libraryDic = new Dictionary<ulong, GameLibrary>();
				//}

				//clean name
				Dictionary<string, string> catalog = FileOps.ReadAliasFile();
				//string ignore;
				//Is this an alias for a game we already have? Need a much better way to do this
				//catalog.TryGetValue(gameName, out ignore);
				//if (ignore == null)
				//{
				//	//Game does not exist in gamescatalog or must be full game name
				//	return false;
				//	//Might be good to pass message instead
				//}

				//if (_games.Contains(ignore))
				//{
				//	return false;
				//}
				//gameName = gameName.Trim();
				if (catalog.ContainsKey(gameName))
				{
					//Found matching game name in catalog
					//Is an alias, so change to proper name
					gameName = catalog.GetValueOrDefault(gameName);
					_games.Add(gameName);
					_games.Sort();
					return true;
				}
				else if (catalog.ContainsValue(gameName))
				{
					//Is the proper game name, so add
					_games.Add(gameName);
					_games.Sort();
					return true;
				}
				else
				{
					//Failed to find matching game name in catalog to add
					return false;
				}

				//libraryDic[Owner]
				//FileOps.SaveDictionary(libraryDic);
			}
			else
			{
				//throw new Exception(gameName + " already exists in library.");
				//canned error message here please
				return false;
			}

		}


		public bool RemoveGame(string gameName)
		{
			if (_games.Contains(gameName))
			{
				_games.Remove(gameName);
				return true;
			}
			else
			{
				//throw new Exception("No game matching \"" + gameName + " \" exists.");
				//Canned error message here please
				return false;
			}
		}

		public List<string> GetGames()
		{
			return this._games;
		}

		public List<string> Match(GameLibrary other)
		{
			List<string> matches = new List<string>();
			Array listA = _games.ToArray<string>();
			for (int i=0; i < listA.Length; i++)
			{
				
				Array listB = other._games.ToArray<string>();
				for (int j = 0; j < listB.Length; j++)
				{
					if (listA.GetValue(i).Equals(listB.GetValue(j)))
					{
						matches.Add(listA.GetValue(i).ToString());
					}
				}
			}
			return matches;
		}
	}

	public static class GameLibraryHelper
	{
		public static bool FindLibrary(ulong dId)
		{
			return false;
		}

		public static void RefLibrary(ulong dId)
		{

		}
	}
}
