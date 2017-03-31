using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Controller;
using VarejoSimples.Model;

namespace VarejoSimples.Tests
{
    [TestClass]
    public class TestProdutosController
    {
        public TestProdutosController()
        {
            UsuariosController.UsuarioAtual = new UsuariosController().Find(1);
            UsuariosController.LojaAtual = new LojasController().Find(1);
        }

        private void ClearProdutoTeste()
        {
            ProdutosController controller = new ProdutosController();
            List<Produtos> list = controller.Search("PRDTST");
            if (list.Count == 1)
                controller.Remove(list[0].Id);
        }

        private Produtos GetProduto()
        {
            ClearProdutoTeste();
            
            Produtos produto = new Produtos();
            produto.Ean = "1234567891234";
            produto.Referencia = "PRDTST";
            produto.Ncm = "454800000";
            produto.Localizacao = "LOCAL TESTE";
            produto.Unidade_id = 1;
            produto.Marca_id = 1;
            produto.Valor_unit = 10;
            produto.Descricao = "PRODUTO TESTE";
            produto.Controla_lote = false;
            produto.Controla_grade = false;
            produto.Aliquota = 18;
            produto.Fabricante_id = 1;
            produto.Grupo_id = 1;

            return produto;
        }

        [TestMethod]
        public void DEVE_SALVAR_UM_PRODUTO()
        {
            ProdutosController controller = new ProdutosController();
            Produtos produto = GetProduto();
            bool result = controller.Save(produto);

            controller = new ProdutosController();
            controller.Remove(produto.Id);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void DEVE_BUSCAR_UM_PRODUTO_PELA_ID()
        {
            ProdutosController controller = new ProdutosController();
            Produtos produto = controller.Find(1);

            Assert.IsTrue(produto != null);
        }

        [TestMethod]
        public void DEVE_BUSCAR_UM_PRODUTO_PELO_EAN()
        {
            ProdutosController controller = new ProdutosController();
            List<Produtos> list = controller.Search("4740000155788");

            Assert.IsTrue(list.Count > 0);
            Assert.IsTrue(list.First().Ean.Equals("4740000155788"));
        }

        [TestMethod]
        public void DEVE_BUSCAR_UM_PRODUTO_PELA_REFERENCIA()
        {
            ProdutosController controller = new ProdutosController();
            List<Produtos> list = controller.Search("BMB001");

            Assert.IsTrue(list.Count > 0);
            Assert.IsTrue(list.First().Referencia.Equals("BMB001"));
        }

        [TestMethod]
        public void DEVE_REMOVER_UM_PRODUTO()
        {
            ProdutosController controller = new ProdutosController();
            Produtos produto = GetProduto();
            controller.Save(produto);

            controller = new ProdutosController();
            bool result = controller.Remove(produto.Id);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void DEVE_SALVAR_PRODUTO_E_ADICIONAR_GRADE()
        {
            ProdutosController controller = new ProdutosController();
            Produtos produto = GetProduto();
            produto.Controla_grade = true;

            controller.Save(produto);

            Grades_produtosController gc = new Grades_produtosController();

            Grades_produtos grade = new Grades_produtos();
            grade.Produto_id = produto.Id;
            grade.Cor_id = 1;
            grade.Tamanho_id = 1;

            bool result = gc.Save(grade);
            controller = new ProdutosController();
            controller.Remove(produto.Id);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void DEVE_REMOVER_UM_PRODUTO_COM_GRADE()
        {
            ProdutosController controller = new ProdutosController();
            Produtos produto = GetProduto();
            produto.Controla_grade = true;

            controller.Save(produto);

            Grades_produtosController gc = new Grades_produtosController();

            Grades_produtos grade = new Grades_produtos();
            grade.Produto_id = produto.Id;
            grade.Cor_id = 1;
            grade.Tamanho_id = 1;

            gc.Save(grade);

            controller = new ProdutosController();
            bool result = controller.Remove(produto.Id);

            Assert.IsTrue(result);
        }
    }
}
