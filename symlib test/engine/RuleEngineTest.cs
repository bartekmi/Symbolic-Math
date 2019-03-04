using Microsoft.VisualStudio.TestTools.UnitTesting;
using symlib.model;
using symlib.parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace symlib.engine {

    [TestClass]
    public class RuleEngineTest {

        private RuleEngine _engine = new RuleEngine();
        private Parser _parser = new Parser();

        [TestMethod]
        public void RuleEngine_Trig() {
            Test("sin(x)'", "cos(x)");
            Test("cos(x)'", "-1 * sin(x)");
            Test("tan(x)'", "1 / (cos(x) ^ 2)");
        }

        [TestMethod]
        public void RuleEngine_Power() {
            Test("c'", "0");
            Test("x'", "1");
            Test("(x^n)'", "n * (x ^ (n - 1))");
            Test("(x^3)'", "3 * (x ^ 2)");
        }

        [TestMethod]
        public void RuleEngine_PlusMinus() {
            Test("(c + sin(x))'", "cos(x)");
            Test("(x ^ 2 + sin(x))'", "(2 * x) + cos(x)");
            Test("(1 + x + x ^ 2 + x ^ 3)'", "(1 + (2 * x)) + (3 * (x ^ 2))");
            Test("(1 - x)'", "-1");
        }

        private void Test(string input, string expected) {
            Expression inputExpr = _parser.Parse(input);
            Expression output = _engine.Solve(inputExpr);

            Assert.AreEqual(expected, output.ToString());
        }
    }
}
