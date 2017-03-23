using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using VarejoSimples.Model;

namespace VarejoSimples.Interfaces
{
    public interface IPainelVenda
    {
        void AbreVenda(int tipo_movimento);
        decimal GetTotalParcial();
        void VendeItem(Itens_movimento item);
        void RemoveItem(int item_id);
        void InformaCliente(int cliente_id);
        void IncrementaItem(int item_id);
        void DecrementaItem(Estoque estoque);
        void DecrementaItem(int item_id);
        void AplicarDescontoItemReais(int item_id, decimal valor);
        void AplicarDescontoItemPercent(int item_id, decimal valor);
        void EfetuarPagamento(int forma_pagamento_id, decimal valor);
        bool Encerrar(decimal troco);
        decimal GetValorParcial();
        void NFCe();
        int TransformarEmPedido();

        bool ClienteInformado { get; }
        UserControl CurrentUserControl { get; }
    }
}
