using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NCmd;

namespace BruteScalp
{
    class Program
    {
        // setup the command line argument parser.
        private static readonly ArgumentParser ArgParse = new ArgumentParser() {
            { "h|?|help", "Display usage message and exit.", v => ShowHelp () },
            { "v|version", "Display version information.", v=>ShowVersion() }
        };

        static void Main(string[] args)
        {
            ArgParse.Parse(args);
            var ds = new InteractiveShell();
            ds.CmdLoop();
        }

        private static void ShowHelp()
        {
            var ap = new AutoProgramMetaData(typeof(Program).Assembly);
            Cmd.WriteUsageStatement(ap, ArgParse, Console.Out);
            Environment.Exit(0);
        }

        private static void ShowVersion()
        {
            var ap = new AutoProgramMetaData(typeof(Program).Assembly);
            Cmd.WriteVersionStatement(ap, Console.Out);
            Environment.Exit(0);
        }
    }
}
