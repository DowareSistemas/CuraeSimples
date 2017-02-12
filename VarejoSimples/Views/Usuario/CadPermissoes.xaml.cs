using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VarejoSimples.Controller;
using VarejoSimples.Model;

namespace VarejoSimples.Views.Usuario
{
    /// <summary>
    /// Lógica interna para CadPermissoes.xaml
    /// </summary>
    public partial class CadPermissoes : Window
    {
        public CadPermissoes()
        {
            InitializeComponent();
            dataGrid.AplicarPadroes(false);
            ListaPermissoes();
        }

        private void ListaPermissoes()
        {
            List<Rotinas> list = new RotinasController().ListAll();
            List<PermissoesAdapter> permissoes = new List<PermissoesAdapter>();
            list.ForEach(e => permissoes.Add(new PermissoesAdapter(e)));

            dataGrid.ItemsSource = permissoes;
        }

        private void btSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(txCod_usuario.Text) == 0)
            {
                BStatus.Alert("Informe o usuário antes de aplicar as permissões");
                return;
            }

            foreach (var item in dataGrid.Items)
            {
                PermissoesAdapter pa = (item as PermissoesAdapter);
                PermissoesController pc = new PermissoesController();

                Permissoes permissoes = (pa.Permissao_id == 0
                            ? new Permissoes()
                            : pc.Find(pa.Permissao_id));

                permissoes.Id = pa.Permissao_id;
                permissoes.Usuario_id = int.Parse(txCod_usuario.Text);
                permissoes.Rotina_id = pa.Rotina_id;
                permissoes.Acesso = pa.Acesso;
                permissoes.Salvar = pa.Salvar;
                permissoes.Excluir = pa.Excluir;
                
                if (!pc.Save(permissoes))
                    break;

                BStatus.Success($"Permissões aplicadas com sucesso!");
            }
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btSelecionarUsuario_Click(object sender, RoutedEventArgs e)
        {
            PesquisarUsuario pu = new PesquisarUsuario();
            pu.ShowDialog();

            txCod_usuario.Text = pu.Selecionado.Id.ToString();
            txNome_usuario.Text = (pu.Selecionado.Id == 0
                ? "Não selecionado"
                : pu.Selecionado.Nome);

            if (pu.Selecionado.Permissoes.Count > 0)
            {
                List<PermissoesAdapter> permissoesAdp = new List<PermissoesAdapter>();
                List<Permissoes> perms = pu.Selecionado.Permissoes.ToList();
                perms.ForEach(p => permissoesAdp.Add(new PermissoesAdapter(p)));

                dataGrid.ItemsSource = permissoesAdp;
            }
            else
                ListaPermissoes();
        }
    }

    public class PermissoesAdapter
    {
        public PermissoesAdapter(Rotinas rotina)
        {
            Rotina_id = rotina.Id;
            Rotina = rotina.Descricao;
        }

        public PermissoesAdapter(Permissoes permissoes)
        {
            if (permissoes.Rotinas != null)
                Rotina = permissoes.Rotinas.Descricao;

            Rotina_id = permissoes.Rotina_id;
            Permissao_id = permissoes.Id;
            Acesso = permissoes.Acesso;
            Salvar = permissoes.Salvar;
            Excluir = permissoes.Excluir;
        }

        public int Permissao_id { get; set; }
        public int Rotina_id { get; set; }
        public string Rotina { get; set; }
        public bool Acesso { get; set; }
        public bool Salvar { get; set; }
        public bool Excluir { get; set; }
    }
}
