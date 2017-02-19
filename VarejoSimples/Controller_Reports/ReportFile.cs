using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base.Controller_Reports
{
    public class ReportFile
    {
        /// <summary>
        /// Nome fisico real do RPT
        /// Ex: PFRN001.rpt
        /// </summary>
        public string FisicalName { get; set; }

        /// <summary>
        /// Nome lógico do RPT.
        /// Semelhante ao nome fisico real, porém,
        /// sem o ".rpt"
        /// Ex: PFRN001
        /// </summary>
        public string LogicalName { get; set; }

        /// <summary>
        /// Descrição amigável para o relatório
        /// Ex: Produtos por fornecedor
        /// </summary>
        public string Alias { get; set; }
    }
}
