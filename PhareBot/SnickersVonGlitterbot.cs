using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace PhareBot
{
	public class SnickersVonGlitterbot : IHostedService
	{

		public SnickersVonGlitterbot(IConfiguration config, IHubContext<GlitterHub> glitterHub)
		{
			_BotPassword = config["AccessToken"];
			_GlitterHub = glitterHub;
		}

		private TwitchClient _Client;
		private readonly string _BotPassword;
		private readonly IHubContext<GlitterHub> _GlitterHub;

		public Task StartAsync(CancellationToken cancellationToken)
		{

			_Client = new TwitchClient();
			var glitterCreds = new ConnectionCredentials("TheFritzBot", _BotPassword);
      _Client.Initialize(glitterCreds);
			_Client.OnConnected += (sender, args) => _Client.JoinChannel("Csharpfritz");
			_Client.OnMessageReceived += _Client_OnMessageReceived;
			_Client.OnNewSubscriber += _Client_OnNewSubscriber;
			_Client.OnReSubscriber += _Client_OnReSubscriber;
			_Client.OnGiftedSubscription += _Client_OnGiftedSubscription;

			_Client.Connect();
			return Task.CompletedTask;

		}

		private void _Client_OnGiftedSubscription(object sender, TwitchLib.Client.Events.OnGiftedSubscriptionArgs e)
		{
			_GlitterHub.Clients.All.SendAsync("SubBoop");
		}

		private void _Client_OnReSubscriber(object sender, TwitchLib.Client.Events.OnReSubscriberArgs e)
		{
			_GlitterHub.Clients.All.SendAsync("SubBoop");
		}

		private void _Client_OnNewSubscriber(object sender, TwitchLib.Client.Events.OnNewSubscriberArgs e)
		{
			_GlitterHub.Clients.All.SendAsync("SubBoop");
		}

		private void _Client_OnMessageReceived(object sender, 
			TwitchLib.Client.Events.OnMessageReceivedArgs e)
		{

			//if (e.ChatMessage.Bits == 0) return;

			if (e.ChatMessage.Message.Contains("phareBoop"))
			{

				_GlitterHub.Clients.All.SendAsync("Boop", e.ChatMessage.Bits);
			}

		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_Client.Disconnect();
			return Task.CompletedTask;
		}
	}
}
