using symlib.model;
using symlib.parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace symlib.engine {
    public abstract class Rule {

        internal abstract bool CanApply(Expression exp, out object clientData);
        internal abstract Expression Apply(Expression exp, object clientData);

        internal string Name { get; set; }
    }
}
