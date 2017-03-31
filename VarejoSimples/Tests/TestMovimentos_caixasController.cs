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
    public class TestMovimentos_caixasController
    {
        [TestMethod]
        public void DEVE_LISTAR_KVP_FORMAS_PAG_VALOR_CASO_EXISTA_MOV_CAIXA()
        {
            UsuariosController.UsuarioAtual = new UsuariosController().Find(1);
            Movimentos_caixasController mc_controller = new Movimentos_caixasController();
            List<KeyValuePair<Formas_pagamento, decimal>> kvp = mc_controller.GetTotaisPorFormaPagamentoCaixaAtual();

            Assert.IsTrue(kvp.Count > 0);
        }
    }
}
