using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using T1.Properties;

namespace T1 {
    public partial class MainWindow : Form {
        private OutputSheet outputSheet;
        private Interpreter interpreter;

        public MainWindow() {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e) {
            webBrowser.DocumentText = Resources.StartupPage;
            webBrowser.DocumentCompleted += (sender2, e2) => {
                BodyLoaded(webBrowser.Document);
            };
        }

        private void BodyLoaded(HtmlDocument doc) {
            outputSheet = new OutputSheet(doc);
            interpreter = new Interpreter(outputSheet);

            input.KeyPress += (sender, e) => {
                if (e.KeyChar == 13) {
                    OnEnter();
                    e.Handled = true;
                }
            };

            input.Focus();
        }

        private void OnEnter() {
            string text = input.Text.Trim();
            input.Text = "";
            if (text.Length > 0) {
                interpreter.Interpret(text);
            }
        }
    }
}
