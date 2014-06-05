pagcoin.net
===========

.NET library to integrate with PagCoin

PagCoin.NET é uma biblioteca PCL (Portable Class Library), compatível com .NET Framework 4.5, além de poder ser usada também no desenvolvimento de aplicações Windows Store e Windows Phone 7 e 8.

###Configurando e Compilando o projeto###

O projeto está configurado para o Visual Studio 2012. Basta baixar o código, buildar em modo Release e usar a DLL resultante. Adicione referências também para as DLLs Lib\Portable, de acordo com sua necessidade:

- Específicas:
 - Projetos de aplicações ASP.NET, ASP.NET MVC, Windows Forms, WPF e Console: **Lib\Portable.Desktop.dll**
 - Projetos de aplicações Windows Phone 7 e 8: **Lib\Portable.Phone.dll**
 - Projetos de aplicações Windows 8 Store (Metro / Modern UI): **Lib\Portable.Store.dll**
- Gerais (todos os tipos de projeto devem incluir referência para estas DLLs):
 - **Lib\Portable.Runtime.dll**
 - **Lib\Portable.Security.Cryptography.dll**

###Usando PagCoin.NET###

Para usar o PagCoin.NET, crie uma instância do objeto ```Pagcoin``` informando sua API Key (encontrada no painel de administração do PagCoin, em https://pagcoin.com/Painel/Api) no construtor.

Para criar uma Ordem de Pagamento, instancie um objeto ```OrdemPagamento``` e preencha com os campos ```valorEmMoedaOriginal``` (obrigatório), ```nomeProduto``` (obrigatório), ```idInterna``` (opcional), ```email``` (opcional) e  ```redirectURL``` (opcional). Para uma descrição dos tipos e valores aceitáveis para estes campos, visite https://pagcoin.com/Home/Desenvolvedores. Com uma instância deste objeto, efetue uma chamada para o método ```CriarOrdemDePagamento``` de sua instância do objeto ```PagCoin```.

Para autenticar a chamada IPN, chame no início do seu método responsável pelo processamento da chamada o método ```ValidarAssinatura```, passando como parâmetro o objeto ```HttpWebRequest``` referente à requisição. Caso não seja possível, passar o ```HttpWebRequest```, extraia os campos de cabeçalho ```AssinaturaPagCoin``` e ```EnderecoPagCoin```, e informe-os junto ao conteudo recebido na requisição para o método  ```ValidarAssinatura```.
