using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace symlib.model {
    public class ExpressionVariable : Expression {
        public string Name { get; set; }

        public override bool NeedsBrackets() {
            return false;
        }

        public override string ToString() {
            return Name;
        }
    }
}
