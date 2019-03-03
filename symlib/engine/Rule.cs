using symlib.model;
using symlib.parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace symlib.engine {
    public class Rule {

        private static Parser _parser = new Parser();

        internal string Name { get; set; }
        internal Expression From { get; set; }
        internal Expression To { get; set; }

        internal Rule(string from, string to, string name = null) {
            From = _parser.Parse(from);
            To = _parser.Parse(to);
            Name = name;
        }

        #region Can Apply
        internal bool CanApply(Expression exp, out Dictionary<string, Expression> mappings) {
            mappings = new Dictionary<string, Expression>();
            return CanApply(From, exp, mappings);
        }

        private static bool CanApply(Expression template, Expression exp, Dictionary<string, Expression> mappings) {
            if (!CanApplyThis(template, exp, mappings))
                return false;

            Expression[] ruleChildren = template.GetChildren().ToArray();
            Expression[] expChildren = exp.GetChildren().ToArray();

            if (ruleChildren.Length != expChildren.Length)
                throw new Exception("Should never happen");

            for (int ii = 0; ii < ruleChildren.Length; ii++)
                if (!CanApply(ruleChildren[ii], expChildren[ii], mappings))
                    return false;

            return true;
        }

        private static bool CanApplyThis(Expression template, Expression exp, Dictionary<string, Expression> mappings) {
            if (template.IsDiffConstant && exp.IsDiffConstant) {
                mappings[template.ToString()] = exp;
                return true;
            }

            if (template.IsDiffX && exp.IsDiffX)
                return true;

            if (template.GetType() != exp.GetType())
                return false;

            if (template.IsUnary)
                return template.AsUnary.Operator == exp.AsUnary.Operator;

            if (template.IsBinary)
                return template.AsBinary.Operator == exp.AsBinary.Operator;

            return false;
        }
        #endregion

        #region Apply
        internal Expression Apply(Expression exp, Dictionary<string, Expression> mappings) {
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
