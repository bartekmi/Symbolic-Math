using symlib.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace symlib.model {

    public enum UnaryOperator {
        Derivative,
        Minus,
        Sin,
        Cos,
        Tan,
        Sqrt,
        Etox
    }

    public class ExpressionUnary : Expression {
        public UnaryOperator Operator { get; set; }

        public Expression Child {
            get { return GetChild(0); }
            set { SetChild(value, 0, 1); }
        }

        public override bool NeedsBrackets() {
            return 
                Operator == UnaryOperator.Minus;
        }

        public override string ToString() {
            switch (Operator) {
                case UnaryOperator.Minus:
                    return string.Format("(-{0})", Child);
                case UnaryOperator.Derivative:
                    return string.Format("({0})'", Child);
                default:
                    return string.Format("{0}({1})", Operator.ToString().ToLower(), Child);
            }
        }

        internal static bool IsUnaryOp(string str, out UnaryOperator unaryOp) {
            unaryOp = UnaryOperator.Minus;
            UnaryOperator? op = EnumUtils.ParseNullable<UnaryOperator>(str);
            if (op == null)
                return false;

            unaryOp = op.Value;
            return true;
        }
    }
}
