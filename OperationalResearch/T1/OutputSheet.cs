using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace T1 {
    class OutputSheet {
        private HtmlDocument doc;

        public OutputSheet(HtmlDocument doc) {
            this.doc = doc;
        }

        private void AddLine(HtmlElement elem) {
            HtmlElement lineDiv = doc.CreateElement("div");
            lineDiv.AppendChild(elem);
            doc.Body.AppendChild(lineDiv);
        }

        private void AddHtmlLine(string html, bool error) {
            HtmlElement lineDiv = doc.CreateElement("div");
            if (error) {
                lineDiv.Style = "color: #F00";
            }
            lineDiv.InnerHtml = html;
            doc.Body.AppendChild(lineDiv);
        }

        public void AddError(string error) {
            AddHtmlLine(error, true);
        }

        public void  AddValue(object value) {
            AddHtmlLine(Print(value), false);
        }

        public void AddValue(string label, object value) {
            string format = "<table><tr><td>{0}</td><td>{1}</td></tr></table>";
            AddHtmlLine(string.Format(format, label, Print(value)), false);
        }

        public void AddAsignment(string name, object value) {
            AddValue(name + " =", value);
        }

        private string Print(object value) {
            if (value is double[]) {
                return Print((double[])value);
            } else if (value is double[][]) {
                return Print((double[][])value);
            } else if (value is CompactTableau) {
                return Print((CompactTableau)value);
            } else if (value is SimplexProblem) {
                return Print((SimplexProblem)value);
            } else if (value is BigTableau) {
                return Print((BigTableau)value);
            } else if (value == null) {
                return "null";
            } else {
                return value.ToString();
            }
        }

        private string Print(double[] v) {
            return "[" + string.Join("&nbsp; ", v) + "]";
        }

        private string Print(double[][] m) {
            StringBuilder sb = new StringBuilder();
            sb.Append("<table class='matrix'>");
            for (var i = 0; i < m.Length; i++) {
                sb.Append("<tr>");
                for (var j = 0; j < m[0].Length; j++) {
                    sb.AppendFormat("<td>{0:0.###}</td>", m[i][j]);
                }
                sb.Append("</tr>");
            }
            sb.Append("</table>");
            return sb.ToString();
        }

        private string Print(CompactTableau ct) {
            return ct.dual ? PrintDualTableau(ct) : PrintSimplexTableau(ct);
        }

        private string PrintDualTableau(CompactTableau ct) {
            StringBuilder sb = new StringBuilder();

            sb.Append("<table class='simplex-dual-tableau'>");

            for (int i = 0; i < ct.m; i++) {
                sb.Append("<tr>");

                sb.AppendFormat("<td class='first'>y<sub>{0}</sub></td>",
                        ct.ind[ct.n + i] + 1);

                for (int j = 0; j < ct.n; j++) {
                    sb.AppendFormat("<td>{0:0.###}</td>", ct.t[i][j]);
                }

                sb.AppendFormat("<td class='last'>{0:0.###}</td>",
                        ct.t[i][ct.n]);

                sb.Append("</tr>");
            }

            sb.Append("<tr>");
            sb.Append("<td class='vals first'>-1</td>");
            for (int j = 0; j < ct.n; j++) {
                sb.AppendFormat("<td class='vals'>{0:0.###}</td>",
                    ct.t[ct.m][j]);
            }
            sb.AppendFormat("<td class='vals last'>{0:0.###}</td>",
                    ct.t[ct.m][ct.n]);
            sb.Append("</tr>");

            sb.Append("<tr>");
            sb.Append("<td class='first'>&nbsp;</td>");
            for (int j = 0; j < ct.n; j++) {
                sb.AppendFormat("<td>= y<sub>{0}</sub></td>", ct.ind[j] + 1);
            }
            sb.AppendFormat("<td class='last'>= w</td>");
            sb.Append("</tr>");

            sb.Append("</table>");

            return sb.ToString();
        }

        private string PrintSimplexTableau(CompactTableau ct) {
            StringBuilder sb = new StringBuilder();

            sb.Append("<table class='simplex-tableau'>");

            PrintSimplexHeader(sb, ct);

            for (int i = 0; i < ct.m; i++) {
                sb.Append("<tr>");

                for (int j = 0; j < ct.n; j++) {
                    sb.AppendFormat("<td>{0:0.###}</td>", ct.t[i][j]);
                }

                sb.AppendFormat("<td class='rhs'>{0:0.###}</td>",
                        ct.t[i][ct.n]);
                sb.AppendFormat("<td class='last'>= -{0}</td>",
                        XNumber(ct, ct.n + i));

                sb.Append("</tr>");
            }

            PrintSimplexFooter(sb, ct);

            sb.Append("</table>");

            return sb.ToString();
        }

        private void PrintSimplexHeader(StringBuilder sb, CompactTableau ct) {
            sb.Append("<tr class='header'>");

            for (int j = 0; j < ct.n; j++) {
                sb.AppendFormat("<td>" + XNumber(ct, j) + "</td>");
            }

            sb.Append("<td class='rhs'>-1</td>");
            sb.Append("<td>&nbsp;</td>");

            sb.Append("</tr>");
        }

        private void PrintSimplexFooter(StringBuilder sb, CompactTableau ct) {
            sb.Append("<tr class='footer'>");

            for (int j = 0; j < ct.n; j++) {
                sb.AppendFormat("<td>{0:0.###}</td>", ct.t[ct.m][j]);
            }

            sb.AppendFormat("<td class='rhs'>{0:0.###}</td>",
                    ct.t[ct.m][ct.n]);
            sb.Append("<td class='last'>= z</td>");

            sb.Append("</tr>");
        }

        private string XNumber(CompactTableau ct, int j) {
            if (ct.hasU && ct.ind[j] == ct.ind.Length - 1) {
                return "u";
            }

            return "x<sub>" + (ct.ind[j] + 1) + "</sub>";
        }

        private string Print(BigTableau bt) {
            StringBuilder sb = new StringBuilder();

            sb.Append("<table class='big-tableau'>");

            sb.Append("<tr><td class='header first'>&nbsp;</td>");
            for (int j = 0; j < bt.n; j++) {
                sb.AppendFormat("<td class='header'>x<sub>{0}</sub></td>",
                        j + 1);
            }
            sb.Append("<td class='header last'>RHS</td></tr>");

            for (int i = 0; i < bt.m; i++) {
                sb.AppendFormat("<tr><td class='first'>x<sub>{0}</sub></td>",
                        bt.ind[i] + 1);
                for (int j = 0; j < bt.n; j++) {
                    sb.AppendFormat("<td>{0:0.###}</td>", bt.t[i][j]);
                }
                sb.AppendFormat("<td class='last'>{0:0.###}</td></tr>",
                        bt.t[i][bt.n]);
            }

            sb.Append("<tr><td class='footer first'>z</td>");
            for (int j = 0; j < bt.n; j++) {
                sb.AppendFormat("<td class='footer'>{0:0.###}</td>",
                        bt.t[bt.m][j]);
            }
            sb.AppendFormat("<td class='footer last'>{0:0.###}</td></tr>",
                    bt.t[bt.m][bt.n]);

            return sb.ToString();
        }

        private string Print(SimplexProblem p) {
            int m = p.A.Length;
            int n = p.A[0].Length;

            StringBuilder sb = new StringBuilder();

            sb.Append("<table class='simplex-problem'>");

            sb.AppendFormat("<tr><td class='first'>{0} ",
                    p.max ? "max" : "min");
            string comp = p.max ? "≤" : "≥";
            string name = p.max ? "x" : "y";
            bool first = true;
            for (int j = 0; j < n; j++) {
                AddX(sb, name, p.c[j], j, ref first);
            }
            sb.Append("</td></tr>");

            for (int i = 0; i < m; i++) {
                sb.Append("<tr><td class='first'>");
                first = true;
                for (int j = 0; j < n; j++) {
                    AddX(sb, name, p.A[i][j], j, ref first);
                }
                sb.AppendFormat("</td><td>{0}</td><td>{1:0.###}</td></tr>",
                        comp, p.b[i]);
            }

            sb.Append("<tr><td class='first'>");
            for (int j = 0; j < n; j++) {
                if (j > 0) {
                    sb.Append(",");
                }
                sb.AppendFormat("{0}<sub>{1}</sub>", name, j + 1);
            }
            sb.Append("</td><td>≥</td><td>0</td></tr>");

            sb.Append("</table>");

            return sb.ToString();
        }

        private static void AddX(StringBuilder sb, string name, double v,
                int index, ref bool first) {
            if (v == 0) {
                return;
            }
            string sgn = v < 0 ? "−" : (first ? "" : "+");
            first = false;
            if (v < 0) {
                v = -v;
            }
            string cof = v == 1 ? "" : String.Format("{0:0.###}", v);
            sb.AppendFormat("{0}{1}{2}<sub>{3}</sub>", sgn, cof, name,
                    index + 1);
        }
    }
}
