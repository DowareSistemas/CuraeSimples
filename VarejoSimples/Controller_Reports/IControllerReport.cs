using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Base.Controller_Reports
{
    public interface IControllerReport
    {
        ReportDocument GetReportDocument(string reportFileName);
        void AddDataSource(string name, System.Collections.IEnumerable enumerable);
        void AddDataSource(string name, DataTable datatable);
        void BindParameter(string paramenterName, object value);
        List<ReportFile> ReportFiles(string prefix);
        void ShowReport(string title, string reportFileName);
    }
}
