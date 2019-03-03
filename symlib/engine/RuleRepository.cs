using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace symlib.engine {
    internal static class RuleRepository {
        internal static IEnumerable<Rule> GetRules() {
            return new List<Rule>() {

                //===================================================================================
                // Derivative Rules
                //===================================================================================

                //// General Rules
                //new Rule("(f(x) + g(x))'", "(f(x))' + (g(x))'", "Derivative of sum"),
                //new Rule("(f(x) - g(x))'", "(f(x))' - (g(x))'", "Derivative of difference"),
                //new Rule("(a * f(x))'", "a * (f(x))'", "Multiply derivative by constant"),

                //// Product/Quotient
                //new Rule("(f(x) * g(x))'", "(f(x))' * g(x) + f(x) * (g(x))'", "Product Rule"),
                //new Rule("(f(x) / g(x))'", "((f(x))' * g(x) - f(x) * (g(x))') / g(x) ^ 2", "Quotient Rule"),

                // Chain Rule is tricky - consider new binary operator of(f,g)
                //new Rule("(f(g(x)))'", "(f(g(x))", "Chain Rule"),

                // Power
                new Rule("c'", "0"),
                new Rule("x'", "1"),
                new Rule("(x^n)'", "n * x^(n-1)"),

                // Trig
                new Rule("(sin(x))'", "cos(x)"),
                new Rule("(cos(x))'", "-sin(x)"),
                new Rule("(tan(x))'", "1 / (cos(x))^2"),
            };
        }
    }
}
