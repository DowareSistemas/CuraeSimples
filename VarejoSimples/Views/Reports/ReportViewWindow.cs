using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VarejoSimples.Views.Reports
{
    public partial class ReportViewWindow : Form
    {
        public ReportViewWindow(string title, ReportDocument rd)
        {
            InitializeComponent();

            this.Text = title;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.crystalReportViewer.ReportSource = rd;
            this.ShowDialog();
        }
    }
}
