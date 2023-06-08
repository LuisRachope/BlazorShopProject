using BlazorShop.Models.DTOs;

namespace BlazorShop.Web.Services
{
    public interface IProdutoService
    {
        Task<IEnumerable<ProdutoDto>> GetAll();
        Task<ProdutoDto> GetItem(int id);
    }
}
