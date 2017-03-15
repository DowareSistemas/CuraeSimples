using iTextSharp.text;
using iTextSharp.text.pdf;
using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace NFCe_Spool
{
    public partial class Form1 : Form
    {
        string arquivoDanfe = string.Empty;
        string impressora = string.Empty;

        public Form1()
        {
            string parametros = "";
            string line = "";
            StreamReader reader = new StreamReader(Directory.GetCurrentDirectory() + @"\PARAMS.txt");
            while ((line = reader.ReadLine()) != null)
            {
                parametros += line;
            }
            reader.Close();

            if (parametros != null)
            {
                string[] pair = parametros.Split(';');
                arquivoDanfe = pair[0];
                impressora = pair[1].TrimStart();
            }

            InitializeComponent();
            this.Show();
        }

        private void Resize()
        {
            PdfReader reader = new PdfReader(arquivoDanfe);
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc,
            new FileStream(@"c:\Temp\Out.PDF", FileMode.Create));
            doc.Open();
            PdfContentByte cb = writer.DirectContent;
            PdfImportedPage page = writer.GetImportedPage(reader, 1); //page #1
            float Scale = 0.85f;
            cb.AddTemplate(page, Scale, 0, 0, 1, 200, 0);
            doc.Close();
        }

        private void Print()
        {
            try
            {
                Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument();
                doc.LoadFromFile(@"c:\Temp\Out.PDF");
                doc.PageScaling = PdfPrintPageScaling.ActualSize;

                PrintDialog dialogPrint = new PrintDialog();
                dialogPrint.AllowPrintToFile = true;
                dialogPrint.AllowSomePages = true;
                dialogPrint.PrinterSettings.MinimumPage = 1;
                dialogPrint.PrinterSettings.MaximumPage = doc.Pages.Count;
                dialogPrint.PrinterSettings.FromPage = 1;
                dialogPrint.PrinterSettings.ToPage = doc.Pages.Count;

                //Set the pagenumber which you choose as the start page to print
                doc.PrintFromPage = dialogPrint.PrinterSettings.FromPage;
                //Set the pagenumber which you choose as the final page to print
                doc.PrintToPage = dialogPrint.PrinterSettings.ToPage;
                //Set the name of the printer which is to print the PDF
                doc.PrinterName = dialogPrint.PrinterSettings.PrinterName;

                PrintDocument printDoc = doc.PrintDocument;
                dialogPrint.Document = printDoc;
                printDoc.PrinterSettings.PrinterName = impressora;
                printDoc.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocorreu um problema ao processar a DANFE.\n{ex.Message}", "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.VisibleChanged += Form1_VisibleChanged;
        }

        private void Form1_VisibleChanged(object sender, EventArgs e)
        {
            new Thread(() =>
           {
               Resize();
               Print();
               Environment.Exit(0);
           }).Start();
        }
    }
}
