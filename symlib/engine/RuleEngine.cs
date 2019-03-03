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
                foreach (Rule rule in _rules)
                    if (rule.CanApply(exp, out Dictionary<string, Expression> mappings)) {
                        Console.Write(string.Format("Rule: {0}   =====>   ", rule));

                        exp = rule.Apply(exp, mappings);
                        madeProgress = true;

                        Console.WriteLine(exp);
                    }
            }

            return exp;
        }
    }
}
