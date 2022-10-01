using KTXCore;

namespace Paifu
{
    static class Info
    {
        public const string Major = "0";
        public const string Minor = "1";
        public const string Build = "0";

        public const string Version = Major + "." + Minor + "." + Build + ".0";
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Init(80, 32);
            Console.Theme = Theme.Dark;
            Console.Title = "Paifu";
            Console.UpdatesPerSecond = 60;
            new PaifuViewer().Start();
        }
    }
}
