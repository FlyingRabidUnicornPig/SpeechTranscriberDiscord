using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Audio;
using Shiny.SpeechRecognition;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Shiny;

namespace SpeechTranscriberBot
{
    public class TranscriberBot
    {
        public const string PREFIX = "tr!";

        public static IVoiceChannel CurrentChannel { get; private set; }
        public static IObservable<string> Listener { get; private set; }

        private DiscordSocketClient client;
        private CommandService commands;
        private IServiceProvider services;

        #region Init
        public async Task RunBotAsync()
        {
            Setup();

            // Event Subscriptions
            client.Log += Log;

            await RegisterCommandsAsync();

            // Log in using the token stored on the host computer.
            await client.LoginAsync(TokenType.Bot,
                Environment.GetEnvironmentVariable("DiscordToken", EnvironmentVariableTarget.User));
            
            await client.StartAsync();

            // Wait indefinitely
            await Task.Delay(-1);
        }

        private void Setup()
        {
            client = new DiscordSocketClient();
            commands = new CommandService();

            services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(commands)
                .BuildServiceProvider();
            /*
            Listener = CrossSpeechRecognizer.ContinuousDictation();
            Listener.Subscribe(phrase => { });*/
        }
        #endregion

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);

            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            client.MessageReceived += HandleCommandAsync;

            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;

            // If there's nothing int he message or the message writer is a bot, ignore
            if (message is null || message.Author.IsBot)
                return;

            int argPos = 0;

            // Check if the message either has the chat prefix, or if this bot was @'d at
            if (message.HasStringPrefix(PREFIX, ref argPos)
                || message.HasMentionPrefix(client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(client, message);

                var result = await commands.ExecuteAsync(context, argPos, services);

                // If there's an error, write the error down.
                if (!result.IsSuccess)
                    Console.WriteLine(result.ErrorReason);
            }
        }

        public static async Task SetVoiceChannel(IVoiceChannel channel)
        {
            if (channel == null)
                await CurrentChannel.DisconnectAsync();

            CurrentChannel = channel;
            await CurrentChannel?.ConnectAsync(false, true);
        }
    }
}
