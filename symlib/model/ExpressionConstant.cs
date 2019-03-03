using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace symlib.model {
    public class ExpressionConstant : Expression {
        public double Value { get; set; }

        public override bool NeedsBrackets() {
            return false;
        }

        public override string ToString() {
            return Value.ToString();
        }
    }
}
