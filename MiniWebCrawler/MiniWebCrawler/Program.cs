using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using MiniWebCrawler.DAL;
using MiniWebCrawler.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MiniWebCrawler
{
    class Program
    {
        static void Main(string[] args)
        {

            ReceitaContext context = new ReceitaContext();
            context.Iniciar();
            startCrawlerasync();
            Console.ReadLine();
        }

        /// <summary>
        /// inicia o WebCrawler coletando as receitas mais novas do site
        /// </summary>
        /// <returns></returns>
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


        /// <summary>
        /// realiza a busca de um autor no banco
        /// </summary>
        /// <param name="author">objeto tipo Autor que será comparado</param>
        /// <returns>true, para caso seja encontrado, se não, false</returns>
        private bool existeAutor(Autor author)
        {
            using (var context = new ReceitaContext())
            {
                var autores = context.Autor;
                foreach(var a in autores)
                {
                    if (author == a)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// método que retorna se a categoria já existe no banco
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        private bool existeCategoria(Categoria category)
        {
            using (var context = new ReceitaContext())
            {
                var categorias = context.Categoria;
                foreach(var c in categorias)
                {
                    if (c == category)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// busca uma categoria por meio de seu nome
        /// </summary>
        /// <param name="category">nome da categoria buscada</param>
        /// <returns>retorna um objeto categoria preenchido, se não for encontrado será null</returns>
        private Categoria buscaCategoria(string category)
        {
            using (var context = new ReceitaContext())
            {
                var categorias = context.Categoria;
                foreach(var c in categorias)
                {
                    if(category == c.Nome)
                    {
                        return c;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// busca o autor através do nome
        /// </summary>
        /// <param name="author">nome que será buscado na tabela de autores</param>
        /// <returns>retorna um objeto autor com os dados preenchidos no caso de ser encontrado, se não será null</returns>
        private Autor buscaAutor(string author)
        {
            using (var context = new ReceitaContext())
            {
                var autores = context.Autor;
                foreach (var a in autores)
                {
                    if (author == a.Nome)
                    {
                        return a;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// método que verifica se a receita já está cadastrada no banco
        /// </summary>
        /// <param name="recipe"></param>
        /// <returns>retorna true se a receita já existir, e false no caso contrário</returns>
        private bool existeReceita(Receita recipe)
        {
            using (var context = new ReceitaContext())
            {
                var receitas = context.Receita;
                foreach(var r in receitas)
                {
                    if(r == recipe)
                    {
                        return true;
                    }
                }      
            }
            return false;
        }

        /// <summary>
        /// método que adiciona uma receita ao banco de dados
        /// </summary>
        /// <param name="category">
        /// recebe o objeto categoria da receita obtida
        /// </param>
        /// <param name="recipe">
        /// recebe o objeto receita
        /// </param>
        /// <param name="author">
        /// recebe o objeto autor
        /// </param>
        private void AdicionaReceita(Categoria category, Receita recipe, Autor author)
        {
            using (var context = new ReceitaContext())
            {
                if (!existeCategoria(category))
                {
                    context.Categoria.Add(category);
                }
                category = buscaCategoria(category.Nome);
                if (!existeAutor(author))
                {
                    context.Autor.Add(author);
                }
                author = buscaAutor(author.Nome);
                recipe.Categoria = category;
                recipe.Autor = author;
                context.Receita.Add(recipe);
                context.SaveChanges();
            }       
        }

        /// <summary>
        /// lista todas as receitas cadastradas, suas respectivas categorias e seus autores
        /// </summary>
        private void listaReceitas()
        {
            using(var context = new ReceitaContext())
            {
                var receitas = context.Receita
                    .Include(receita => receita.Autor)
                    .Include(r => r.Categoria)
                    .ToList<Receita>();
                foreach(var r in receitas)
                {
                    Console.WriteLine(r);
                    Console.WriteLine("===============================================================");
                }
            }
        }


        /*private void listaReceitasAutor(string autor)
        {
            Autor author = buscaAutor(autor);
            using (var context = new ReceitaContext())
            {
                var autores = context.Autor
                    .Include(a => a.Receitas;
                foreach(var a in autores)
                {

                }
            }
        }*/
    }
}
