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
        public void OptionWithArgTest(params string[] args)
        {
            CommandLineArgs cmdline = new CommandLineArgs();
            cmdline.RegisterOptionMatchHandler("a", true, (sender, e) => { });

            cmdline.ProcessCommandLineArgs(args);

            Assert.That(cmdline.ArgCount, Is.EqualTo(1));

            IEnumerable<string> emptylist = new List<string>(0);
            Assert.That(cmdline.InvalidArgs, Is.EquivalentTo(emptylist));
        }

        [TestCase("-a", "testarg", "-b")]
        [TestCase("-b", "-a", "testarg")]
        [TestCase("-a=testarg", "-b")]
        [TestCase("-b", "-a=testarg")]
        public void OptionNoArgTest(params string[] args)
        {
            CommandLineArgs cmdline = new CommandLineArgs();
            cmdline.RegisterOptionMatchHandler("a", true, (sender, e) => { });
            cmdline.RegisterOptionMatchHandler("b", (sender, e) => { });

            cmdline.ProcessCommandLineArgs(args);

            Assert.That(cmdline.ArgCount, Is.EqualTo(2));

            IEnumerable<string> emptylist = new List<string>(0);
            Assert.That(cmdline.InvalidArgs, Is.EquivalentTo(emptylist));
        }

        [TestCase("-a", "testarg")]
        [TestCase("-a=testarg")]
        public void InvalidArgumentTest(params string[] args)
        {
            CommandLineArgs cmdline = new CommandLineArgs();
            cmdline.RegisterOptionMatchHandler("a", (sender, e) => { });

            cmdline.ProcessCommandLineArgs(args);

            Assert.That(cmdline.ArgCount, Is.EqualTo(0));

            Assert.That(cmdline.InvalidArgs, Is.Not.Empty);
        }
    }
}
