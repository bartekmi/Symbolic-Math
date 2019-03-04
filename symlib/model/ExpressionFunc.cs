using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace symlib.model {
    public class ExpressionFunc : Expression {

        public static readonly string[] FUNC_NAMES = new string[] { "f", "g" };

        public string Name;

        public override bool NeedsBrackets() {
            return false;
        }

        public override string ToString() {
            return string.Format("{0}(x)", Name);
        }
    }
}
