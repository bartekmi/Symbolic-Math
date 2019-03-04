using symlib.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace symlib.parser {

    public class Parser {

        public Expression Parse(string input) {
            List<object> tokenList = Tokenizer.Tokenize(input);
            tokenList.Reverse();
            Stack<object> tokens = new Stack<object>(tokenList);
            Expression expression = ParseExpression(tokens);

            if (tokens.Count > 0)
                throw new Exception("Unexpected token: " + tokens.Pop());

            return expression;
        }

        private Expression ParseExpression(Stack<object> tokens) {
            Expression previous = null;
            Expression current = null;
            BinaryOperator? binaryOp = null;

            while (tokens.Count > 0) {
                object token = tokens.Pop();

                // A string indicates a function or a variable
                if (token is string) {
                    string str = token as string;
                    //BinaryOperator? funcBinaryOp = ExpressionBinary.ParseOperator(str);

                    // Unary function
                    if (ExpressionUnary.IsUnaryOp(str, out UnaryOperator unaryOp)) {
                        AboutToCreateExpression(token, ref current);

                        Expect(tokens, '(');
                        Expression child = ParseExpression(tokens);
                        Expect(tokens, ')');

                        current = new ExpressionUnary() {
                            Operator = unaryOp,
                            Child = child
                        };
                    // Binary function
                    } else if (ExpressionBinary.IsBinaryOp(str, out BinaryOperator funcBinaryOp)) {
                        AboutToCreateExpression(token, ref current);

                        Expect(tokens, '(');
                        Expression left = ParseExpression(tokens);
                        Expect(tokens, ',');
                        Expression right = ParseExpression(tokens);
                        Expect(tokens, ')');

                        current = new ExpressionBinary() {
                            Operator = funcBinaryOp,
                            Left = left,
                            Right = right
                        };
                    // Variable
                    } else if (str.Length == 1) {
                        if (ExpressionFunc.FUNC_NAMES.Contains(str)) {
                            Expect(tokens, '(');
                            Expect(tokens, 'x');
                            Expect(tokens, ')');
                            AboutToCreateExpression(token, ref current);
                            current = new ExpressionFunc() {
                                Name = str
                            };
                        } else {
                            AboutToCreateExpression(token, ref current);
                            current = new ExpressionVariable() {
                                Name = str
                            };
                        }
                    } else
                        throw new Exception("Unexpected token: " + token);
                // Numerical constants
                } else if (token is double) {
                    AboutToCreateExpression(token, ref current);
                    current = new ExpressionConstant() {
                        Value = (double)token
                    };
                // Single character operator: +-*/^,()
                } else if (token is char) {
                    switch ((char)token) {
                        // Begin bracketed expression
                        case '(':
                            AboutToCreateExpression(token, ref current);
                            current = ParseExpression(tokens);
                            current.BrackedDuringParsing = true;
                            Expect(tokens, ')');
                            break;
                        // End bracketed expression
                        case ')':
                        case ',':       // Used for function-like binary expressions: e.g. Root(3, 27)  // Cube root of 27
                            if (current == null)
                                throw new Exception("Unexpected token: " + token);
                            tokens.Push(token);     // Will be Expect()-ed in parent call
                            return current;

                        // Derivative
                        case '\'':
                            if (current == null)
                                throw new Exception("Unexpected token: " + token);
                            MakeDerivativeExpression(ref current);
                            break;

                        // Unary or binary operator
                        case '+':
                        case '-':
                        case '*':
                        case '/':
                        case '^':
                            if (current == null || binaryOp != null)
                                if (token.Equals('-')) {
                                    // This is a unary minus. Interpret it as prefixing a multiply by negative one
                                    previous = new ExpressionConstant() { Value = -1.0 };
                                    binaryOp = BinaryOperator.Multiply;
                                    continue;
                                } else
                                    throw new Exception("Unexpected binary operator: " + token);

                            binaryOp = ExpressionBinary.ParseOperator(token.ToString());
                            previous = current;
                            current = null;
                            break;
                    }
                } else
                    throw new Exception(string.Format("Unexpected token type {0} of type {1}", token, token.GetType()));

                MakeBinaryExpression(ref previous, ref current, ref binaryOp);
            }

            if (binaryOp != null)
                throw new Exception("Dangling binary operator: " +
                    ExpressionBinary.ToSymbol(binaryOp.Value));

            if (current == null)
                throw new Exception("Empty expression");

            return current;
        }

        #region Make Derivative Expression
        private void MakeDerivativeExpression(ref Expression current) {
            // Find right-most expression un-bracketed
            ExpressionBinary parent = null;
            Expression expression = current;
            while (expression is ExpressionBinary && !expression.BrackedDuringParsing) {
                parent = expression as ExpressionBinary;
                expression = parent.Right;
            }

            // Create the unary derivative expression
            ExpressionUnary derivative = new ExpressionUnary() {
                Operator = UnaryOperator.Derivative,
                Child = expression,
            };

            if (expression == current) {
                current = derivative;
            } else {
                parent.Right = derivative;
            }
        }
        #endregion

        #region Make Binary Expression
        private void MakeBinaryExpression(ref Expression previous, ref Expression current, ref BinaryOperator? binaryOp) {
            if (previous != null && binaryOp != null && current != null) {

                current = new ExpressionBinary() {
                    Operator = binaryOp.Value,
                    Left = previous,
                    Right = current
                };

                // Adjust for precedence
                ExpressionBinary adjust = current as ExpressionBinary;
                while (FlipOrderDueToPrecedence(adjust, out ExpressionBinary adjustLeft)) {
                    adjust.Right = new ExpressionBinary() {
                        Operator = adjust.Operator,
                        Left = adjustLeft.Right,
                        Right = adjust.Right
                    };
                    adjust.Operator = adjustLeft.Operator;
                    adjust.Left = adjustLeft.Left;

                    adjust = adjust.Right as ExpressionBinary;
                }

                previous = null;
                binaryOp = null;
            }
        }

        private bool FlipOrderDueToPrecedence(ExpressionBinary parent, out ExpressionBinary left) {
            left = null;
            if (parent == null)
                return false;

            left = parent.Left as ExpressionBinary;
            if (left == null || left.BrackedDuringParsing)
                return false;

            return ExpressionBinary.GetPrecedence(parent.Operator) > ExpressionBinary.GetPrecedence(left.Operator);
        }
        #endregion

        #region Utils
        private void Expect(Stack<object> tokens, char expected) {
            if (tokens.Count == 0)
                throw new Exception("EOF but expected: " + expected);

            object token = tokens.Pop();

            if (token is char && (char)token == expected ||
                token is string && token.ToString().Length == 1 && token.ToString()[0] == expected) {
                // Good!
            } else
                throw new Exception(string.Format("Expected {0} but got {1}", expected, token));
        }

        private void AboutToCreateExpression(object token, ref Expression current) {
            if (current != null)
                throw new Exception("Unexpected token: " + token);
        }
        #endregion
    }
}
