using symlib.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace symlib.engine {
    public class RuleEngine {

        private IEnumerable<Rule> _rules;

        public RuleEngine() {
            _rules = RuleRepository.GetRules();
        }

        public Expression Solve(Expression exp) {

            Console.WriteLine("Solving: " + exp);

            // Brute force approach is to keep applying rules until nothing more can be done
            bool madeProgress = true;
            int pass = 1;

            while (madeProgress) {
                Console.WriteLine("Pass " + (pass++));
                madeProgress = false;

                foreach (Expression expression in Expression.DepthFirst(exp)) {
                    Expression replacement = ApplyRulesToExpression(expression);
                    if (replacement != null) {
                        madeProgress = true;
                        if (expression == exp)
                            exp = replacement;
                        else
                            expression.Parent.ReplaceChild(expression, replacement);
                    }
                }
            }

            return exp;
        }

        // If any rules can be applied, return the modified expression (only apply first rule)
        // Return null if nothing more can be done
        private Expression ApplyRulesToExpression(Expression exp) {
            foreach (Rule rule in _rules)
                if (rule.CanApply(exp, out object clientData)) {
                    Console.Write(string.Format("Rule: {0}   =====>   ", rule));

                    exp = rule.Apply(exp, clientData);

                    Console.WriteLine(exp);
                    return exp;
                }

            return null;
        }
    }
}
