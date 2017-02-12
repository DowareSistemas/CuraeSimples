using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;

namespace Base.Controller_Reports
{
    public class ReportController : IControllerReport
    {
        private List<ReportDataSource> DataSources = null;
        private ReportController()
        {
            DataSources = new List<ReportDataSource>();
        }

        public List<ReportFile> ReportFiles(string prefix)
        {
            List<ReportFile> result = new List<ReportFile>();
            try
            {
                string fileHelp = Directory.GetCurrentDirectory() + @"\Relatorios\reports.txt";
                StreamReader reader = new StreamReader(fileHelp);
                List<KeyValuePair<string, string>> modelos = new List<KeyValuePair<string, string>>();

                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    string[] names = line.Split('=');
                    if (names[0].StartsWith(prefix))
                        result.Add(new ReportFile()
                        {
                            FisicalName = names[0] + ".rpt",
                            LogicalName = names[0],
                            Alias = names[1]
                        });
                }
                reader.Close();
            }
            catch { }
            return result;
        }

        public static IControllerReport GetInstance()
        {
            return new ReportController();
        }

        public void AddDataSource(string name, DataTable datatable)
        {
            DataSources.Add(new ReportDataSource()
            {
                Source_Type = ReportDataSource.SOURCE_TYPE.DataTable,
                Name = name,
                DataTable = datatable
            });
        }

        public void AddDataSource(string name, IEnumerable enumerable)
        {
            DataSources.Add(new ReportDataSource()
            {
                Source_Type = ReportDataSource.SOURCE_TYPE.IEnumerable,
                Name = name,
                IEnumerable = enumerable
            });
        }

        public ReportDocument GetReportDocument(string reportFileName)
        {
            if (!reportFileName.EndsWith(".rpt"))
                reportFileName += ".rpt";
            ReportDocument rd = new ReportDocument();
            rd.Load(Directory.GetCurrentDirectory() + $@"\Relatorios\{reportFileName}");

            foreach (Table table in rd.Database.Tables)
            {
                ReportDataSource rds = DataSources.First(e => e.Name.Equals(table.Name));
                switch (rds.Source_Type)
                {
                    case ReportDataSource.SOURCE_TYPE.IEnumerable:
                        table.SetDataSource(rds.IEnumerable);
                        break;

                    case ReportDataSource.SOURCE_TYPE.DataTable:
                        table.SetDataSource(rds.DataTable);
                        break;
                }
            }
            return rd;
        }
    }
}
