using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VarejoSimples.Controller
{
    public class LogController
    {
        public static void WriteLog(string msg)
        {
            StreamWriter writer = null;
            try
            {
                if (!Directory.Exists(@"C:\Temp\"))
                    Directory.CreateDirectory(@"C:\Temp\");

                string fileName = @"C:\Temp\Doware-" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";

                writer = (File.Exists(fileName)
                    ? File.AppendText(fileName)
                    : new StreamWriter(fileName));

                writer.WriteLine("+--------------------------------------------------------------------------+");

                if (UsuariosController.UsuarioAtual != null)
                {
                    writer.WriteLine(" Usuário ID: " + UsuariosController.UsuarioAtual.Id);
                    writer.WriteLine(" Usuário Nome: " + UsuariosController.UsuarioAtual.Nome);
                }

                writer.WriteLine($" [{DateTime.Now.ToString()}]: {msg}");
                writer.WriteLine("+--------------------------------------------------------------------------+");
                writer.WriteLine(Environment.NewLine);
                writer.Close();
            }
            catch (Exception ex)
            {
                if (writer != null)
                    writer.Close();

                WriteLog(msg);
            }
        }
    }
}
