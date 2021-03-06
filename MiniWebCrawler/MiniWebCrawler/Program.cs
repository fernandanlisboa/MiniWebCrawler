﻿using HtmlAgilityPack;
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
            printInicio();
            startCrawlerasync();
            //Console.ReadLine();
            listaReceitas();
            buscarAutor();
            buscarCategoria();
            Console.ReadLine();
        }

        private static void printInicio()
        {
            Console.WriteLine($"\tBem vindo(a) ao WebReceitas!!!\n" +
                "\n\n\nAqui você encontrará as receitas mais novas adicionadas ao site do Tudo Gostoso!" +
                "\nEspero que se delicie também!!!");
            Console.WriteLine("\n\n\n\tIniciando Crawler...");
        }

        private static void buscarAutor()
        {
            Console.WriteLine("\nDigite o nome do autor desejado: ");
            var a = Console.ReadLine();
            if (existeAutor(a))
            {
                Autor autor = buscaAutor(a);
                listaReceitasAutor(autor);
            }
            else
            {
                Console.WriteLine("Esse autor não foi encontrado!");
            }
        }

        private static void buscarCategoria()
        {
            Console.WriteLine("\nDigite o nome da categoria desejada: ");
            var c = Console.ReadLine();
            if (existeCategoria(c))
            {
                Categoria categoria = buscaCategoria(c);
                listaReceitaCategoria(categoria);
            }
            else
            {
                Console.WriteLine("Essa categoria não foi encontrada!");
            }
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

                var likes = div.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("favorites")).ToList();
                
                var tempo = div.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("time")).ToList();
                var tem = tempo[0].InnerText.Replace("\n", "");

                var portion = div.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("portion")).ToList();
                string porcao = portion[0].InnerText.Replace("\n", "");
                string[] por = porcao.Split(" ");
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
                    Tempo = tem,
                    Porcao = Convert.ToInt32(por[0]),
                    Likes = Convert.ToInt32(likes[0].InnerText.Replace("\n", "")),
            };

                AdicionaReceita(categoria, receita, autor);
                //Console.WriteLine("\nReceita: " + receita.Nome + "\nAutor: " + autor.Nome + "\nCategoria: " + categoria.Nome);
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
        private static bool existeAutor(string author)
        {
            using (var context = new ReceitaContext())
            {
                context.Database.EnsureCreated();
                Autor autor = new Autor();
                try
                {
                    autor = context.Autor.Where<Autor>(a => a.Nome == author).FirstOrDefault();
                }
                catch
                {
                    Console.WriteLine("Algo deu errado! :/");
                }
                if(autor != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// método que retorna se a categoria já existe no banco
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        private static bool existeCategoria(string category)
        {
            using (var context = new ReceitaContext())
            {
                context.Database.EnsureCreated();
                Categoria categoria = new Categoria();
                try
                {
                    categoria = context.Categoria.Where<Categoria>(categoria => categoria.Nome == category).FirstOrDefault();
                }
                catch
                {
                    Console.WriteLine("Algo deu errado! :/");
                }
                if(categoria != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// busca uma categoria por meio de seu nome
        /// </summary>
        /// <param name="category">nome da categoria buscada</param>
        /// <returns>retorna um objeto categoria preenchido, se não for encontrado será null</returns>
        private static Categoria buscaCategoria(string category)
        {
            using (var context = new ReceitaContext())
            {
                context.Database.EnsureCreated();
                Categoria categoria = new Categoria();
                try
                {
                    categoria = context.Categoria.Where<Categoria>(categoria => categoria.Nome == category).FirstOrDefault();
                    return categoria;
                }
                catch
                {
                    Console.WriteLine("Erro ao buscar categoria!");
                    return null;
                }
            }
        }

        /// <summary>
        /// busca o autor através do nome
        /// </summary>
        /// <param name="author">nome que será buscado na tabela de autores</param>
        /// <returns>retorna um objeto autor com os dados preenchidos no caso de ser encontrado, se não será null</returns>
        private static Autor buscaAutor(string author)
        {
            using (var context = new ReceitaContext())
            {
                context.Database.EnsureCreated();
                Autor autor = new Autor();
                try
                {
                    autor = context.Autor.Where<Autor>(autor => autor.Nome == author).FirstOrDefault();
                    return autor;
                }
                catch
                {
                    Console.WriteLine("Erro ao encontrar o autor!");
                    return null;
                }
            }            
        }

        /// <summary>
        /// método que verifica se a receita já está cadastrada no banco
        /// </summary>
        /// <param name="recipe"></param>
        /// <returns>retorna true se a receita já existir, e false no caso contrário</returns>
        private static int existeReceita(Receita recipe, string author)
        {
            using (var context = new ReceitaContext())
            {
                context.Database.EnsureCreated();
                Receita receita = new Receita();
                try
                {
                    receita = context.Receita
                        .Where<Receita>(r => r.Nome == recipe.Nome && r.Tempo == recipe.Tempo && r.Porcao == recipe.Porcao && r.Autor.Nome == author).FirstOrDefault();
                }
                catch
                {
                    Console.WriteLine("Algo deu errado! :/");
                }
                if(receita != null)
                {
                    return receita.Id;
                }
            }
            return -1;
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
        private static void AdicionaReceita(Categoria category, Receita recipe, Autor author)
        {
            
            using (var context = new ReceitaContext())
            {
                context.Database.EnsureCreated();
                int id = existeReceita(recipe, author.Nome);
                if (id != -1)
                {
                    try
                    {
                        var updateRecipe = context.Receita.Where<Receita>(r => r.Id == id).FirstOrDefault();
                        updateRecipe.Likes = recipe.Likes;
                        context.SaveChanges();
                    }
                    catch
                    {
                        Console.WriteLine("Erro ao atualizar receita!");
                    }
                }
                else
                {
                    try
                    {
                        if (existeCategoria(category.Nome))
                        {
                            category = buscaCategoria(category.Nome);
                            recipe.CategoriaId = category.Id;
                        }
                        else
                        {
                            recipe.Categoria = category;
                        }

                        if (existeAutor(author.Nome))
                        {
                            author = buscaAutor(author.Nome);
                            recipe.AutorId = author.Id;
                        }
                        else
                        {
                            recipe.Autor = author;
                        }

                        context.Receita.Add(recipe);
                        context.SaveChanges();
                    }
                    catch
                    {
                        Console.WriteLine("Erro ao adicionar a receita ao banco!");
                    }
                }                
            }       
        }

        /// <summary>
        /// lista todas as receitas cadastradas, suas respectivas categorias e seus autores
        /// </summary>
        private static void listaReceitas()
        {
            using(var context = new ReceitaContext())
            {
                context.Database.EnsureCreated();
                try
                {
                    var receitas = context.Receita
                    .Include(receita => receita.Autor)
                    .Include(r => r.Categoria)
                    .ToList<Receita>();
                    Console.WriteLine("Todas as receitas cadastradas!");
                    foreach (var r in receitas)
                    {
                        Console.WriteLine(r);
                        Console.WriteLine("===============================================================");
                    }
                }
                catch
                {
                    Console.WriteLine("Erro ao apresentar todas as receitas cadastradas no banco!");
                }
                Console.ReadLine();
            }
        }


        private static void listaReceitasAutor(Autor autor)
        {
            using (var context = new ReceitaContext())
            {
                context.Database.EnsureCreated();
                try
                {
                    var receitas = context.Receita.Where(r => r.Autor == autor).Include(r => r.Categoria)
                    .ToList<Receita>();
                    foreach(var receita in receitas)
                    {
                        Console.WriteLine(receita.receitaAutor());
                        Console.WriteLine("===================================================================");
                    }
                }
                catch
                {
                    Console.WriteLine("Ocorreu algum erro!\n");
                }
                
            }
        }

        private static void listaReceitaCategoria(Categoria categoria)
        {
            using (var context = new ReceitaContext())
            {
                context.Database.EnsureCreated();
                try
                {
                    var receitas = context.Receita.Where(r => r.Categoria == categoria).Include(r => r.Autor)
                    .ToList<Receita>();
                    foreach (var receita in receitas)
                    {
                        Console.WriteLine(receita.receitaCategoria());
                        Console.WriteLine("\n================================================================\n");
                    }
                }
                catch
                {
                    Console.WriteLine("Ocorreu algum erro!\n");
                }

            }
        }
    }
}
