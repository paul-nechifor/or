using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T1 {
    class Simplex {
        private int m;
        private int n;
        private double[][] t;
        private int[] indices;
        private double[] solution;

        public delegate void TableauHandler(double[][] t, int[] indices);
        public event TableauHandler Tableau;

        public Simplex(double[][] tableau, int m, int n) {
            this.t = tableau;
            this.m = m;
            this.n = n;
            this.indices = new int[n + m];
            this.solution = null;

            for (int i = 0; i < this.indices.Length; i++) {
                this.indices[i] = i;
            }
        }

        /// <summary>
        /// Initializes the tableau from the 'max c<sup>T</sup>x' and
        /// 'Ax&lt;=b' form.
        /// </summary>
        public static Simplex FromAbc(double[][] A, double[] b, double[] c) {
            int m = A.Length;
            int n = A[0].Length;

            double[][] t = new double[m + 1][];

            for (int i = 0; i < m; i++) {
                t[i] = new double[n + 1];

                Array.Copy(A[i], t[i], n);
                t[i][n] = b[i];
            }
            t[m] = new double[n + 1];
            Array.Copy(c, t[m], n);

            return new Simplex(t, m, n);
        }

        public double[] Solution {
            get {
                return solution;
            }
        }

        public void Solve() {
            if (Tableau != null) {
                Tableau(t, indices);
            }

            while (true) {
                int l = ChooseColumnBland();
                if (l == -1) {
                    break; // The optimal solution was found.
                }

                int k = ChooseRow(l);
                if (k == -1) {
                    throw new Exception("This is bad.");
                }

                Pivot(k, l);
                ChangeIndices(k, l);

                //PrintTableau();
                if (Tableau != null) {
                    Tableau(t, indices);
                }
            }

            FormSolution();
        }

        private int ChooseColumnBland() {
            for (int j = 0; j < n; j++) {
                if (t[m][j] > 0) {
                    return j;
                }
            }

            return -1;
        }

        // Use this if you like infinite loops.
        private int ChooseColumnMax() {
            int l = 0;
            double max = Double.MinValue;
            double tmj;

            for (int j = 0; j < n; j++) {
                tmj = t[m][j];
                if (tmj > 0 && tmj > max) {
                    max = tmj;
                    l = j;
                }
            }

            return l;
        }

        private int ChooseRow(int l) {
            int k = -1;
            double tkmin = Double.MaxValue;

            for (int i = 0; i < m; i++) {
                if (t[i][l] > 0) {
                    double tk = t[i][n] / t[i][l];
                    if (tk < tkmin) {
                        tkmin = tk;
                        k = i;
                    }
                }
            }

            return k;
        }

        private void Pivot(int k, int l) {
            for (int i = 0; i <= m; i++) {
                if (i == k) {
                    continue;
                }

                for (int j = 0; j <= n; j++) {
                    if (j == l) {
                        continue;
                    }

                    t[i][j] = (t[i][j] * t[k][l] - t[i][l] * t[k][j]) / t[k][l];
                }
            }

            for (int i = 0; i <= m; i++) {
                if (i != k) {
                    t[i][l] = -t[i][l] / t[k][l];
                }
            }

            for (int j = 0; j <= n; j++) {
                if (j != l) {
                    t[k][j] /= t[k][l];
                }
            }

            t[k][l] = 1 / t[k][l];
        }

        private void ChangeIndices(int k, int l) {
            int tmp = indices[n + k];
            indices[n + k] = indices[l];
            indices[l] = tmp;
        }

        private void PrintTableau() {
            for (int i = 0; i <= m; i++) {
                for (int j = 0; j <= n; j++) {
                    Console.Write(t[i][j] + "\t");
                }
                if (i < m) {
                    Console.WriteLine(indices[n + i] + " ");
                }
            }
            Console.WriteLine("\n");
        }

        private void FormSolution() {
            solution = new double[n];

            for (int i = 0; i < m; i++) {
                var index = indices[n + i];
                if (index < n) {
                    solution[index] = t[i][n];
                }
            }
        }
    }
}
