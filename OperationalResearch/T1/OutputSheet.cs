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
            lineDiv.SetAttribute("class", "line");
            lineDiv.AppendChild(elem);
            doc.Body.AppendChild(lineDiv);
        }

        private void AddHtmlLine(string html) {
            HtmlElement lineDiv = doc.CreateElement("div");
            lineDiv.SetAttribute("class", "line");
            lineDiv.InnerHtml = html;
            doc.Body.AppendChild(lineDiv);
        }

        public void  AddValue(object value) {
            AddHtmlLine(Print(value));
        }

        public void AddAsignment(string name, object value) {
            AddValue(name + " =", value);
        }

        public void AddValue(string label, object value) {
            string format = "<table><tr><td>{0}</td><td>{1}</td></tr></table>";
            AddHtmlLine(string.Format(format, label, Print(value)));
        }

        public void AddError(string error) {
            AddHtmlLine(error);
        }

        private string Print(object value) {
            if (value is double[]) {
                return Print((double[])value);
            } else if (value is double[][]) {
                return Print((double[][])value);
            } else if (value is CompactTableau) {
                return Print((CompactTableau)value);
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
                    sb.AppendFormat("<td>{0}</td>", m[i][j]);
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
    }
}
