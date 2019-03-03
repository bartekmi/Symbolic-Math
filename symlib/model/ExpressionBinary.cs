using symlib.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace symlib.model {

    public enum BinaryOperator {
        Plus,
        Minus,
        Multiply,
        Divide,
        Exponent,
        Log,
        Root,
    }

    // Arranged in reverse BEDMAS order so that numerically higher value is, in fact, 
    // the stronger precedence (so brackets trump everything else)
    public enum Precedence {
        AdditionSubtraction,
        DivisionMultiplication,
        Exponents,
        Brackets,
    }

    public class ExpressionBinary : Expression {
        public BinaryOperator Operator { get; set; }
        public Expression Left { get; set; }
        public Expression Right { get; set; }

        public override string ToString() {
            if (NeedsBrackets())
                return string.Format("{0} {1} {2}",
                    Left.BracketIfNecessary(),
                    ToSymbol(Operator),
                    Right.BracketIfNecessary());

            return string.Format("{0}({1}, {2})", ToSymbol(Operator), Left, Right);
        }

        public override bool NeedsBrackets() {
            switch (Operator) {
                case BinaryOperator.Log:
                case BinaryOperator.Root:
                    return false;
                default:
                    return true;
            }
        }

        internal override IEnumerable<Expression> GetChildren() {
            return new Expression[] { Left, Right };
        }

        internal static bool IsBinaryOp(string str, out BinaryOperator unaryOp) {
            unaryOp = BinaryOperator.Plus;
            BinaryOperator? op = EnumUtils.ParseNullable<BinaryOperator>(str);
            if (op == null)
                return false;

            unaryOp = op.Value;
            return true;
        }

        internal static string ToSymbol(BinaryOperator op) {
            switch (op) {
                case BinaryOperator.Plus: return "+";
                case BinaryOperator.Minus: return "-";
                case BinaryOperator.Multiply: return "*";
                case BinaryOperator.Divide: return "/";
                case BinaryOperator.Exponent: return "^";
                case BinaryOperator.Log: return "log";
                case BinaryOperator.Root: return "root";
                default:
                    throw new Exception("Invalid: " + op);
            }
        }

        internal static BinaryOperator? ParseOperator(string op) {
            switch (op.ToLower()) {
                case "+": return BinaryOperator.Plus;
                case "-": return BinaryOperator.Minus;
                case "*": return BinaryOperator.Multiply;
                case "/": return BinaryOperator.Divide;
                case "^": return BinaryOperator.Exponent;
                case "log": return BinaryOperator.Log;
                case "root": return BinaryOperator.Root;
                default: return null;
            }
        }

        internal static Precedence GetPrecedence(BinaryOperator op) {
            switch (op) {
                case BinaryOperator.Exponent:
                    return Precedence.Exponents;
                case BinaryOperator.Divide:
                case BinaryOperator.Multiply:
                    return Precedence.DivisionMultiplication;
                case BinaryOperator.Plus: 
                case BinaryOperator.Minus:
                    return Precedence.AdditionSubtraction;
                default:
                    return Precedence.Brackets; // Things like Log, Root act same as if surrounded by brackets
            }
        }
    }
}
