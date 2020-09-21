using Microsoft.EntityFrameworkCore;
using MiniWebCrawler.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniWebCrawler.DAL
{
    public class ReceitaContext : DbContext
    {
        public DbSet<Receita> Receita { get; set;  }
        public DbSet<Autor> Autor { get; set; }
        public DbSet<Categoria> Categoria { get; set; }

        public void Iniciar()
        {
            this.Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=localhost;database=receitadb;user=root;password=");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Autor>().ToTable("Autor");
            modelBuilder.Entity<Categoria>().ToTable("Categoria");
            modelBuilder.Entity<Receita>().ToTable("Receita");
        }
    }
}
