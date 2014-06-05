using PagCoin.Net.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace PagCoin.Net
{
    public class Pagcoin
    {
        private const string URL_CRIAR_ORDEM_PAGAMENTO = "https://pagcoin.com/api/v1/CriarInvoice/";
        public string ApiKey { get; set; }

        public Pagcoin(string apikey)
        {
            ApiKey = apikey;
        }

        /// <summary>
        /// Chama a API PagCoin para solicitar a criação de uma Ordem de Pagamento
        /// </summary>
        /// <param name="ordem">Ordem de pagamento com dados preenchidos.</param>
        /// <returns>URL para redirect do usuário, contendo a ordem de pagamento com o respectivo código de carteira.</returns>
        /// <remarks>Leia https://pagcoin.com/Home/Desenvolvedores para verificar os campos obrigatórios, opcionais e ignorados do objeto OrdemPagamento para a criação de uma ordem de pagamento</remarks>
        public string CriarOrdemDePagamento(OrdemPagamento ordem)
        {
            var json = SerializarOrdemPagamentoParaJSON(ordem);
            var assinatura = GerarAssinaturaString(ApiKey, URL_CRIAR_ORDEM_PAGAMENTO + json);

            var request = HttpWebRequest.Create(URL_CRIAR_ORDEM_PAGAMENTO);
            request.Headers["EnderecoPagCoin"] = URL_CRIAR_ORDEM_PAGAMENTO;
            request.Headers["AssinaturaPagCoin"] = assinatura;
            request.Method = "POST";
            request.ContentType = "application/json";

            var erro = default(string);
            var responseString = default(string);

            var getRequestAsyncResult = request.BeginGetRequestStream(asynchronousResult => {}, request);
            getRequestAsyncResult.AsyncWaitHandle.WaitOne();

            var postStream = request.EndGetRequestStream(getRequestAsyncResult);
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(json);

                postStream.Write(byteArray, 0, json.Length);
                postStream.Flush();
                postStream.Close();
            }

            var getResponseAsyncResult = request.BeginGetResponse(asynchronousResultResponse => {}, request);
            getResponseAsyncResult.AsyncWaitHandle.WaitOne();

            using (var response = (HttpWebResponse)request.EndGetResponse(getResponseAsyncResult))
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    erro = "Erro ao solicitar a criação do Invoice. Código do erro: " + (int)response.StatusCode;
                }
                if (!ValidarAssinatura(response, out responseString))
                {
                    throw new Exception("Assinatura inválida enviada pelo servidor. Por segurança, o código de Invoice recebido não será informado");
                }
            }
            
            if (!string.IsNullOrEmpty(erro))
            {
                throw new Exception(erro + " \nDescrição:" + responseString ?? "N/A");
            }

            return "https://pagcoin.com" + responseString;
        }

        /// <summary>
        /// Valida se a resposta enviada pelo PagCoin é autentica.
        /// </summary>
        /// <param name="response">WebResponse recebido após efetuar o request</param>
        /// <param name="conteudo">Variável out para fornecer o conteúdo da resposta, não necessitando efetuar a leitura da stream novamente.</param>
        /// <returns>true caso válida, falso caso inválida</returns>
        public bool ValidarAssinatura(WebResponse response, out string conteudo)
        {
            var assinatura = response.Headers["AssinaturaPagCoin"];
            var endereco = response.Headers["EnderecoPagCoin"];

            using (Stream streamResponse = response.GetResponseStream())
            {
                using (var streamRead = new StreamReader(streamResponse))
                {
                    conteudo = streamRead.ReadToEnd();
                }
            }

            var assinaturaValida = ValidarAssinatura(assinatura, endereco, conteudo);

            if (!assinaturaValida)
            {
                conteudo = null;
            }

            return assinaturaValida;
        }

        /// <summary>
        /// Valida se uma requisição recebida, que tenha sido enviada pelo PagCoin, é autentica.
        /// </summary>
        /// <param name="request">WebRequest recebido após efetuar o request</param>
        /// <param name="conteudo">Variável out para fornecer o conteúdo da resposta, não necessitando efetuar a leitura da stream novamente.</param>
        /// <returns>true caso válida, falso caso inválida</returns>
        /// <remarks>Não use este método passando uma WebRequest montada por você. Apenas use para requisições recebidas.</remarks>
        public bool ValidarAssinatura(WebRequest request, out string conteudo)
        {
            var assinatura = request.Headers["AssinaturaPagCoin"];
            var endereco = request.Headers["EnderecoPagCoin"];

            var getRequestAsyncResult = request.BeginGetRequestStream(asynchronousResult => { }, request);
            getRequestAsyncResult.AsyncWaitHandle.WaitOne();

            var requestStream = request.EndGetRequestStream(getRequestAsyncResult);
            using (var streamRead = new StreamReader(requestStream))
            {
                conteudo = streamRead.ReadToEnd();
            }

            var assinaturaValida = ValidarAssinatura(assinatura, endereco, conteudo);

            if (!assinaturaValida)
            {
                conteudo = null;
            }

            return assinaturaValida;
        }

        /// <summary>
        /// Valida uma assinatura, baseada nos valores de endereço e conteúdo
        /// </summary>
        /// <param name="assinaturaPagCoin">Deve ser informado o valor do campo AssinaturaPagCoin da requisição HTTP</param>
        /// <param name="enderecoPagCoin">Deve ser informado o valor do campo EnderecoPagCoin da requisição HTTP</param>
        /// <param name="conteudo">Conteúdo da requisição</param>
        /// <returns>true caso válida, falso caso inválida</returns>
        public bool ValidarAssinatura(string assinaturaPagCoin, string enderecoPagCoin, string conteudo)
        {
            return (GerarAssinaturaString(enderecoPagCoin, conteudo) == assinaturaPagCoin);
        }

        /// <summary>
        /// Calcula o HMAC SHA256 para a API Key da instância a partir do endereço e do conteúdo
        /// </summary>
        /// <param name="toSignString"></param>
        /// <returns>Assinatura HMACSHA256 válida para o PagCoin</returns>
        public string GerarAssinaturaString(string enderecoPagCoin, string conteudo)
        {
            return GerarAssinaturaString(enderecoPagCoin + conteudo);
        }

        /// <summary>
        /// Calcula o HMAC SHA256 para a API Key da instância e a string informada
        /// </summary>
        /// <param name="toSignString"></param>
        /// <returns>Assinatura HMACSHA256 válida para o PagCoin</returns>
        public string GerarAssinaturaString(string toSignString)
        {
            var toSignBytes = Encoding.UTF8.GetBytes(toSignString);
            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(ApiKey));
            var digest = hmac.ComputeHash(toSignBytes);
            return Convert.ToBase64String(digest);
        }

        private static string SerializarOrdemPagamentoParaJSON(OrdemPagamento ordem)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(ordem);
        }
    }
}
