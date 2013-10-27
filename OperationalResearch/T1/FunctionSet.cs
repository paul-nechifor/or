using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace T1 {
    class FunctionSet {
        private OutputSheet os;

        public FunctionSet(OutputSheet outputSheet) {
            this.os = outputSheet;
        }

        public double[] simplex(double[][] A, double[] b, double[] c) {
            return simplex(A, b, c, 0);
        }

        public double[] simplex(double[][] A, double[] b, double[] c,
                int trace) {
            SimplexProblem p = new SimplexProblem(true, A, b, c);
            Action<string, object> notify;

            if (trace == 0) {
                notify = null;
            } else {
                notify = (label, value) => {
                    os.AddValue(label, value);
                };
            }

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
            Action<string, object> notify;

            if (trace == 0) {
                notify = null;
            } else {
                notify = (label, value) => {
                    os.AddValue(label, value);
                };
            }

            CompactTableau ct = Simplex.CreateDualTableau(p, notify);
            Simplex.SolveDual(ct, notify);
            return ct.MakeDualSolution();
        }
    }
}
