using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Model;
using VarejoSimples.Repository;

namespace VarejoSimples.Controller
{
    public class Grades_produtosController
    {
        private Grades_produtosRepository db = null;

        public Grades_produtosController()
        {
            db = new Grades_produtosRepository();
        }

        public string GerarIdentificadorGrade()
        {
            Random ramdom = new Random(DateTime.Now.Day + DateTime.Now.Minute + DateTime.Now.Year + DateTime.Now.Second);
            int id = ramdom.Next();

            string identificador = id.ToString();

            if (identificador.Length < 10)
                identificador = identificador.PadRight(10, '0');
            if (identificador.Length > 10)
                identificador = identificador.Substring(0, 10);

            if (db.Find(identificador) == null)
                return identificador;
            else
                return GerarIdentificadorGrade();
        }

        public bool Save(Grades_produtos grade)
        {
            UnitOfWork unit = null;
            try
            {
                unit = new UnitOfWork();
                unit.BeginTransaction();
                db.Context = unit.Context;

                if (db.Find(grade.Identificador) == null)
                {
                    grade.Identificador = GerarIdentificadorGrade();
                    db.Save(grade);

                    Estoque estoque = new Estoque();
                    estoque.Produto_id = grade.Produto_id;
                    estoque.Loja_id = UsuariosController.LojaAtual.Id;
                    estoque.Quant = 0;
                    estoque.Lote = "";
                    estoque.Sublote = "";
                    estoque.Grade_id = grade.Identificador;

                    EstoqueController ec = new EstoqueController();
                    ec.SetContext(unit.Context);
                    if (!ec.Save(estoque, true))
                    {
                        unit.RollBack();
                        return false;
                    }
                }
                else
                    db.Update(grade);

                unit.Commit();
                BStatus.Success("Grade de produto salva");
                return true;
            }
            catch (Exception ex)
            {
                if (unit != null)
                    unit.RollBack();

                return false;
            }
        }

        public List<Grades_produtos> ListByProduto(int produto_id)
        {
            return db.Where(e => e.Produto_id == produto_id).ToList();
        }

        public Grades_produtos Find(string identificador)
        {
            return db.Find(identificador);
        }

        public bool RemoveGradeCompleto(string identificador)
        {
            UnitOfWork unitOfWork = null;
            try
            {
                unitOfWork = new UnitOfWork();
                unitOfWork.BeginTransaction();
                db.Context = unitOfWork.Context;

                EstoqueController eController = new EstoqueController();

                if (!eController.RemoveByGrade(identificador, unitOfWork))
                {
                    unitOfWork.RollBack();
                    return false;
                }

                Grades_produtos grade = Find(identificador);
                db.Remove(grade);
                unitOfWork.Commit();

                return true;
            }
            catch (Exception ex)
            {
                unitOfWork.RollBack();
                return false;
            }
        }

        internal bool RemoveApenasGrade(string identificador, UnitOfWork unitOfWork)
        {
            try
            {
                db.Context = unitOfWork.Context;

                Grades_produtos grade = Find(identificador);
                db.Remove(grade);
                db.Commit();

                return true;
            }
            catch (Exception ex)
            {
                db.RollBack();
                return false;
            }
        }

        internal void SetContext(varejo_config context)
        {
            db.Context = context;
        }
    }
}
