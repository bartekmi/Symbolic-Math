using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace symlib.model {
    public abstract class Expression {
        private Expression[] _children;

        public abstract bool NeedsBrackets();

        internal Expression Parent;
        internal bool BrackedDuringParsing;

        public string BracketIfNecessary() {
            if (NeedsBrackets())
                return string.Format("({0})", this);
            else
                return this.ToString();
        }

        internal bool IsConstant { get { return this is ExpressionConstant; } }
        internal bool IsVariable { get { return this is ExpressionVariable; } }
        internal bool IsUnary { get { return this is ExpressionUnary; } }
        internal bool IsBinary { get { return this is ExpressionBinary; } }
        internal bool IsFunc { get { return this is ExpressionFunc; } }

        internal ExpressionConstant AsConstant { get { return this as ExpressionConstant; } }
        internal ExpressionVariable AsVariable { get { return this as ExpressionVariable; } }
        internal ExpressionUnary AsUnary { get { return this as ExpressionUnary; } }
        internal ExpressionBinary AsBinary { get { return this as ExpressionBinary; } }
        internal ExpressionFunc AsFunc { get { return this as ExpressionFunc; } }

        #region Children
        internal IEnumerable<Expression> GetChildren() {
            return _children == null ? new Expression[0] : _children;
        }

        internal void SetChild(Expression child, int index, int childCount) {
            if (_children == null)
                _children = new Expression[childCount];
            _children[index] = child;
            child.Parent = this;
        }

        internal Expression GetChild(int index) {
            return _children[index];
        }

        internal void ReplaceChild(Expression old, Expression _new) {
            for (int ii = 0; ii < _children.Length; ii++) {
                if (_children[ii] == old) {
                    _children[ii] = _new;
                    _new.Parent = this;
                    return;
                }
            }

            throw new Exception("No such child: " + old);
        }
        #endregion

        internal bool IsX { get { return this is ExpressionVariable && (this as ExpressionVariable).Name == "x"; } }
        internal bool IsNonX { get { return this is ExpressionVariable && (this as ExpressionVariable).Name != "x"; } }

        internal static IEnumerable<Expression> DepthFirst(Expression exp) {
            List<Expression> items = new List<Expression>();
            exp.DepthFirst(items);
            return items;
        }

        private void DepthFirst(List<Expression> items) {
            foreach (Expression child in GetChildren())
                child.DepthFirst(items);
            items.Add(this);
        }
    }
}
