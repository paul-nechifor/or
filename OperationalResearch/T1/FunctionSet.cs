using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace T1 {
    class FunctionSet {
        private OutputSheet os;
        private Action<string, object> notify;
        private double epsilon = 1E-5; // Should it be bigger? ¯\(ツ)/¯

        public FunctionSet(OutputSheet outputSheet) {
            this.os = outputSheet;
            trace(1);
        }

        public void trace(int on) {
            if (on != 0) {
                this.notify = (label, value) => {
                    os.AddValue(label, value);
                };
            } else {
                notify = null;
            }
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

        public SimplexProblem simplexProblem(double[][] A, double[] b,
                double[] c) {
            return new SimplexProblem(true, A, b, c);
        }

        public SimplexProblem simplexDualProblem(double[][] A, double[] b,
                double[] c) {
            return new SimplexProblem(false, A, b, c);
        }

        public SimplexProblem maxFlowProblem(double[][] g, int s, int t) {
            return Simplex.CreateFromMaxFlow(g, s, t);
        }

        public CompactTableau simplexTableau(SimplexProblem p) {
            return Simplex.CreateTableau(p, notify);
        }

        public CompactTableau simplexDualTableau(SimplexProblem p) {
            return Simplex.CreateDualTableau(p, notify);
        }

        public BigTableau simplex2Tableau(SimplexProblem p) {
            return Simplex.CreateBigTableau(p, notify);
        }

        public double[] simplex(double[][] A, double[] b, double[] c) {
            return simplex(simplexProblem(A, b, c));
        }

        public double[] simplex(SimplexProblem p) {
            return simplex(simplexTableau(p));
        }

        public double[] simplex(CompactTableau ct) {
            Simplex.SolveFeasible(ct, notify);
            return ct.MakeSolution();
        }

        public double[] simplexDual(double[][] A, double[] b, double[] c) {
            return simplexDual(simplexDualProblem(A, b, c));
        }

        public double[] simplexDual(SimplexProblem p) {
            return simplexDual(simplexDualTableau(p));
        }

        public double[] simplexDual(CompactTableau ct) {
            Simplex.SolveDual(ct, notify);
            return ct.MakeDualSolution();
        }

        public double[] simplex2(double[][] A, double[] b, double[] c) {
            return simplex2(simplexProblem(A, b, c));
        }

        public double[] simplex2(SimplexProblem p) {
            return simplex2(simplex2Tableau(p));
        }

        public double[] simplex2(BigTableau bt) {
            Simplex.SolveBig(bt, notify);
            return bt.MakeSolution();
        }

        public BigTableau simplex2TableauSolved(SimplexProblem p) {
            BigTableau bt = simplex2Tableau(p);
            Simplex.SolveBig(bt, notify);
            return bt;
        }

        public double[] simplex2Restart(BigTableau old, SimplexProblem p) {
            BigTableau bt2 = Simplex.CreateBigRestartedTableau(old, p);

            if (notify != null) {
                notify("Before pivoting:", bt2);
            }

            Simplex.FixBigRestartedTableau(bt2, notify);
            return bt2.MakeSolution();
        }

        public double[][] f21b(BigTableau bt, double[] b, double[] bBar) {
            return Simplex.F21b(bt, b, bBar);
        }

        public double[][] f21c(BigTableau bt, double[] c, double[] cBar) {
            return Simplex.F21c(bt, c, cBar);
        }
    }
}
