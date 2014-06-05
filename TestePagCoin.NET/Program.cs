using PagCoin.Net;
using PagCoin.Net.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagCoin.Testes.Console
{
    class Program
    {
        private static string apikeyTeste = "INSIRA_A_API_KEY";

        static void Main(string[] args)
        {
            Pagcoin pc = new Pagcoin(apikeyTeste);
            OrdemPagamento op = new OrdemPagamento();
            op.apiKey = apikeyTeste;
            op.nomeProduto = "Teste via API: PagCoin.NET";
            op.valorEmMoedaOriginal = 12.50M;
            op.idInterna = "T0001";

            var retorno = pc.CriarOrdemDePagamento(op);
            System.Console.WriteLine(retorno);

            System.Console.ReadLine();
        }
    }
}
