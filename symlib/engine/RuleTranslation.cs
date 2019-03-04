using symlib.model;
using symlib.parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace symlib.engine {
    public class RuleTranslation : Rule {

        private static Parser _parser = new Parser();

        internal Expression From { get; set; }
        internal Expression To { get; set; }

        internal RuleTranslation(string from, string to, string name = null) {
            From = _parser.Parse(from);
            To = _parser.Parse(to);
            Name = name;
        }

        #region Can Apply

        enum CanApplyResult {
            NoMatch,
            MatchThisLevel,
            MatchAndTerminate
        }

        internal override bool CanApply(Expression exp, out object clientData) {
            Dictionary<string, Expression>  mappings = new Dictionary<string, Expression>();
            clientData = mappings;
            return CanApply(From, exp, mappings);
        }

        private static bool CanApply(Expression template, Expression exp, Dictionary<string, Expression> mappings) {
            switch (CanApplyThis(template, exp, mappings)) {
                case CanApplyResult.NoMatch: return false;
                case CanApplyResult.MatchAndTerminate: return true;
                case CanApplyResult.MatchThisLevel: break;      // Will go on to check children
                default: throw new Exception("Unexpected");
            }

            Expression[] ruleChildren = template.GetChildren().ToArray();
            Expression[] expChildren = exp.GetChildren().ToArray();

            if (ruleChildren.Length != expChildren.Length)
                throw new Exception("Should never happen");

            for (int ii = 0; ii < ruleChildren.Length; ii++)
                if (!CanApply(ruleChildren[ii], expChildren[ii], mappings))
                    return false;

            return true;
        }

        private static CanApplyResult CanApplyThis(Expression template, Expression exp, Dictionary<string, Expression> mappings) {
            // A function f(x) or g(x) matches anything, even a constant
            if (template.IsFunc) {
                mappings[template.ToString()] = exp;
                return CanApplyResult.MatchAndTerminate;
            }

            // Match exact number
            if (template.IsConstant && exp.IsConstant && template.AsConstant.Value == exp.AsConstant.Value)
                return CanApplyResult.MatchAndTerminate;

            // A constant in the template can match another constant (symbolic or literal) in expression
            if (template.IsNonX && (exp.IsConstant || exp.IsNonX)) {
                mappings[template.ToString()] = exp;
                return CanApplyResult.MatchAndTerminate;
            }

            // x in template matches x in expression
            if (template.IsX && exp.IsX)
                return CanApplyResult.MatchAndTerminate;

            if (template.GetType() != exp.GetType())
                return CanApplyResult.NoMatch;

            if (template.IsUnary)
                return template.AsUnary.Operator == exp.AsUnary.Operator ?
                    CanApplyResult.MatchThisLevel : CanApplyResult.NoMatch;

            if (template.IsBinary)
                return template.AsBinary.Operator == exp.AsBinary.Operator ?
                    CanApplyResult.MatchThisLevel : CanApplyResult.NoMatch;

            return CanApplyResult.NoMatch;
        }
        #endregion

        #region Apply
        internal override Expression Apply(Expression exp, object clientData) {
            Dictionary<string, Expression> mappings = (Dictionary<string, Expression>)clientData;
            Expression output = Construct(To, mappings);
            return output;
        }

        private static Expression Construct(Expression template, Dictionary<string, Expression> mappings) {
            if (template.IsUnary)
                return new ExpressionUnary() {
                    Operator = template.AsUnary.Operator,
                    Child = Construct(template.AsUnary.Child, mappings)
                };
            else if (template.IsBinary)
                return new ExpressionBinary() {
                    Operator = template.AsBinary.Operator,
                    Left = Construct(template.AsBinary.Left, mappings),
                    Right = Construct(template.AsBinary.Right, mappings),
                };

            if (mappings.ContainsKey(template.ToString()))
                return mappings[template.ToString()];

            return template;
        }
        #endregion

        public override string ToString() {
            return Name == null ? string.Format("{0} -> {1}", From, To) : Name;
        }

    }
}
