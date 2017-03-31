using SQL2Search.Compiler;
using SQL2Search.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using VarejoSimples.Controller;
using VarejoSimples.Model;
using VarejoSimples.Views.Setup;

namespace VarejoSimples.Tasks
{
    public class SetupTask : AsyncTask<int, string, int>
    {
        private PreparacaoSistema View { get; set; }

        public SetupTask(PreparacaoSistema view)
        {
            View = view;
        }

        public override int DoInBackGround(int param)
        {
            try
            {
                UpdateProgress("Criando sua base de dados...");
                Thread.Sleep(5000);
                varejo_config db = new varejo_config();
                db.Database.Create();

                UpdateProgress("Estamos configurando algumas coisas para você...");

                SQLEntityDecompiler decompiler = new SQLEntityDecompiler();
                SQLEntity entity = decompiler.Decompile(Directory.GetCurrentDirectory() + @"\setup.csql");

                string[] commands = entity.FullSQLCommand.Split(';');

                db.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);

                foreach (string cmd in commands)
                    if (!string.IsNullOrWhiteSpace(cmd))
                    {
                        db.Database.ExecuteSqlCommand(cmd);
                        Thread.Sleep(3000);
                    }

                UpdateProgress("Estamos quase terminando...");
                Thread.Sleep(5000);
                db.Database.CurrentTransaction.Commit();

                UpdateProgress("Vamos começar");
                Thread.Sleep(20000);
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public override void OnPostExecute(int result)
        {
            View.Close();
        }

        public override void OnPreExecute()
        {
            View.lbProgresso.Content = string.Empty;
        }

        public override void OnProgressUpdate(string progress)
        {
            View.lbProgresso.Content = progress;

            if (progress == "Vamos começar")
                View.imgLoading.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
