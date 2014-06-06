namespace PagCoin.Net.Model
{
    public enum StatusPagamento
    {
        Iniciada = 1,
        Recebida = 2,
        Confirmada = 3,
        Rejeitada = 6,
        ParcialmenteRecebida = 7,
        Timeout = 8
    }
}
