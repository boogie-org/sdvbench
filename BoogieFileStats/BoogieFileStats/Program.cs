using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Boogie;

namespace BoogieFileStats
{
    class Driver
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: BoogieFileStats.exe file-pattern1 file-pattern2 ...");
            }

            // Initialize Boogie
            CommandLineOptions.Install(new CommandLineOptions());
            CommandLineOptions.Clo.PrintInstrumented = true;

            foreach (var arg in args)
            {
                foreach (var file in System.IO.Directory.GetFiles(".", arg))
                {
                    Console.WriteLine("{0}", System.IO.Path.GetFileName(file));
                    var program = ReadAndResolve(file, true);
                    PrintStats(program);
                }
            }

        }

        // print various statistics of a Boogie program
        static void PrintStats(Program program)
        {
            // Number of procedures
            var procCount = program.TopLevelDeclarations.OfType<Procedure>().Count();
            Console.WriteLine("  Procedures: {0}", procCount);

            // Number of implementation
            var implCount = program.TopLevelDeclarations.OfType<Implementation>().Count();
            Console.WriteLine("  Implementations: {0}", implCount);

            // Number of global variables
            var gblCount = program.TopLevelDeclarations.OfType<GlobalVariable>().Count();
            Console.WriteLine("  Global variables: {0}", gblCount);

            // Number of map-typed global variables
            var mapCount = program.TopLevelDeclarations.OfType<GlobalVariable>()
                .Where(g => g.TypedIdent.Type.IsMap)
                .Count();
            Console.WriteLine("  Map-typed Global variables: {0}", mapCount);

            // Total number of basic blocks
            var blkCnt = 0;
            program.TopLevelDeclarations.OfType<Implementation>()
                .Iter(impl => blkCnt += impl.Blocks.Count);
            Console.WriteLine("  Basic blocks: {0}", blkCnt);

            // Total number of commands / instructions
            var cmdCnt = 0;
            program.TopLevelDeclarations.OfType<Implementation>()
                .Iter(impl => impl.Blocks
                .Iter(blk => cmdCnt += blk.Cmds.Count));
            Console.WriteLine("  Commands: {0}", cmdCnt);

            // Total number of call commands 
            var callCnt = 0;
            program.TopLevelDeclarations.OfType<Implementation>()
                .Iter(impl => impl.Blocks
                .Iter(blk => callCnt += blk.Cmds.OfType<CallCmd>().Count()));
            Console.WriteLine("  Call Commands: {0}", callCnt);
        }

        // Read in a Boogie program from a file
        static Program ParseProgram(string f)
        {
            Program p = new Program();

            try
            {
                if (Parser.Parse(f, new List<string>(), out p) != 0)
                {
                    Console.WriteLine("Failed to read " + f);
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
            return p;
        }

        // Read in a boogie program and do resolution and type-checking
        static Program ReadAndResolve(string filename, bool doTypecheck = true)
        {
            Program p = ParseProgram(filename);

            if (p == null)
            {
                throw new InvalidProg("Parse errors in " + filename);
            }

            if (p.Resolve() != 0)
            {
                throw new InvalidProg("Cannot resolve " + filename);
            }
            if (doTypecheck && p.Typecheck() != 0)
            {
                throw new InvalidProg("Cannot typecheck " + filename);
            }

            return p;
        }

        class InvalidProg : Exception
        {
            public InvalidProg(string message) : base(message) { }

        }

    }
}
