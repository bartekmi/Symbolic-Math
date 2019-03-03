using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using symlib.parser;

namespace symlib.parser {
    [TestClass]
    public class TokenizerTest {
        [TestMethod]
        public void Tokenizer_Numbers() {
            Test("1", 1.0);
            Test("1.23", 1.23);
            Test("-1", '-', 1.0);
            Test("-1.23", '-', 1.23);
        }

        private void Test(string input, params object[] expected) {
            List<object> actual = Tokenizer.Tokenize(input);
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
