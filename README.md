pagcoin.net
===========

.NET library to integrate with PagCoin

PagCoin.NET é uma biblioteca PCL (Portable Class Library), compatível com .NET Framework 4.5, além de poder ser usada também no desenvolvimento de aplicações Windows Store e Windows Phone 7 e 8.

O projeto está configurado para o Visual Studio 2012. Basta baixar o código, buildar em modo Release e usar a DLL resultante. Adicione referências também para as DLLs Lib\Portable, de acordo com sua necessidade:

- Específicas:
 - Projetos de aplicações ASP.NET, ASP.NET MVC, Windows Forms, WPF e Console: **Lib\Portable.Desktop.dll**
 - Projetos de aplicações Windows Phone 7 e 8: **Lib\Portable.Phone.dll**
 - Projetos de aplicações Windows 8 Store (Metro / Modern UI): **Lib\Portable.Store.dll**
- Gerais (todos os tipos de projeto devem incluir referência para estas DLLs):
 - **Lib\Portable.Runtime.dll**
 - **Lib\Portable.Security.Cryptography.dll**
