using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace T1 {
    class FunctionSet {
        private OutputSheet outputSheet;

        public FunctionSet(OutputSheet outputSheet) {
            this.outputSheet = outputSheet;
        }

        public double[] simplex(double[][] A, double[] b, double[] c) {
            return simplex(A, b, c, 0);
        }

        public double[] simplex(double[][] A, double[] b, double[] c,
                int trace) {
            Simplex s = Simplex.FromAbc(A, b, c);
            if (trace != 0) {
                s.Tableau += (t, indices) => {
                    outputSheet.AddSimplexTableau(t, indices);
                };
            }
            s.Solve();
            return s.Solution;
        }
    }
}
