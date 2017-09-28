using System.Collections.Generic;

using libcmdline;

using NUnit.Framework;

namespace test
{
    [TestFixture]
    public class CommandLineArgsTests
    {
        [TestCase("-a", "testarg")]
        [TestCase("-a=testarg")]
        public void Option_WithArg(params string[] args)
        {
            CommandLineProcessor cmdline = new CommandLineProcessor();
            cmdline.RegisterOptionMatchHandler("a", true, (sender, e) => { });

            cmdline.ProcessCommandLineArgs(args);

            Assert.That(cmdline.ArgCount, Is.EqualTo(1));

            IEnumerable<string> emptylist = new List<string>(0);
            Assert.That(cmdline.InvalidArgs, Is.EquivalentTo(emptylist));
        }

        [TestCase("-a")]
        [TestCase("/a")]
        public void Option_NoArg_1(params string[] args)
        {
            CommandLineProcessor cmdline = new CommandLineProcessor();
            cmdline.RegisterOptionMatchHandler("a", (sender, e) => { });

            cmdline.ProcessCommandLineArgs(args);

            Assert.That(cmdline.ArgCount, Is.EqualTo(1));

            IEnumerable<string> emptylist = new List<string>(0);
            Assert.That(cmdline.InvalidArgs, Is.EquivalentTo(emptylist));
        }

        [TestCase("-a", "testarg", "-b")]
        [TestCase("-b", "-a", "testarg")]
        [TestCase("-a=testarg", "-b")]
        [TestCase("-b", "-a=testarg")]
        public void Option_NoArg_2(params string[] args)
        {
            CommandLineProcessor cmdline = new CommandLineProcessor();
            cmdline.RegisterOptionMatchHandler("a", true, (sender, e) => { });
            cmdline.RegisterOptionMatchHandler("b", (sender, e) => { });

            cmdline.ProcessCommandLineArgs(args);

            Assert.That(cmdline.ArgCount, Is.EqualTo(2));

            IEnumerable<string> emptylist = new List<string>(0);
            Assert.That(cmdline.InvalidArgs, Is.EquivalentTo(emptylist));
        }

        [TestCase("-a", "testarg")]
        [TestCase("-a=testarg")]
        public void Invalid_Argument_1(params string[] args)
        {
            CommandLineProcessor cmdline = new CommandLineProcessor();
            cmdline.RegisterOptionMatchHandler("a", (sender, e) => { });

            cmdline.ProcessCommandLineArgs(args);

            Assert.That(cmdline.ArgCount, Is.EqualTo(0));

            Assert.That(cmdline.InvalidArgs, Is.Not.Empty);
        }

        [TestCase("-a", "testarg", "-invalid")]
        [TestCase("-invalid", "-a", "testarg")]
        public void Invalid_Argument_2(params string[] args)
        {
            CommandLineProcessor cmdline = new CommandLineProcessor();
            cmdline.RegisterOptionMatchHandler("a", true, (sender, e) => { });

            cmdline.ProcessCommandLineArgs(args);

            Assert.That(cmdline.ArgCount, Is.EqualTo(1));

            Assert.That(cmdline.InvalidArgs.Count, Is.EqualTo(1));
        }
    }
}
