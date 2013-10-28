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

        public double[] simplex(double[][] A, double[] b, double[] c) {
            SimplexProblem p = new SimplexProblem(true, A, b, c);

            CompactTableau ct = Simplex.CreateTableau(p, notify);
            Simplex.SolveFeasible(ct, notify);
            return ct.MakeSolution();
        }

        public double[] simplex2(double[][] A, double[] b, double[] c) {
            SimplexProblem p = new SimplexProblem(true, A, b, c);

            BigTableau bt = Simplex.CreateBigTableau(p, notify);
            Simplex.SolveBig(bt, notify);
            return bt.MakeSolution();
        }

        public double[][] simplexMat(double[][] A, double[] b, double[] c) {
            SimplexProblem p = new SimplexProblem(true, A, b, c);

            BigTableau bt = Simplex.CreateBigTableau(p, notify);
            Simplex.SolveBig(bt, notify);
            return bt.t;
        }

        public double[] simplexRestart(double[][] mat, double[][] A,
                double[] b, double[] c) {
            BigTableau bt = Simplex.CreateBigRestartedTableau(mat, A, b, c);
            
            Simplex.FixBigRestartedTableau(bt, notify);
            return bt.MakeSolution();
        }

        public double[] simplexDual(double[][] A, double[] b, double[] c) {
            SimplexProblem p = new SimplexProblem(false, A, b, c);

            CompactTableau ct = Simplex.CreateDualTableau(p, notify);
            Simplex.SolveDual(ct, notify);
            return ct.MakeDualSolution();
        }

        public double[] simplexMaxFlow(double[][] g, int s, int t) {
            SimplexProblem p = Simplex.CreateFromMaxFlow(g, s, t);

            CompactTableau ct = Simplex.CreateTableau(p, notify);
            Simplex.SolveFeasible(ct, notify);
            return ct.MakeSolution();
        }

        // TODO: Remove this. Create a max flow function which returns just
        // a SimplexProblem.
        public double[] simplex2MaxFlow(double[][] g, int s, int t) {
            SimplexProblem p = Simplex.CreateFromMaxFlow(g, s, t);

            BigTableau bt = Simplex.CreateBigTableau(p, notify);
            Simplex.SolveBig(bt, notify);
            return bt.MakeSolution();
        }
    }
}
