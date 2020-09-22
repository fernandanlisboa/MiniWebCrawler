using System;
using System.Collections.Generic;
using System.Text;

namespace MiniWebCrawler.Models
{
    public class Receita
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int Likes { get; set; }
        public string Tempo { get; set; }
        public int Porcao { get; set; }
        public int AutorId { get; set; }
        public int CategoriaId { get; set; }
        public virtual Autor Autor { get; set; }
        public virtual Categoria Categoria { get; set; }

        public override string ToString()
        {
            String dados = $"Nome da receita: {Nome}\nLikes: {Likes}\nTempo de preparo: {Tempo}\n"
                + $"Quantidade de Porções: {Porcao}\n"
                + $"Autor: {Autor?.Nome}\nCategoria: {Categoria?.Nome}";
            return dados;
        }

        public string listaReceitaCategoria()
        {
            String dados = $"Nome da receita: {Nome}\nLikes: {Likes}\nTempo de preparo: {Tempo}\n"
                + $"Quantidade de Porções: {Porcao}\n"
                + $"Categoria: {Categoria?.Nome}";
            return dados;
        }
    }

    
}
