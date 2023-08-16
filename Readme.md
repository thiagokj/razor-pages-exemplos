# Razor Pages | Visão Geral

Seguindo o curso do [balta.io 2820](https://github.com/balta-io/2820)

## Razor

O Razor é um View Engine (Motor de visualização do ASP.NET). Permite criar o FrontEnd da aplicação, mesclando Csharp com Html. Para criar uma aplicação do tipo ASP.NET Razor Pages, inicie um novo projeto com:

```csharp
dotnet new web -o RazorApp
```

Obs: Para facilitar, crie um novo projeto vazio ASP.NET Core no Visual Studio.

Esse padrão é mais simples que o padrão MVC, que possui uma arquitetura mais complexa.

Para iniciar as configurações:

```csharp
var builder = WebApplication.CreateBuilder(args);
// Adiciona suporte a Razor Pages
builder.Services.AddRazorPages();

var app = builder.Build();

// Redireciona para conexão segura
app.UseHttpsRedirection();
// Permite servir conteúdos estáticos da pasta padrão wwwroot. Ex: Imagens, paginas, etc.
app.UseStaticFiles();

// Mapeamento das paginas com URLs personalizadas ou automatizar a busca das paginas
app.UseRouting();
app.MapRazorPages();

app.Run();
```

## Criando uma pagina

Crie as pastas wwwroot e Pages na raiz do projeto. Agora crie uma pagina Razor dentro de Pages

Serão criados 2 arquivos com a estrutura:

```csharp
// Index.cshtml | Arquivo com sintaxe HTML e C#
@page // Indica que é uma pagina Razor
@model RazorApp.Pages.IndexModel // Junta os arquivos cshtml, apontando para classe Index
@{
}

// Index.cshtml.cs | Arquivo complementar códigos C#
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorApp.Pages
{
    // É uma classe que herda de PageModel
    public class IndexModel : PageModel
    {
        // Sempre que a pagina for renderizada (obter a informação)
        public void OnGet()
        {
        }

        // Sempre que enviar informações da pagina
        public void OnPost()
        {
        }
    }
}

// Aqui um exemplo interpolando codigo Csharp na Index
// Sempre utilizar @ e em seguida o comando C#
<!DOCTYPE html>

<html>
<head>
</head>
<body>
<div>
    @for (var i = 0; i <= 10; i++)
        {
            <p>Item @i</p>
        }
</div>
</body>
</html>
```

## Criando uma lista de categorias

O Pages é bem mais simples que o MVC. Podemos criar um record para representar uma categoria.

```csharp
namespace RazorApp.Pages
{
    public class IndexModel : PageModel
    {
        // Inicializa a lista de categorias
        public List<Category> Categories { get; set; } = new();

        // Ao exibir a pagina, Adiciona 100 itens a lista
        public async Task OnGetAsync()
        {
            await Task.Delay(500);
            for (int i = 0; i <= 100; i++)
            {
                Categories.Add(new Category(i, $"Categoria {i}", i * 17.54M));
            }
        }
    }

    // Tipo de objeto categoria. Não há comportamentos a serem alterados, apenas estrutura basica.
    public record Category(
        int Id,
        string Title,
        decimal Price);
}
```

Agora no pagina que será exibida ao usuario, podemos utilizar a variavel especial @Model.
Ela faz referencia aos modelos contidos no Page Model da pagina Index.cshtml.cs.
Então podemos iterar as caterogias usando Model.Categories em um Loop e usar o @category.X, onde X 
permite o acesso a cada propriedade.

```csharp
<html>
<head>
</head>
<body>
    <div>
        <table>
            <thead>
                <tr>
                    <td>ID</td>
                    <td>Title</td>
                    <td>Price</td>
                </tr>
            </thead>
            <tbody>
                @foreach (var category in Model.Categories)
                {
                    <tr>
                        <td>@category.Id</td>
                        <td>@category.Title</td>
                        <td>@category.Price</td>
                    </tr>
                }
            </tbody>
        </table>
    </div>
</body>
</html>
```

## Navegação de Páginas

O Razor sempre busca as paginas renderizadas na raiz da aplicação em wwwroot.
O ~ representa a raiz da estrutura de arquivos

```html
<ul>
    <li>
        <a href="~/">Home</a>
    </li>
    <li>
        <a href="~/About">Sobre</a>
    </li>
    <li>
        <a href="~/Login">Login</a>
    </li>
</ul>
```

Para reutilizar esse codigo, vamos compartilhar o mesmo em uma Partial e referenciar usando
TagHelpers (<asp-net>) do ASP.NET

Adicione ao topo do arquivo a instrução *@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers*

E agora no codigo, utilize a tag *partial name*:

```csharp
<body>
  <div>
    <partial name="Shared/NavMenuPartial" />
    <h1>Categorias</h1>
  </div>
</body>
```

## View Imports

Esse é um arquivo especial, para compartilhar as importações com todas as Razor pages.

```csharp
@using RazorApp
@namespace RazorApp.Pages
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers

@using System.Globalization
```

## Layouts

Crie um arquivo chamado de _Layouts na pasta Shared. Nesse arquivo, adicione a estrutura do HTML
e use @RenderBody, para mesclar as paginas que utilizarão o layout.

```csharp
<!DOCTYPE html>

<html>
<head>
  <title>Razor App</title>
</head>
<body>
  <header>
    <partial name="Shared/NavMenuPartial" />
  </header>
  <main>
    @RenderBody()
  </main>
</body>
</html>
```

A pagina index então, pode ser simplificada dessa maneira:

```csharp
@page
@model IndexModel
@{
  Layout = "Shared/_Layout";
}

<h1>Categorias</h1>
<div>
  <table>
    <thead>
      <tr>
        <td>ID</td>
        <td>Title</td>
        <td>Price</td>
      </tr>
    </thead>
    <tbody>
      @foreach (var category in Model.Categories)
      {
        <tr>
          <td>@category.Id</td>
          <td>@category.Title</td>
          <td>@category.Price.ToString("C", new CultureInfo("pt-BR"))</td>
        </tr>
      }
    </tbody>
  </table>
</div>
```

## ViewStart

Criando uma pagina _ViewStart.cshtml, informamos que o padrão a ser carregado 
por todas as paginas. Podemos então definir o layout padrão, omitindo a declaração
individual em cada pagina

```csharp
@{
  Layout = "Shared/_Layout";
}
```

## Rotas e Hiperlinks

Declare qual é a pagina a ser renderizada utilizando o caminho absuloto com @page "~/nomeDaPagina".
Use tambem o TagHelper asp-page para mapeamento dinamico das paginas, chamando apenas o nome.

Exemplo do Menu
```csharp
<ul>
  <li>
    <a asp-page="Home">Home</a>
  </li>
  <li>
    <a asp-page="About">Sobre</a>
  </li>
  <li>
    <a asp-page="Login">Login</a>
  </li>
</ul>
```

Exemplo da pagina About, alterando o nome de exibição para Sobre

```csharp
@page "~/Sobre"
```

## ViewData

Usamos o ViewData para passar informações através das páginas.

Por exemplo, definir o titulo na aba do navegador.

```csharp
// Arquivo _Layout
<head>
  <title>@ViewData["Title"] - Razor App</title>
</head>

// Adicionando o ViewData a pagina Sobre
@page "~/sobre"
@model RazorApp.Pages.AboutModel
@{
  ViewData["Title"] = "Sobre";
}
```

## Paginação

Para paginar os registros, podemos informar 2 parametros skip e take diretamente na rota

```csharp
// a interrogação torna os parametros opcionais, tornando mais simples a primeira chamada a pagina
@page "~/categorias/{skip?}/{take?}"

// Temos então a modificação no metodo para trazer a consulta
 public class CategoriesModel : PageModel
    {
    public List<Category> Categories { get; set; } = new();

    // Parametros com valores padrão
    public async Task OnGetAsync(int skip = 0, int take = 25)
    {
      // lista temporaria apenas para testes
      var categoriesList = new List<Category>();

      await Task.Delay(100);
      for (int i = 0; i <= 1000; i++)
      {
        categoriesList.Add(new Category(i, $"Categoria {i}", i * 17.54M));
      }

      // Query com os itens da lista sendo paginados
      Categories = categoriesList
        .Skip(skip)
        .Take(take)
        .ToList();
    }
  }
```

## Append Version

O Tag Helper asp-append-version cria um hash para evitar cache para itens estáticos como CSS e JS.

```csharp
<link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
```

Dessa forma, a cada deploy da aplicação, é gerado um novo hash, forçando o carregamento
dos arquivos estáticos com as versões mais recentes.

## Nested CSS

Podemos criar um CSS individual para cada pagina, criando o arquivo com a extensão
*pagina.cshtml.css*

Isso ajuda muito na estilização especifica, evitando carregar folhas de estilos não usadas
em todas as paginas.

Para que esse arquivo seja gerado na renderização, deve ser especificado o nome especial, usando 
o nome da aplicação. Ex: NomeDaAplicação.styles.css

```csharp
<link rel="stylesheet" href="~/RazorApp.styles.css" asp-append-version="true" />
```

## Render Section

Para executar scripts como javascript somente no final da pagina, podemos utilizar o RenderSection.

Seria como uma div do HTML. Assim garantimos a execução do script na renderização junto com Razor.

Exemplo:

```csharp
// Habilitando no _Layout.cshtml
  <footer>
    @RenderSection(name:"scripts", required:false)
  </footer>

// Chamando o script na pagina Sobre
<h1>Sobre</h1>

@section scripts{
  <script src="~/js/site.js"></script>
}
```