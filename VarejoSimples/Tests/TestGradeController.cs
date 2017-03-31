using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Controller;

namespace VarejoSimples.Tests
{
    [TestClass]
    public class TestGradeController
    {
        public TestGradeController()
        {
            UsuariosController.UsuarioAtual = new UsuariosController().Find(1);
            UsuariosController.LojaAtual = new LojasController().Find(1);
        }

        [TestMethod]
        public void DEVE_EXCLUIR_GRADE_COMPLETA_PRODUTO()
        {
            Grades_produtosController controller = new Grades_produtosController();
            bool result = controller.RemoveGradeCompleto("1181758741");

            Assert.IsTrue(result);
        }
    }
}
