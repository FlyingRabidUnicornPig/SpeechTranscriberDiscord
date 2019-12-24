using Discord;
using Discord.Commands;
using System.Threading.Tasks;


namespace SpeechTranscriberBot
{
    public class TranscriberBotCommands : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task PingAsync()
        {
            await ReplyAsync("Hello world!");
        }

        [Command("join", RunMode = RunMode.Async)]
        public async Task Join(IVoiceChannel channel = null)
        {
            channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
            
            if (channel == null)
            {
                await Context.Channel.SendMessageAsync("User is not in channel and/or a channel was not provided as an argument.");
                return;
            }

            await TranscriberBot.SetVoiceChannel(channel);

            await Context.Channel.SendMessageAsync($"Successfully connected, please use \"{TranscriberBot.PREFIX}transcribe [Output Channel]\" to start transcribing.");
        }

        [Command("leave", RunMode = RunMode.Async)]
        public async Task Leave()
        {
            await TranscriberBot.CurrentChannel.DisconnectAsync();
            await TranscriberBot.SetVoiceChannel(null);
        }
    }
}
