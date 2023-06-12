﻿using BlazorShop.Models.DTOs;
using System.Net;
using System.Net.Http.Json;

namespace BlazorShop.Web.Services
{
    public class CarrinhoCompraService : ICarrinhoCompraService
    {
        public HttpClient _httpClient;
        public ILogger<ProdutoService> _logger;

        public CarrinhoCompraService(HttpClient httpClient, ILogger<ProdutoService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<CarrinhoItemDto> AdicionaItem(CarrinhoItemAdicionaDto carrinhoItemAdicionaDto)
        {
            try
            {
                var response = await _httpClient
                                .PostAsJsonAsync<CarrinhoItemAdicionaDto>("api/CarrinhoCompra", 
                                carrinhoItemAdicionaDto);

                if (response.IsSuccessStatusCode)// Status code entre 200 a 299
                {
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        // retorna o valor "padrão" ou vazio para um objeto do tipo carrinhoItemDto
                        return default(CarrinhoItemDto);
                    }
                    // Lê o conteúdo HTTP e retorna o valor resultante
                    // da serialização do conteúdo JSON para o objeto Dto
                    return await response.Content.ReadFromJsonAsync<CarrinhoItemDto>();
                }
                else
                {
                    // serializa o conteúdo HTTP como uma string
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"{response.StatusCode} Mensagem: {message}");
                }
            }
            catch (Exception err)
            {
                _logger.LogError($"Erro ao acessar carrinhos : api/carrinhoCompra. Mensagem: {err}");
                throw;
            }
        }

        public async Task<List<CarrinhoItemDto>> GetItens(string usuarioId)
        {
            try
            {
                //envia um request GET para a uri da API CarrinhoCompra
                var response = await _httpClient.GetAsync($"api/CarrinhoCompra/{usuarioId}/GetItens");

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return Enumerable.Empty<CarrinhoItemDto>().ToList();
                    }
                    return await response.Content.ReadFromJsonAsync<List<CarrinhoItemDto>>();
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Http Status Code: {response.StatusCode} Mensagem: {message}");
                }

            }
            catch (Exception err)
            {
                _logger.LogError($"Erro ao acessar carrinhos : api/carrinhoCompra. Mensagem: {err}");
                throw;
            }
        }
    }
}