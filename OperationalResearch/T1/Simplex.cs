using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T1 {
    public class InfeasibleProblemException : Exception {
        public InfeasibleProblemException()
            : base("Infeasible problem.") {
        }
    }

    public class UnboundedProblemException : Exception {
        public UnboundedProblemException()
            : base("Unbounded problem.") {
        }
    }

    public class SimplexProblem {
        public bool max;
        public double[][] A;
        public double[] b;
        public double[] c;

        public SimplexProblem(bool max, double[][] A, double[] b, double[] c) {
            this.max = max;
            this.A = A;
            this.b = b;
            this.c = c;
        }

        public bool IsInitiallyFeasible() {
            for (int i = 0; i < b.Length; i++) {
                if (b[i] < 0) {
                    return false;
                }
            }

            return true;
        }
    }

    public class CompactTableau {
        public int m;
        public int n;
        public double[][] t;
        public int[] ind;
        public bool hasU = false;
        public bool dual = false;

        public CompactTableau(int m, int n, double[][] t, int[] ind,
                bool hasU) {
            this.m = m;
            this.n = n;
            this.t = t;
            this.ind = ind;
            this.hasU = hasU;
        }

        public double[] MakeSolution() {
            double[] ret = new double[n];

            for (int i = 0; i < m; i++) {
                var index = ind[n + i];
                if (index < n) {
                    ret[index] = t[i][n];
                }
            }

            return ret;
        }

        public double[] MakeDualSolution() {
            double[] ret = new double[m];

            for (int j = 0; j < n; j++) {
                var index = ind[j];
                if (index < m) {
                    ret[index] = -t[m][j];
                }
            }

            return ret;
        }
    }

    public class BigTableau {
        public int m;
        public int n;
        public double[][] t;
        public int[] ind;

        public BigTableau(int m, int n, double[][] t, int[] ind) {
            this.m = m;
            this.n = n;
            this.t = t;
            this.ind = ind;
        }

        public double[] MakeSolution() {
            double[] ret = new double[n - m];

            for (int i = 0; i < m; i++) {
                if (ind[i] < ret.Length) {
                    ret[ind[i]] = t[i][n];
                }
            }

            return ret;
        }
    }

    class Simplex {
        public static CompactTableau CreateTableau(SimplexProblem p,
                Action<string, object> notify) {
            CompactTableau ct;

            if (notify != null) {
                notify("Problem:", p);
            }

            if (p.IsInitiallyFeasible()) {
                ct = CreateFeasibleTableau(p);
                if (notify != null) {
                    notify("Created feasible tableau:", ct);
                }

                return ct;
            }

            ct = CreateInfeasibleTableau(p);
            if (notify != null) {
                notify("Created infeasible tableau:", ct);
            }

            PivotInfeasibleTableau(ct);
            if (notify != null) {
                notify("Pivoted infeasible tableau:", ct);
            }

            SolveFeasible(ct, notify);

            TransformToFeasible(ct, p.c);
            if (notify != null) {
                notify("Removed column:", ct);
            }

            return ct;
        }

        public static CompactTableau CreateFeasibleTableau(SimplexProblem p) {
            int m = p.A.Length;
            int n = p.A[0].Length;

            double[][] t = new double[m + 1][];

            for (int i = 0; i < m; i++) {
                t[i] = new double[n + 1];

                Array.Copy(p.A[i], t[i], n);
                t[i][n] = p.b[i];
            }
            t[m] = new double[n + 1];
            Array.Copy(p.c, t[m], n);

            int[] ind = new int[n + m];

            for (int i = 0; i < ind.Length; i++) {
                ind[i] = i;
            }

            return new CompactTableau(m, n, t, ind, false);
        }

        public static CompactTableau CreateInfeasibleTableau(SimplexProblem p) {
            int m = p.A.Length;
            int nOrig = p.A[0].Length;
            int n = nOrig + 1;

            double[][] t = new double[m + 1][];

            for (int i = 0; i < m; i++) {
                t[i] = new double[n + 1];

                Array.Copy(p.A[i], t[i], nOrig);
                t[i][nOrig] = -1;
                t[i][n] = p.b[i];
            }
            t[m] = new double[n + 1];
            t[m][nOrig] = -1;

            int[] ind = new int[n + m];

            for (int i = 0; i < nOrig; i++) {
                ind[i] = i;
            }

            ind[nOrig] = ind.Length - 1;

            for (int i = 0; i < m; i++) {
                ind[n + i] = nOrig + i;
            }

            return new CompactTableau(m, n, t, ind, true);
        }

        public static void PivotInfeasibleTableau(CompactTableau ct) {
            int iMin = 0;
            double iValMin = ct.t[iMin][ct.n];

            for (int i = 1; i < ct.m; i++) {
                if (ct.t[i][ct.n] < iValMin) {
                    iValMin = ct.t[i][ct.n];
                    iMin = i;
                }
            }

            Pivot(ct, iMin, ct.n - 1);
        }

        public static void SolveFeasible(CompactTableau ct) {
            SolveFeasible(ct, null);
        }

        public static void SolveFeasible(CompactTableau ct,
                Action<string, object> notify) {
            int iter = 1;

            while (true) {
                int l = ChooseColumnBland(ct);
                if (l == -1) {
                    break; // The optimal solution was found.
                }

                int k = ChooseRow(ct, l);

                if (k == -1) {
                    throw new UnboundedProblemException();
                }

                Pivot(ct, k, l);

                if (notify != null) {
                    notify("After iteration " + iter, ct);
                }
                iter++;
            }
        }

        private static int ChooseColumnBland(CompactTableau ct) {
            for (int j = 0; j < ct.n; j++) {
                if (ct.t[ct.m][j] > 0) {
                    return j;
                }
            }

            return -1;
        }

        // Use this if you like infinite loops.
        private static int ChooseColumnMax(CompactTableau ct) {
            int l = 0;
            double max = Double.MinValue;
            double tmj;

            for (int j = 0; j < ct.n; j++) {
                tmj = ct.t[ct.m][j];
                if (tmj > 0 && tmj > max) {
                    max = tmj;
                    l = j;
                }
            }

            return l;
        }

        private static int ChooseRow(CompactTableau ct, int l) {
            int k = -1;
            double tkmin = double.MaxValue;
            double[][] t = ct.t;

            for (int i = 0; i < ct.m; i++) {
                if (t[i][l] > 0) {
                    double tk = t[i][ct.n] / t[i][l];
                    if (tk < tkmin) {
                        tkmin = tk;
                        k = i;
                    }
                }
            }

            return k;
        }

        private static void Pivot(CompactTableau ct, int k, int l) {
            double[][] t = ct.t;

            for (int i = 0; i <= ct.m; i++) {
                if (i == k) {
                    continue;
                }

                for (int j = 0; j <= ct.n; j++) {
                    if (j == l) {
                        continue;
                    }

                    t[i][j] = (t[i][j] * t[k][l] - t[i][l] * t[k][j]) / t[k][l];
                }
            }

            for (int i = 0; i <= ct.m; i++) {
                if (i != k) {
                    t[i][l] = -t[i][l] / t[k][l];
                }
            }

            for (int j = 0; j <= ct.n; j++) {
                if (j != l) {
                    t[k][j] /= t[k][l];
                }
            }

            t[k][l] = 1 / t[k][l];

            // Change indices.
            int tmp = ct.ind[ct.n + k];
            ct.ind[ct.n + k] = ct.ind[l];
            ct.ind[l] = tmp;
        }

        public static void TransformToFeasible(CompactTableau ct, double[] c) {
            ct.n--;

            double[][] t = ct.t;
            int[] ind = ct.ind;
            int m = ct.m;
            int n = ct.n;

            int uIndex = ind.Length - 1;

            // Checking the position of the u in the columns.
            int uPos = -1;
            for (int i = 0; i <= n; i++) {
                if (ind[i] == uIndex) {
                    uPos = i;
                    break;
                }
            }

            if (uPos == -1) {
                throw new InfeasibleProblemException();
            }

            // Removing the column.
            for (int i = 0; i < m; i++) {
                for (int j = uPos; j <= n; j++) {
                    t[i][j] = t[i][j + 1];
                }
            }

            // Removing the extra index.
            for (int j = uPos; j < uIndex; j++) {
                ind[j] = ind[j + 1];
            }

            // Solving the new objective function.
            for (int j = 0; j < n; j++) {
                t[m][j] = (ind[j] < n) ? c[ind[j]] : 0;
            }
            t[m][n] = 0;

            for (int i = 0; i < m; i++) {
                if (ind[n + i] < n) {
                    for (int j = 0; j <= n; j++) {
                        t[m][j] -= c[ind[n + i]] * t[i][j];
                    }
                }
            }

            ct.hasU = false;
        }

        public static CompactTableau CreateDualTableau(SimplexProblem p,
                Action<string, object> notify) {
            if (notify != null) {
                notify("Problem:", p);
            }

            int m = p.A[0].Length;
            int n = p.A.Length;

            double[][] t = new double[m + 1][];

            for (int i = 0; i < m; i++) {
                t[i] = new double[n + 1];
                for (int j = 0; j < n; j++) {
                    t[i][j] = p.A[j][i];
                }
                t[i][n] = p.c[i];
            }
            t[m] = new double[n + 1];
            Array.Copy(p.b, t[m], n);

            int[] ind = new int[n + m];
            for (int j = 0; j < n; j++) {
                ind[j] = m + j;
            }
            for (int i = 0; i < m; i++) {
                ind[n + i] = i;
            }

            CompactTableau ct = new CompactTableau(m, n, t, ind, false);
            ct.dual = true;
            if (notify != null) {
                notify("Created dual tableau:", ct);
            }

            return ct;
        }



        public static void SolveDual(CompactTableau ct) {
            SolveDual(ct, null);
        }

        public static void SolveDual(CompactTableau ct,
                Action<string, object> notify) {
            int iter = 1;

            while (true) {
                int k = ChooseRowBlandDual(ct);
                if (k == -1) {
                    break; // The optimal solution was found.
                }

                int l = ChooseColumnDual(ct, k);

                if (l == -1) {
                    throw new UnboundedProblemException();
                }

                Pivot(ct, k, l);

                if (notify != null) {
                    notify("After iteration " + iter + ":", ct);
                }
                iter++;
            }
        }

        private static int ChooseRowBlandDual(CompactTableau ct) {
            for (int i = 0; i < ct.m; i++) {
                if (ct.t[i][ct.n] < 0) {
                    return i;
                }
            }

            return -1;
        }

        private static int ChooseColumnDual(CompactTableau ct, int k) {
            int l = -1;
            double tlmin = double.MaxValue;
            double[][] t = ct.t;

            for (int j = 0; j < ct.n; j++) {
                if (t[k][j] < 0) {
                    double tl = t[ct.m][j] / t[k][j];
                    if (tl < tlmin) {
                        tlmin = tl;
                        l = j;
                    }
                }
            }

            return l;
        }

        public static SimplexProblem CreateFromMaxFlow(double[][] g, int s,
                int t) {
            int nVars = g.Length;

            // Computing the number of nodes there are.
            int nNodes = 0;
            for (int i = 0; i < nVars; i++) {
                if (g[i][0] > nNodes) {
                    nNodes = (int) g[i][0];
                }
                if (g[i][1] > nNodes) {
                    nNodes = (int) g[i][1];
                }
            }
            nNodes++; // Because g contains indices from 0.

            List<int>[] nodes = new List<int>[nNodes];
            for (int i = 0; i < nNodes; i++) {
                nodes[i] = new List<int>();
            }

            for (int i = 0; i < nVars; i++) {
                int pos = i + 1;
                nodes[(int)g[i][0]].Add(-pos);
                nodes[(int)g[i][1]].Add(pos);
            }

            double[][] A = new double[nVars + 2 * nNodes - 4][];
            double[] b = new double[A.Length];

            for (int i = 0; i < A.Length; i++) {
                A[i] = new double[nVars];
            }

            // The capacity restrictions.
            for (int i = 0; i < nVars; i++) {
                A[i][i] = 1;
                b[i] = g[i][2];
            }

            // The node restrictions.
            for (int i = 0, k = nVars; i < nNodes; i++) {
                if (i == s || i == t) {
                    continue;
                }
                foreach (int pos in nodes[i]) {
                    int positive = (pos < 0) ? -1 : 1;
                    int varIndex = (pos < 0) ? (-pos - 1) : (pos - 1);
                    A[k][varIndex] = positive;
                    A[k + 1][varIndex] = -positive;
                }
                k += 2;
            }

            // Setting the optimization coeficients. The sum of all the
            // variables who enter the exit node.
            double[] c = new double[nVars];
            foreach (int entry in nodes[t]) {
                if (entry > 0) {
                    c[entry - 1] = 1;
                }
            }

            return new SimplexProblem(true, A, b, c);
        }

        public static string SixPapFormat(SimplexProblem p) {
            int m = p.A.Length;
            int n = p.A[0].Length;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("objective: MAX\nm: {0}\ns: {1}\n", m, n);
            sb.Append("C(j):");
            for (var j = 0; j < n; j++) {
                sb.Append(" " + p.c[j]);
            }
            sb.Append("\nconstraints:\n");

            for (var i = 0; i < m; i++) {
                sb.AppendFormat("({0}):", i + 1);
                for (var j = 0; j < n; j++) {
                    sb.Append(" " + p.A[i][j]);
                }
                sb.AppendFormat(" < {0}\n", p.b[i]);
            }

            return sb.ToString();
        }

        public static BigTableau CreateBigTableau(SimplexProblem p,
                Action<string, object> notify) {
            if (notify != null) {
                notify("Problem:", p);
            }

            int m = p.A.Length;
            int nVars = p.A[0].Length;
            int n = m + nVars;

            double[][] t = new double[m + 1][];

            for (int i = 0; i < m; i++) {
                t[i] = new double[n + 1];

                Array.Copy(p.A[i], t[i], nVars);
                t[i][nVars + i] = 1;
                t[i][n] = p.b[i];
            }
            t[m] = new double[n + 1];
            for (int i = 0; i < nVars; i++) {
                t[m][i] = -p.c[i];
            }

            int[] ind = new int[m];

            for (int i = 0; i < ind.Length; i++) {
                ind[i] = nVars + i;
            }

            BigTableau bt = new BigTableau(m, n, t, ind);

            if (notify != null) {
                notify("Created tableau:", bt);
            }

            return bt;
        }

        public static void SolveBig(BigTableau ct) {
            SolveBig(ct, null);
        }

        public static void SolveBig(BigTableau ct,
                Action<string, object> notify) {
            int iter = 1;

            while (true) {
                int l = ChooseColumnBland(ct);
                if (l == -1) {
                    break; // The optimal solution was found.
                }

                int k = ChooseRow(ct, l);

                if (k == -1) {
                    throw new UnboundedProblemException();
                }

                Pivot(ct, k, l);

                if (notify != null) {
                    notify("After iteration " + iter, ct);
                }
                iter++;
            }
        }

        private static int ChooseColumnBland(BigTableau bt) {
            for (int j = 0; j < bt.n; j++) {
                if (bt.t[bt.m][j] < 0) {
                    return j;
                }
            }

            return -1;
        }

        private static int ChooseRow(BigTableau bt, int l) {
            int k = -1;
            double tkmin = double.MaxValue;
            double[][] t = bt.t;

            for (int i = 0; i < bt.m; i++) {
                if (t[i][l] > 0) {
                    double tk = t[i][bt.n] / t[i][l];
                    if (tk < tkmin) {
                        tkmin = tk;
                        k = i;
                    }
                }
            }

            return k;
        }

        private static void Pivot(BigTableau bt, int k, int l) {
            double[][] t = bt.t;

            for (int i = 0; i <= bt.m; i++) {
                if (i == k) {
                    continue;
                }

                for (int j = 0; j <= bt.n; j++) {
                    if (j == l) {
                        continue;
                    }

                    t[i][j] = (t[i][j] * t[k][l] - t[i][l] * t[k][j]) / t[k][l];
                }
            }

            for (int i = 0; i <= bt.m; i++) {
                if (i != k) {
                    t[i][l] = 0;
                }
            }

            for (int j = 0; j <= bt.n; j++) {
                if (j != l) {
                    t[k][j] /= t[k][l];
                }
            }

            t[k][l] = 1;

            // Change indices.
            bt.ind[k] = l;
        }
    }
}
