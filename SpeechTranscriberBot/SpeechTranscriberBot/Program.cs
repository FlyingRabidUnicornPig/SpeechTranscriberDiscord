using System;
using System.Threading.Tasks;

namespace SpeechTranscriberBot
{
    class Program
    {
        static void Main(string[] args) =>
            new TranscriberBot().RunBotAsync().GetAwaiter().GetResult();
    }
}
