using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.IO;
using System.Collections.Generic;

namespace discordbot
{
	public class Program
	{
		private DiscordSocketClient _client;
		private CommandService _commands;
		private CommandHandler _commandler;
		
		//private IServiceProvider _services;

		public static Task Main(string[] args) => new Program().MainAsync();
		public async Task MainAsync()
		{
			_client = new DiscordSocketClient(new DiscordSocketConfig() {GatewayIntents = GatewayIntents.All, AlwaysDownloadUsers = true });
			_client.Log += Log;
			_commands = new CommandService();
			_commands.Log += Log;
			_commandler = new CommandHandler(_client, _commands);
			
			//_services = new ServiceCollection;

			//  You can assign your bot token to a string, and pass that in to connect.
			//  This is, however, insecure, particularly if you plan to have your code hosted in a public repository.
			var token = "***";

			// Some alternative options would be to keep your token in an Environment Variable or a standalone file.
			// var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
			// var token = File.ReadAllText("token.txt");
			// var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;

			await _client.LoginAsync(TokenType.Bot, token);
			await _client.StartAsync();
			

			await _commandler.InstallCommandsAsync();
			await _client.SetGameAsync("fugg :DDD");
			// Block this task until the program is closed.
			await Task.Delay(-1);
		}

		private Task Log(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}

	}

}
