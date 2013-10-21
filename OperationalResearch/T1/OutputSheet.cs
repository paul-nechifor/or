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

        public void AddValue(object value) {
            AddHtmlLine(Print(value));
        }

        public void AddAsignment(string name, object value) {
            string format = "<table><tr><td>{0} = </td><td>{1}</td></tr></table>";
            AddHtmlLine(string.Format(format, name, Print(value)));
        }

        public void AddError(string error) {
            AddHtmlLine(error);
        }

        public void AddSimplexTableau(double[][] t, int[] indices) {
            int m = t.Length - 1;
            int n = t[0].Length - 1;

            StringBuilder sb = new StringBuilder();

            sb.Append("<table class='simplex-tableau'>");

            AddSimplexHeader(sb, n, indices);

            for (int i = 0; i < m; i++) {
                sb.Append("<tr>");

                for (int j = 0; j < n; j++) {
                    sb.AppendFormat("<td>{0}</td>", t[i][j]);
                }

                sb.AppendFormat("<td class='rhs'>{0}</td>", t[i][n]);
                sb.AppendFormat("<td class='last'>= -x<sub>{0}</sub></td>",
                        indices[n + i] + 1);

                sb.Append("</tr>");
            }

            AddSimplexFooter(sb, m, n, t);

            sb.Append("</table>");

            AddHtmlLine(sb.ToString());
        }

        public void AddSimplexHeader(StringBuilder sb, int n, int[] indices) {
            sb.Append("<tr class='header'>");

            for (int j = 0; j < n; j++) {
                sb.AppendFormat("<td>x<sub>{0}</sub></td>", indices[j] + 1);
            }

            sb.Append("<td class='rhs'>-1</td>");
            sb.Append("<td>&nbsp;</td>");

            sb.Append("</tr>");
        }

        private void AddSimplexFooter(StringBuilder sb, int m, int n,
                double[][] t) {
            sb.Append("<tr class='footer'>");


            for (int j = 0; j < n; j++) {
                sb.AppendFormat("<td>{0}</td>", t[m][j]);
            }

            sb.AppendFormat("<td class='rhs'>{0}</td>", t[m][n]);
            sb.Append("<td class='last'>= z</td>");

            sb.Append("</tr>");
        }

        private string Print(object value) {
            if (value is double[]) {
                return PrintVector((double[]) value);
            } else if (value is double[][]) {
                return PrintMatrix((double[][]) value);
            } else {
                return value.ToString();
            }
        }

        private string PrintVector(double[] v) {
            return "[" + string.Join("&nbsp; ", v) + "]";
        }

        private string PrintMatrix(double[][] m) {
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
    }
}
