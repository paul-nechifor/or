using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace T1 {
    class FunctionSet {
        private OutputSheet os;
        private Action<string, object> notifyOn;
        private double epsilon = 1E-5; // Should it be bigger? ¯\(ツ)/¯

        public FunctionSet(OutputSheet outputSheet) {
            this.os = outputSheet;
            this.notifyOn = (label, value) => {
                os.AddValue(label, value);
            };
        }

        public bool equal(double[] a, double[] b) {
            if (a.Length != b.Length) {
                return false;
            }

            for (var i = 0; i < a.Length; i++) {
                if (Math.Abs(a[i] - b[i]) > epsilon) {
                    return false;
                }
            }

            return true;
        }

        public double[] simplex(double[][] A, double[] b, double[] c) {
            return simplex(A, b, c, 0);
        }

        public double[] simplex(double[][] A, double[] b, double[] c,
                int trace) {
            SimplexProblem p = new SimplexProblem(true, A, b, c);
            var notify = trace != 0 ? notifyOn : null;

            CompactTableau ct = Simplex.CreateTableau(p, notify);
            Simplex.SolveFeasible(ct, notify);
            return ct.MakeSolution();
        }

        public double[] simplexDual(double[][] A, double[] b, double[] c) {
            return simplexDual(A, b, c, 0);
        }

        public double[] simplexDual(double[][] A, double[] b, double[] c,
                int trace) {
            SimplexProblem p = new SimplexProblem(false, A, b, c);
            var notify = trace != 0 ? notifyOn : null;

            CompactTableau ct = Simplex.CreateDualTableau(p, notify);
            Simplex.SolveDual(ct, notify);
            return ct.MakeDualSolution();
        }

        public double[] simplexMaxFlow(double[][] g, int s, int t) {
            return simplexMaxFlow(g, s, t, 0);
        }

        public double[] simplexMaxFlow(double[][] g, int s, int t, int trace) {
            SimplexProblem p = Simplex.CreateFromMaxFlow(g, s, t);
            var notify = trace != 0 ? notifyOn : null;

            CompactTableau ct = Simplex.CreateTableau(p, notify);
            Simplex.SolveFeasible(ct, notify);
            return ct.MakeSolution();
        }
    }
}
