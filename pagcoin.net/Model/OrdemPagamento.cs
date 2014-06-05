namespace PagCoin.Net.Model
{
    public class OrdemPagamento
    {
        public string apiKey { get; set; }
        public string idPagCoin { get; set; }
        public string idInterna { get; set; }
        public string nomeProduto { get; set; }
        public string moedaOriginal { get; set; }
        public decimal valorEmMoedaOriginal { get; set; }
        public string email { get; set; }
        public string statusPagamento { get; set; }
        public string redirectURL { get; set; }
        public int hora { get; set; }
        public int horaResposta { get; set; }
    }
}
