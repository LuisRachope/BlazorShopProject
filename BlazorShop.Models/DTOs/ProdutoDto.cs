using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorShop.Models.DTOs
{
    public class ProdutoDto
    {
        public int Id { get; set; }
        public string? Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; } = string.Empty;
        public string? ImagemUrl { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }
        public int CategoriaId { get; set; }
        public string? CategoriaNome { get; set; }
    }
}
