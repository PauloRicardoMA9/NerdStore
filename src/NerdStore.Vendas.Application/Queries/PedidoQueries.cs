﻿using NerdStore.Vendas.Application.Queries.ViewModels;
using NerdStore.Vendas.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdStore.Vendas.Application.Queries
{
    public class PedidoQueries : IPedidoQueries
    {
        private readonly IPedidoRepository _pedidoRepository;

        public PedidoQueries(IPedidoRepository pedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
        }

        public async Task<CarrinhoViewModel> ObterCarrinhoCliente(Guid clienteId)
        {
            var pedido = await _pedidoRepository.ObterPedidoRascunhoPorClienteId(clienteId);
            if(pedido == null) { return null; }

            var carrinho = new CarrinhoViewModel
            {
                ClienteId = pedido.ClienteId,
                ValorTotal = pedido.ValorTotal,
                PedidoId = pedido.Id,
                ValorDesconto = pedido.Desconto,
                SubTotal = pedido.Desconto + pedido.ValorTotal
            };

            if (pedido.VoucherId != null)
            {
                carrinho.VoucherCodigo = pedido.Voucher.Codigo;
            }

            foreach(var item in pedido.PedidoItens)
            {
                carrinho.Itens.Add(new CarrinhoItemViewModel
                {
                    ProdutoId = item.ProdutoId,
                    ProdutoNome = item.ProdutoNome,
                    Quantidade = item.Quantidade,
                    ValorUnitario = item.ValorUnitario,
                    ValorTotal = item.ValorUnitario * item.Quantidade
                });
            }

            return carrinho;
        }

        public async Task<IEnumerable<PedidoViewModel>> ObterPedidosClientes(Guid clienteId)
        {
            var pedidos = await _pedidoRepository.ObterListaPorClienteId(clienteId);

            pedidos = pedidos.Where(pedido => pedido.PedidoStatus == PedidoStatus.Pago || pedido.PedidoStatus == PedidoStatus.Cancelado)
                .OrderByDescending(pedido => pedido.Codigo);

            if(!pedidos.Any()) { return null; }

            var pedidosView = new List<PedidoViewModel>();

            foreach(var pedido in pedidos)
            {
                pedidosView.Add(new PedidoViewModel
                {
                    ValorTotal = pedido.ValorTotal,
                    PedidoStatus = (int)pedido.PedidoStatus,
                    Codigo = pedido.Codigo,
                    DataCadastro = pedido.DataCadastro
                });
            }

            return pedidosView;
        }
    }
}
