using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace VarejoSimples.Controller
{
    public abstract class AsyncTask<TParams, TProgress, TReturn>
    {
        private TReturn vResult;
        private TParams vParams;
        private BackgroundWorker worker = null;
        public abstract TReturn DoInBackGround(TParams param);
        public abstract void OnPostExecute(TReturn result);
        public abstract void OnProgressUpdate(TProgress progress);
        public abstract void OnPreExecute();
        public void UpdateProgress(TProgress progress)
        {
            worker.ReportProgress(0, progress);
        }

        public void Execute(TParams prms)
        {
            OnPreExecute();
            vParams = prms;

            worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnPostExecute(vResult);
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            OnProgressUpdate((TProgress)e.UserState);
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            vResult = DoInBackGround(vParams);
        }
    }
}
