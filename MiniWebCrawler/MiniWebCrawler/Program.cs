using HtmlAgilityPack;
using MiniWebCrawler.DAL;
using MiniWebCrawler.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MiniWebCrawler
{
    class Program
    {
        static void Main(string[] args)
        {

            //ReceitaContext context = new ReceitaContext();
            //context.Iniciar();
            startCrawlerasync();
            Console.ReadLine();
        }

        private static async Task startCrawlerasync()
        {
            var url = "https://www.tudogostoso.com.br/receitas";
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            var divs = htmlDocument.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "").Equals("mb-3 recipe-card recipe-card-with-hover")).ToList();

            foreach(var div in divs)
            {
                var category = div.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("category")).ToList();
                var cate = category[0].Descendants("span").FirstOrDefault().InnerText;
                cate = cate.Replace("\n", "");

                var owner = div.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("user")).ToList();
                var ow = owner[0].Descendants("span").FirstOrDefault().InnerText;
                ow = ow.Replace("\n", "");

               // var likes = div.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("favorites")).ToList();

                //var tempo = div.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("numbers")).ToList();
               // var tem = tempo[0].Descendants("div").FirstOrDefault().InnerText;

                var categoria = new Categoria
                {
                    Nome = cate
                };

                var autor = new Autor
                {
                    Nome = ow,
                };

                var receita = new Receita
                {
                    Nome = (div.Descendants("h3").FirstOrDefault().InnerText).Replace("\n", ""),
                };

                Console.WriteLine("\nReceita: " + receita.Nome + "\nAutor: " + autor.Nome + "\nCategoria: " + categoria.Nome);
            }
        }

        private Autor IdAuthor(string name)
        {
            Autor autor = new Autor();
            using (var context = new ReceitaContext())
            {
                foreach(var a in context.Autor)
                {
                    if (a.Nome.Equals(name))
                    {
                        autor = a;
                    }
                }
            }
            return autor;
        }

        private Categoria idCategoria(string name)
        {
            Categoria categoria = new Categoria();
            using (var context = new ReceitaContext())
            {
                foreach (var c in context.Categoria)
                {
                    if (c.Nome.Equals(name))
                    {
                        categoria = c;
                    }
                }
            }
            return categoria;
        }

        private void AdicionaReceita(Categoria category, Receita recipe, Autor author)
        {
            int categoryId = -1, authorId = -1;
            using (var context = new ReceitaContext())
            {
                var cat = context.Categoria;
                foreach (var c in cat)
                {
                    if (c.Nome.Equals(category.Nome))
                    {
                        categoryId = c.Id;
                        break;
                    }
                }
                var authors = context.Autor;
                foreach (var a in authors)
                {
                    if (a.Nome.Equals(author.Nome))
                    {
                        authorId = a.Id;
                        break;
                    }
                }
                if (categoryId == -1)
                {
                    context.Categoria.Add(category);
                }
                else
                {
                    recipe.Categoria = idCategoria(category.Nome);
                }
                if (authorId == -1)
                {
                    context.Autor.Add(author);
                }
                else
                {
                    recipe.Autor = IdAuthor(author.Nome);
                }

                context.Receita.Add(recipe);
                context.SaveChanges();
            }       
        }
    }
}
