using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VarejoSimples.Controller
{
    public abstract class AsyncTask<TParams, TProgress, TReturn>
    {
        private bool Cancelable = false;
        private TReturn Result { get; set; }
        private TParams Params { get; set; }
        private BackgroundWorker MainWorker { get; set; }
        private int Progress = 0;

        public abstract TReturn DoInBackGround(TParams param);
        public abstract void OnPostExecute(TReturn result);
        public abstract void OnProgressUpdate(TProgress progress);
        public abstract void OnPreExecute();

        public void UpdateProgress(TProgress progress)
        {
            MainWorker.ReportProgress((Progress++), progress);
        }

        public void Cancel()
        {
            if (!Cancelable)
            {
                MessageBox.Show("Uma AsynkTask não pode ser cancelada antes ou após DoInBackground.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MainWorker.CancelAsync();
        }

        public void Execute(TParams prms)
        {
            OnPreExecute();
            Params = prms;

            MainWorker = new BackgroundWorker();
            MainWorker.WorkerReportsProgress = true;
            MainWorker.DoWork += Worker_DoWork;
            MainWorker.ProgressChanged += Worker_ProgressChanged;
            MainWorker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            
            MainWorker.RunWorkerAsync();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnPostExecute(Result);
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            OnProgressUpdate((TProgress)e.UserState);
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Cancelable = true;
            Result = DoInBackGround(Params);
            Cancelable = false;
        }
    }
}
