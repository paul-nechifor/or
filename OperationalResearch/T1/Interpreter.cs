using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Globalization;

namespace T1 {
    /// <summary>
    /// The nice way to do this would be to define a language, but this works
    /// just dandy.
    /// </summary>
    class Interpreter {
        private static Regex NAME_REGEX = new Regex(@"[a-zA-Z_][a-zA-Z_0-9]*");
        private static Regex DOUBLE_REGEX = new Regex(@"[+-]?[0-9]+.[0-9]+");
        private static Regex INT_REGEX = new Regex(@"[+-]?[0-9]+");

        private OutputSheet outputSheet;
        private FunctionSet functionSet;
        private Type functionsType;
        private Dictionary<string, object> names
                = new Dictionary<string, object>();

        public Interpreter(OutputSheet outputSheet) {
            this.outputSheet = outputSheet;
            this.functionSet = new FunctionSet(outputSheet);
            this.functionsType = functionSet.GetType();
        }

        public void Interpret(string text) {
            text = Regex.Replace(text, @"\s+", "");

            foreach (string subcommand in text.Split(';')) {
                InterpretCommand(subcommand);
            }
        }

        private void InterpretCommand(string text) {
            if (text.Length == 0) {
                return;
            }

            int index = text.IndexOf('=');
            if (index != -1) {
                InterpretAsignment(text, index);
            } else {
                object result = Evaluate(text);
                if (result != null) {
                    names["_"] = result;
                    outputSheet.AddValue(result);
                }
            }
        }

        private void InterpretAsignment(string text, int index) {
            string lvalue = text.Substring(0, index);
            string rvalue = text.Substring(index + 1, text.Length - index - 1);
            object result = Evaluate(rvalue);

            names[lvalue] = result;
            outputSheet.AddAsignment(lvalue, result);
        }

        // Very hackish code! Look away lest you be retinally damaged.
        private object Evaluate(string text) {
            int brackets = 0;
            foreach (char c in text) {
                if (c == '[')
                    brackets++;
                else
                    break;
            }

            if (brackets == 2) { // If it's a matrix.
                string bare = text.Substring(2, text.Length - 4);
                string[] s2 = Regex.Split(bare, "\\],\\[");
                int n = s2.Length;
                int m = s2[0].Split(',').Length;

                double[][] ret = new double[n][];
                for (int i = 0; i < n; i++) {
                    ret[i] = new double[m];
                    for (int j = 0; j < m; j++) {
                        String[] linie = s2[i].Split(',');
                        ret[i][j] = ParseDouble(linie[j]);
                    }
                }
                return ret;
            } else if (brackets == 1) { // If it's a vector.
                string[] split = text.Substring(1, text.Length - 2).Split(',');
                double[] ret = new double[split.Length];
                for (int i = 0; i < split.Length; i++)
                    ret[i] = ParseDouble(split[i]);
                return ret;
            } else if (text[0] == '"') { // If it's a string.
                return text.Substring(1, text.Length - 2);
            } else if (text.Contains("(")) { // If it's a function.
                int index = text.IndexOf("(");
                String name = text.Substring(0, index);
                List<string> expressions = new List<string>();


                // Scot numele și parantezele pentru aceasta functie.
                text = text.Substring(index + 1, text.Length - index - 2);

                int open = 0;
                int openp = 0;
                StringBuilder expr = new StringBuilder();

                foreach (char c in text) {
                    bool add = true;
                    if (c == '(')
                        open++;
                    else if (c == ')')
                        open--;
                    else if (c == '[')
                        openp++;
                    else if (c == ']')
                        openp--;
                    else if (c == ',') {
                        if (open == 0 && openp == 0) {
                            expressions.Add(expr.ToString());
                            expr.Remove(0, expr.Length);
                            add = false;
                        }
                    }

                    if (add)
                        expr.Append(c);
                }
                if (expr.Length > 0)
                    expressions.Add(expr.ToString());

                object[] param = new object[expressions.Count];
                Type[] types = new Type[param.Length];
                for (int i = 0; i < param.Length; i++) {
                    param[i] = Evaluate(expressions[i]);
                    if (param[i] == null) {
                        outputSheet.AddError("Funcția " + name + " nu poate " +
                                "fi apelată deoarece expresia „" + expressions[i] +
                                "“ întoarce null.");
                        return null;
                    }
                    types[i] = param[i].GetType();
                }

                MethodInfo method = functionsType.GetMethod(name, types);
                object ret = method.Invoke(functionSet, param);
                return ret;
            } else if (NAME_REGEX.Matches(text).Count > 0) { // A variable.
                if (!names.ContainsKey(text)) {
                    outputSheet.AddError("No such variable '" + text + "'.");
                    return null;
                }
                return names[text];
            } else if (DOUBLE_REGEX.Matches(text).Count > 0) { // A double.
                return ParseDouble(text);
            } else if (INT_REGEX.Matches(text).Count > 0) { // An integer.
                return int.Parse(text);
            }

            return null;
        }

        private double ParseDouble(string s) {
            return double.Parse(s,
                        System.Globalization.NumberStyles.Any,
                        CultureInfo.GetCultureInfo("en-US"));
        }
    }
}
