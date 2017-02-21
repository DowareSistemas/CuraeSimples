﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows;
using VarejoSimples.Model;
using VarejoSimples.Repository;

namespace VarejoSimples.Controller
{
    public class EstoqueController
    {
        private EstoqueRepository db = null;

        public EstoqueController()
        {
            db = new EstoqueRepository();
        }

        public void SetContext(varejo_config v)
        {
            db.Context = v;
        }

        public bool Save(Estoque est, varejo_config v = null)
        {
            try
            {
                if (v != null)
                    db.Context = v;
                if (db.Find(est.Id) == null)
                {
                    est.Id = db.NextId(e => e.Id);
                    db.Save(est);
                }
                else
                    db.Update(est);

                if (v == null)
                    db.Commit();
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public List<Estoque> ListByProduto(string search)
        {
            return db.Where(e => e.Produtos.Descricao.Contains(search) && e.Lote == string.Empty).ToList();
        }

        public string GeraProximoLote(string LoteAtual)
        {
            try
            {
                StreamReader reader = new StreamReader(Directory.GetCurrentDirectory() + @"\SX170220");
                List<string> dicionario = new List<string>();

                string line = "";
                while ((line = reader.ReadLine()) != null)
                    dicionario.Add(line);

                reader.Close();

                int numeroLote = 0;

                try
                {
                    int index = 0;
                    int value = 0;

                    while (!int.TryParse(LoteAtual.Substring(index), out value))
                    {
                        index++;
                    }

                    numeroLote = value;// int.Parse(LoteAtual.Substring(1));
                }
                catch { }

                string letraLote = "";
                try
                {
                    int index = 0;
                    int value = 0;

                    while (!int.TryParse(LoteAtual.Substring(index), out value))
                    {
                        letraLote += LoteAtual[index].ToString();
                        index++;
                    }
                }
                catch
                {

                }

                if (dicionario.IndexOf(letraLote[0].ToString()) == 26)
                    return null;

                if (numeroLote == 99999 && !(letraLote[0].ToString() == "Z"))
                {
                    char proximaLetra = dicionario[dicionario.IndexOf(letraLote[0].ToString()) + 1][0];
                    letraLote = letraLote.Replace(letraLote[0], proximaLetra);
                    numeroLote = 0;
                }

                if (numeroLote == 99999 && letraLote[0].ToString() == "Z")
                {
                    letraLote = "A" + letraLote;
                    numeroLote = 1;
                    LoteAtual = letraLote + "00001";
                    return LoteAtual;
                }

                numeroLote++;
                LoteAtual = (letraLote.ToString() + numeroLote.ToString().PadLeft((6 - numeroLote.ToString().Length), '0'));
                return UsuariosController.LojaAtual.Id + LoteAtual;
            }
            catch (Exception ex)
            {

            }

            return LoteAtual;
        }

        public void RemoveByProduto(int produto_id, varejo_config v = null)
        {
            if (v != null)
                db.Context = v;

            List<Estoque> estoques = db.Where(e => e.Produto_id == produto_id).ToList();
            if (estoques != null)
                estoques.ForEach(e => db.Remove(e));

            if (v == null)
                db.Commit();
        }

        public bool InsereEstoque(List<Estoque> estoques)
        {
            try
            {
                foreach (Estoque e in estoques)
                {
                    if (string.IsNullOrEmpty(e.Lote))
                        e.Lote = null;
                    if (!InsereEstoque(e.Quant, e.Produto_id, e.Loja_id, e.Lote))
                        return false;
                }
                return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public bool RetiraEstoque(List<Estoque> estoques)
        {
            try
            {
                foreach (Estoque e in estoques)
                {
                    if (string.IsNullOrEmpty(e.Lote))
                        e.Lote = null;

                    if (!RetiraEstoque(e.Quant, e.Produto_id, e.Loja_id, e.Lote))
                        return false;
                }
                return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public Estoque BuscarEstoqueProduto(string search)
        {
            Estoque est = BuscarPorLote(search);
            if (est != null)
                return est;

            Expression<Func<Estoque, bool>> expr = (p =>
            (p.Produto_id.ToString().Equals(search) ||
             p.Produtos.Ean.Equals(search) ||
             p.Produtos.Referencia.Equals(search)) &&
            (p.Lote.Equals("")));

            est = db.Where(expr).OrderBy(p => p.Lote).FirstOrDefault();
            return (est == null
                ? new Estoque()
                : est);
        }

        public Estoque BuscarPorLote(string lote)
        {
            try
            {
                string codlote = lote.Substring(0, lote.IndexOf("SL"));
                string codSublote = lote.Substring(lote.IndexOf("SL") + 2);

                return db.Where(e => e.Lote.Equals(codlote) && e.Sublote.Equals(codSublote)).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public bool RetiraEstoque(decimal quant, int produto_id, int loja_id, string lote = null)
        {
            Estoque estoque = (lote == null
                ? BuscarEstoqueProduto(produto_id.ToString())
                : BuscarEstoqueProduto(lote));

            if (estoque == null)
            {
                BStatus.Alert($"O estoque do produto {produto_id} não foi localizado com o parâmetros informados.");
                return false;
            }

            if (estoque.Quant <= 0)
            {
                if (ParametrosController.FindParametro("EST_SAIZERO").Valor.Equals("N"))
                {
                    MessageBox.Show($"Não é possível retirar o produto '{estoque.Produtos.Descricao}' do estoque porque o sistema está atualmente configurado para não permitir retiradas de estoque cujo o saldo atual é igual ou inferior a 0.", "EST_SAIZERO", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return false;
                }
            }

            estoque.Quant -= quant;

            if (estoque.Quant <= 0 && !string.IsNullOrEmpty(estoque.Lote))
                db.Remove(estoque);
            else
                db.Update(estoque);

            return true;
        }

        public bool InsereEstoque(decimal quant, int produto_id, int loja_id, string lote = null)
        {
            Estoque estoque = (lote == null
                ? BuscarEstoqueProduto(produto_id.ToString())
                : BuscarEstoqueProduto(lote));

            if (estoque.Produtos.Controla_lote)
                return true;
            
            estoque.Quant += quant;
            db.Update(estoque);
            return true;
        }

        private bool Valid(Estoque est)
        {
            if (est.Produtos.Controla_lote)
            {
                if (db.First(e => e.Lote.Equals(est.Lote) && e.Loja_id == UsuariosController.LojaAtual.Id) != null)
                {
                    BStatus.Alert($"O lote '{est.Lote}' já existe para a loja {UsuariosController.LojaAtual.Id}");
                    return false;
                }
            }

            return true;
        }
    }
}
