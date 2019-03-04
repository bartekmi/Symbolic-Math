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
                // Arithmetic Rules
                //===================================================================================
                new RuleArithmetic(),
                new RuleTranslation("0 + f(x)", "f(x)", "Dropping 0 + ..."),
                new RuleTranslation("f(x) + 0", "f(x)", "Dropping ... + 0"),
                new RuleTranslation("f(x) ^ 1", "f(x)", "Dropping ^ 1"),

                //===================================================================================
                // Derivative Rules
                //===================================================================================

                //// General Rules
                new RuleTranslation("(f(x) + g(x))'", "(f(x))' + (g(x))'", "Derivative of sum"),
                new RuleTranslation("(f(x) - g(x))'", "(f(x))' - (g(x))'", "Derivative of difference"),
                //new RuleTranslation("(a * f(x))'", "a * (f(x))'", "Multiply derivative by constant"),

                //// Product/Quotient
                //new RuleTranslation("(f(x) * g(x))'", "(f(x))' * g(x) + f(x) * (g(x))'", "Product Rule"),
                //new RuleTranslation("(f(x) / g(x))'", "((f(x))' * g(x) - f(x) * (g(x))') / g(x) ^ 2", "Quotient Rule"),

                // Chain Rule is tricky - consider new binary operator of(f,g)
                //new Rule("(f(g(x)))'", "(f(g(x))", "Chain Rule"),

                // Power
                new RuleTranslation("c'", "0"),
                new RuleTranslation("x'", "1"),
                new RuleTranslation("(x^n)'", "n * x^(n-1)"),

                // Trig
                new RuleTranslation("(sin(x))'", "cos(x)"),
                new RuleTranslation("(cos(x))'", "-sin(x)"),
                new RuleTranslation("(tan(x))'", "1 / (cos(x))^2"),
            };
        }
    }
}
