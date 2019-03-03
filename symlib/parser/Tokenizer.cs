using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace symlib.parser {

    internal static class Tokenizer {

        private const string VALID_TOKENS = "+-*/^(),'";

        enum State {
            Default,
            InWord,
            InInt,
            InDecimal,
        }

        internal static List<object> Tokenize(string input) {
            List<object> tokens = new List<object>();
            State state = State.Default;
            StringBuilder sb = new StringBuilder();
            Stack<char> chars = new Stack<char>(input.Reverse());

            while (chars.Count > 0) {
                char c = chars.Pop();

                switch (state) {
                    case State.Default:
                        if (char.IsLetter(c)) {
                            sb.Append(c);
                            state = State.InWord;
                        } else if (char.IsDigit(c)) {
                            sb.Append(c);
                            state = State.InInt;
                        } else if (char.IsWhiteSpace(c))
                            continue;
                        else if (VALID_TOKENS.Contains(c))
                            tokens.Add(c);
                        else
                            throw new Exception(string.Format("Invalid Token: {0}", c));
                        break;
                    case State.InWord:
                        if (char.IsLetter(c))
                            sb.Append(c);
                        else
                            RestoreStateToDefault(c, tokens, sb, chars, ref state);
                        break;
                    case State.InInt:
                        if (char.IsDigit(c))
                            sb.Append(c);
                        else if (c == '.') {
                            sb.Append('.');
                            state = State.InDecimal;
                        } else
                            RestoreStateToDefault(c, tokens, sb, chars, ref state);
                        break;
                    case State.InDecimal:
                        if (char.IsDigit(c))
                            sb.Append(c);
                        else
                            RestoreStateToDefault(c, tokens, sb, chars, ref state);
                        break;
                }
            }

            if (state != State.Default)
                RestoreStateToDefault(' ', tokens, sb, chars, ref state);

            return tokens;
        }

        private static void RestoreStateToDefault(char c, List<object> tokens, StringBuilder sb, Stack<char> chars, ref State state) {
            // If there is a token accumulated in the String Builder, interpret and store it
            if (sb.Length > 0) {
                object token = sb.ToString();
                if (double.TryParse(sb.ToString(), out double result))
                    token = result;
                tokens.Add(token);
            }

            sb.Clear();
            chars.Push(c);
            state = State.Default;
        }
    }
}
