using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace symlib.model {
    public abstract class Expression {
        internal bool BrackedDuringParsing;
        internal virtual IEnumerable<Expression> GetChildren() { return new Expression[0]; }
        public abstract bool NeedsBrackets();

        public string BracketIfNecessary() {
            if (NeedsBrackets())
                return string.Format("({0})", this);
            else
                return this.ToString();
        }

        internal bool IsConstant { get { return this is ExpressionConstant; } }
        internal bool IsVariable { get { return this is ExpressionVariable; } }
        internal bool IsUnary { get { return this is ExpressionUnary; } }
        internal bool IsBinary { get { return this is ExpressionBinary; } }

        internal ExpressionConstant AsConstant { get { return this as ExpressionConstant; } }
        internal ExpressionVariable AsVariable { get { return this as ExpressionVariable; } }
        internal ExpressionUnary AsUnary { get { return this as ExpressionUnary; } }
        internal ExpressionBinary AsBinary { get { return this as ExpressionBinary; } }

        internal bool IsDiffConstant {
            get {
                return
                    this is ExpressionConstant ||
                    this is ExpressionVariable && (this as ExpressionVariable).Name != "x";
            }
        }

        internal bool IsDiffX {
            get {
                return this is ExpressionVariable && (this as ExpressionVariable).Name == "x";
            }
        }
    }
}
