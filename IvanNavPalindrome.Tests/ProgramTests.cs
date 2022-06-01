using NUnit.Framework;

namespace IvanNavPalindrome.Tests
{
    [TestFixture]
    public class ProgramTests
    {
        [OneTimeSetUp]
        public void StartTest()
        {
            Program.Main();
        }

        [TestCase("ABBA", true)]
        [TestCase("AbBa", true)]
        [TestCase("AOXOMOXOA", true)]
        [TestCase("AOXOmOXOA", true)]
        [TestCase("CHOCOLATE", false)]
        [TestCase("DOODLE", false)]
        public void IsPalindrome(params object[] args)
        {
            var res = Program.IsPalindrome((string)args[0]);

            Assert.IsTrue(res == (bool)args[1]);
        }

        [TestCase(null)]
        [TestCase("")]
        public void IsPalindromeThrowException(params object[] args)
        {
            Assert.Throws<ArgumentException>(() => Program.IsPalindrome((string)args[0]));
        }
    }
}
