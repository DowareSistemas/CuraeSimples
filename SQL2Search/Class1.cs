using SQL2Search.Compiler;
using SQL2Search.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SQL2Search
{
    public class Class1
    {
        public static void Main(string[] args)
        {
            try
            {
                Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.NoneEnabled;

                string source = new InputBox("Caminho do arquivo SQL de origem").Value;
                string target = new InputBox("Pasta de destino da saída").Value;

                SQLEntityCompiler compiler = new SQLEntityCompiler();
                compiler.Compile(source, target);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocorreu um erro durante a compilação: \n" + ex.Message, "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}