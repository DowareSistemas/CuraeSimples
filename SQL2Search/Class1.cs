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
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("*******************************************************");
                Console.WriteLine("********            Doware Sistemas            ********");
                Console.WriteLine("********          www.doware.com.br            ********");
                Console.WriteLine("********   Curae Utility Tools - SQL2Search    ********");
                Console.WriteLine("*******************************************************");
                Console.WriteLine("");
                Console.WriteLine("Ferramenta de compilação SQL2Search");
                Console.WriteLine("Este programa é destinado a compilação de arquivos .csql");
                Console.WriteLine("");

                Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.NoneEnabled;
                Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocorreu um erro durante a compilação: \n" + ex.Message, "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void Run()
        {
            try
            {
                Console.WriteLine("Caminho do arquivo SQL de origem: ");
                string source = Console.ReadLine();

                Console.WriteLine("Pasta de destino da saída do arquivo .csql: ");
                string target = Console.ReadLine();

                SQLEntityCompiler compiler = new SQLEntityCompiler();
                compiler.Compile(source, target);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("");
                Console.WriteLine("Ocorreu um problema durante a compilação do arquivo .csql. \n" + ex.Message);
                Console.ForegroundColor = ConsoleColor.White;

                Run();
            }
        }
    }
}