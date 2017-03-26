using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace VarejoSimples.Controller
{
    public class BStatusItem
    {

        public int Id { get; set; }
        public Label Label { get; set; }
        public Image Image { get; set; }
    }

    public class BStatus
    {
        private static List<BStatusItem> Itens = new List<BStatusItem>();

        public static void Attach(int id, Label label, Image image)
        {
            Itens.Add(new BStatusItem() { Id = id, Label = label, Image = image });
        }

        public static void Dettach(int id)
        {
            Itens.Remove(Itens.First(e => e.Id == id));
        }

        public static void Success(string text)
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    foreach (BStatusItem item in Itens)
                    {
                        item.Label.Content = $"[{DateTime.Now.ToString()}]: " + text;
                        LogController.WriteLog(text);
                        item.Image.Source = new BitmapImage(new Uri(@"/Images/sucess.png", UriKind.Relative));
                    }
                }));
            }
            catch { }
        }

        public static void ErrorOnSave(string entity, string exception)
        {
            try {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    foreach (BStatusItem item in Itens)
                    {
                        string msg = $"Ocorreu um problema ao salvar {entity}. Acione o suporte Doware.";
                        LogController.WriteLog(msg + "\n" + exception);
                        item.Label.Content = $"[{DateTime.Now.ToString()}]: " + msg;
                        item.Image.Source = new BitmapImage(new Uri(@"/Images/error.png", UriKind.Relative));
                    }
                }));
            }
            catch { }
        }

        internal static void Error(string v)
        {
            try {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    foreach (BStatusItem item in Itens)
                    {
                        string msg = v;
                        item.Label.Content = $"[{DateTime.Now.ToString()}]: " + msg;
                        item.Image.Source = new BitmapImage(new Uri(@"/Images/error.png", UriKind.Relative));
                    }
                }));
            }
            catch { }
        }

        public static void ErrorOnFind(string entity, string exception)
        {
            try {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    foreach (BStatusItem item in Itens)
                    {
                        string msg = $"Ocorreu um problema ao carregar {entity}. Acione o suporte Doware.";
                        LogController.WriteLog(msg + "\n" + exception);
                        item.Label.Content = $"[{DateTime.Now.ToString()}]: " + msg;
                        item.Image.Source = new BitmapImage(new Uri(@"/Images/error.png", UriKind.Relative));
                    }
                }));
            }
            catch { }
        }

        internal static void Alert(string v)
        {
            try {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    foreach (BStatusItem item in Itens)
                    {
                        item.Label.Content = $"[{DateTime.Now.ToString()}]: " + v;
                        item.Image.Source = new BitmapImage(new Uri(@"/Images/warning.png", UriKind.Relative));
                    }
                }));
            }
            catch { }
        }

        public static void ErrorOnRemove(string entity, string exception)
        {
            try {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    foreach (BStatusItem item in Itens)
                    {
                        string msg = $"[{DateTime.Now.ToString()}]: " + $"Ocorreu um problema ao remover {entity}. Acione o suporte Doware.";
                        LogController.WriteLog(msg + "\n" + exception);
                        item.Label.Content = msg;
                        item.Image.Source = new BitmapImage(new Uri(@"/Images/error.png", UriKind.Relative));
                    }
                }));
            }
            catch
            {

            }
        }

        public static void ErrorOnList(string entity, string exception)
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    foreach (BStatusItem item in Itens)
                    {
                        string msg = $"[{DateTime.Now.ToString()}]: " + $"Ocorreu um problema ao listar {entity}. Acione o suporte Doware.";
                        LogController.WriteLog(msg + "\n" + exception);
                        item.Label.Content = msg;
                        item.Image.Source = new BitmapImage(new Uri(@"/Images/error.png", UriKind.Relative));
                    }
                }));
            }
            catch { }
        }
    }
}
