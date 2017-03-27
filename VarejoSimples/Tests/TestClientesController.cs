using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Controller;
using VarejoSimples.Model;
using System.Windows.Forms;

namespace VarejoSimples.Tests
{
    [TestClass]
    public class TestClientesController
    {
        int CLIENTE_ID_BASE = 11;
        [TestMethod]
        public void DEVE_EXECUTAR_CADASTRO_CLIENTE()
        {
            Clientes cliente = new Clientes();
            cliente.Nome = "Teste 001";
            cliente.Telefone = "3345-4040";
            cliente.Celular = "99948-5454";
            cliente.Logradouro = "Logradouro teste";
            cliente.Municipio = "Municipio teste";
            cliente.Uf = "RJ";
            cliente.Bairro = "Bairro teste";
            cliente.Cpf = "444.999.777-00";
            cliente.Email = "marcos8154@gmail.com";
            cliente.Cep = "27281-440";
            cliente.Numero = 12;

            ClientesController clientesController = new ClientesController();
            Assert.IsTrue(clientesController.Save(cliente));
        }

        [TestMethod]
        public void DEVE_EXECUTAR_BUSCA_CLIENTES_CASO_EXISTA()
        {
            ClientesController controller = new ClientesController();
            List<Clientes> resultList = controller.Search("Mar");

            Assert.IsTrue(resultList.Count > 0);
        }

        [TestMethod]
        public void DEVE_BUSCAR_CLIENTE_APARTIR_DO_CPF()
        {
            ClientesController controller = new ClientesController();
            List<Clientes> resultList = controller.Search("444.999.777-00");

            Assert.IsTrue(resultList.FirstOrDefault(e => e.Cpf.Equals("444.999.777-00")) != null);
        }

        [TestMethod]
        public void DEVE_BUSCAR_CLIENTE_APARTIR_DO_ID()
        {
            ClientesController controller = new ClientesController();
            List<Clientes> resultList = controller.Search(CLIENTE_ID_BASE.ToString());

            Assert.IsTrue(resultList.FirstOrDefault(e => e.Id == CLIENTE_ID_BASE) != null);
        }

        [TestMethod]
        public void DEVE_EXCLUIR_CLIENTE_CASO_EXISTA()
        {
            ClientesController controller = new ClientesController();
            Assert.IsTrue(controller.Remove(CLIENTE_ID_BASE));
        }
    }
}
