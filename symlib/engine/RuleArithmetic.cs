using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using symlib.model;

namespace symlib.engine {
    public class RuleArithmetic : Rule {
        internal RuleArithmetic() {
            Name = "Arithmetic";
        }

        internal override bool CanApply(Expression exp, out object clientData) {
            clientData = null;
            return
                exp.GetChildren().Count() > 0 &&
                exp.GetChildren().All(x => x.IsConstant);
        }

        internal override Expression Apply(Expression exp, object clientData) {
            double value;

            if (exp.IsBinary) {
                ExpressionBinary bin = exp.AsBinary;
                double left = bin.Left.AsConstant.Value;
                double right = bin.Right.AsConstant.Value;

                switch (bin.Operator) {
                    case BinaryOperator.Plus:
                        value = left + right;
                        break;
                    case BinaryOperator.Minus:
                        value = left - right;
                        break;
                    case BinaryOperator.Multiply:
                        value = left * right;
                        break;
                    case BinaryOperator.Divide:
                        // TODO: Divide by zero
                        value = left / right;
                        break;
                    case BinaryOperator.Exponent:
                        value = Math.Pow(left, right);
                        break;
                    case BinaryOperator.Root:
                        value = Math.Pow(left, 1.0 / right);
                        break;
                    case BinaryOperator.Log:
                        value = Math.Log(left, right);
                        break;
                    default:
                        throw new Exception("Unexpected: " + bin.Operator);
                }
            } else if (exp.IsUnary) {
                ExpressionUnary unary = exp.AsUnary;
                double child = unary.Child.AsConstant.Value;

                switch (unary.Operator) {
                    case UnaryOperator.Minus:
                        value = -child;
                        break;
                    case UnaryOperator.Derivative:
                        value = 0.0;
                        break;
                    default:
                        throw new Exception("Unexpected: " + unary.Operator);
                }
            } else
                throw new Exception("Unexpected - what else has children???");

            return new ExpressionConstant() {
                Value = value
            };
        }
    }
}
