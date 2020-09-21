using System;
using System.Collections.Generic;
using System.Text;

namespace MiniWebCrawler.Models
{
    public class Receita
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        // public int Likes { get; set; }
        // public TimeSpan Tempo { get; set; }
        // public int Porcao { get; set; }
        public virtual Autor Autor { get; set; }
        public virtual Categoria Categoria { get; set; }

        public override string ToString()
        {
            String dados = $"Receita: {Nome}\nAutor: {Autor?.Nome}\nCategoria: {Categoria?.Nome}";
            return dados;
        }

        public string listaReceitaCategoria()
        {
            String dados = $"Receita: {Nome}\nCategoria: {Categoria?.Nome}";
            return dados;
        }
    }

    
}
