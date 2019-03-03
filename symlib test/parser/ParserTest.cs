using Microsoft.VisualStudio.TestTools.UnitTesting;
using symlib.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace symlib.parser {
    [TestClass]
    public class ParserTest {
        [TestMethod]
        public void Parser_Constant() {
            Test("1");
            Test("1.23");
        }

        [TestMethod]
        public void Parser_Variable() {
            Test("a");
            Test("b");
        }

        [TestMethod]
        public void Parser_SimpleBinary() {
            Test("2 + 3");
            Test("6 + a");
            Test("a - b");
            Test("a * b");
            Test("a / b");
            Test("a ^ b");
        }

        [TestMethod]
        public void Parser_MultipleBinary() {
            Test("2 + 3 + 4", "(2 + 3) + 4");
            Test("2 + a + 4 + b", "((2 + a) + 4) + b");
            Test("a + b - c + d", "((a + b) - c) + d");
        }

        [TestMethod]
        public void Parser_Brackets() {
            Test("(a + b) - (c + d)");
            Test("a / (b / (c / d))");
        }

        [TestMethod]
        public void Parser_Precedence() {
            Test("(a + b) * c", "(a + b) * c");             // Brackets override higher multiplication precedence
            Test("a + b * c", "a + (b * c)");
            Test("a * b + c * d", "(a * b) + (c * d)");
            Test("q + a * b^c * d^e", "q + ((a * (b ^ c)) * (d ^ e))");
        }

        [TestMethod]
        public void Parser_BinaryFunc() {
            Test("root(3, 27)");
        }

        [TestMethod]
        public void Parser_Unary() {
            Test("a'", "(a)'");
            Test("a * b'", "a * (b)'");
            Test("(a ^ b)'", "(a ^ b)'");
            Test("sin(x + k)");
            Test("cos(x + k)");
            Test("tan(x + k)");
            Test("sqrt(x + k)");
            Test("etox(x + k)");
        }

        [TestMethod]
        public void Parser_UnaryMinus() {
            Test("-a", "-1 * a");
            Test("-1", "-1 * 1");
            Test("-1.23", "-1 * 1.23");
        }

        [TestMethod]
        public void Parser_Errors() {
            TestError("", "Empty expression");
            TestError("+", "Unexpected binary operator: +");
            TestError("1 +", "Dangling binary operator: +");
            TestError("1 ++ 2", "Unexpected binary operator: +");
            TestError("1 2", "Unexpected token: 2");
            TestError("(", "Empty expression");
            TestError(")", "Unexpected token: )");
            TestError("(1", "EOF but expected: )");
            TestError("(1 + 2))", "Unexpected token: )");
            TestError("blurg", "Unexpected token: blurg");
            TestError("root(,)", "Unexpected token: ,");
        }

        private void Test(string input, string expected = null) {
            if (expected == null)
                expected = input;

            Expression expression = new Parser().Parse(input);
            string output = expression.ToString();
            Assert.AreEqual(expected, output);
        }

        private void TestError(string input, string error) {
            try {
                new Parser().Parse(input);
                Assert.Fail("No error");
            } catch (Exception e) {
                Assert.AreEqual(error, e.Message);
            }
        }
    }
}
