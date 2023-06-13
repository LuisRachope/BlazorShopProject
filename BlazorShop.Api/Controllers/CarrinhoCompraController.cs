﻿using BlazorShop.Api.Mappings;
using BlazorShop.Api.Repositories;
using BlazorShop.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BlazorShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarrinhoCompraController : ControllerBase
    {
        private readonly ICarrinhoCompraRepository _carrinhoCompraRepository;
        private readonly IProdutoRepository _produtoRepository;

        private ILogger<CarrinhoCompraController> _logger;

        public CarrinhoCompraController(ICarrinhoCompraRepository
            carrinhoCompraRepository,
            IProdutoRepository produtoRepository,
            ILogger<CarrinhoCompraController> logger)
        {
            _carrinhoCompraRepository = carrinhoCompraRepository;
            _produtoRepository = produtoRepository;
            _logger = logger;
        }

        [HttpGet]
        [Route("{usuarioId}/GetItens")]
        public async Task<ActionResult<IEnumerable<CarrinhoItemDto>>> GetItens(string usuarioId)
        {
            try
            {
                var carrinhoItens = await _carrinhoCompraRepository.GetItens(usuarioId);
                if (carrinhoItens == null)
                {
                    return NoContent(); // 204 Status Code
                }

                var produtos = await _produtoRepository.GetItens();
                if (produtos == null)
                {
                    throw new Exception("Não existem produtos...");
                }

                var carrinhoItensDto = carrinhoItens.ConverterCarrinhoItensParaDto(produtos);
                return Ok(carrinhoItensDto);
            }
            catch (Exception err)
            {
                _logger.LogError("## Erro ao obter itens do carrinho");
                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<IEnumerable<CarrinhoItemDto>>> GetItem(int id)
        {
            try
            {
                var carrinhoItem = await _carrinhoCompraRepository.GetItem(id);
                if (carrinhoItem is null)
                {
                    return NotFound($"Item não encontrado"); //404 status code
                }

                var produto = await _produtoRepository.GetItem(carrinhoItem.ProdutoId);

                if (produto is null)
                {
                    return NotFound($"Item não existe na fonte de dados");
                }
                var cartItemDto = carrinhoItem.ConverterCarrinhoItemParaDto(produto);

                return Ok(cartItemDto);
            }
            catch (Exception err)
            {
                _logger.LogError($"## Erro ao obter o item ={id} do carrinho");
                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }


        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<CarrinhoItemDto>>> PostItem([FromBody] CarrinhoItemAdicionaDto carrinhoItemAdicionaDto)
        {
            try
            {
                var novoCarrinhoItem = await _carrinhoCompraRepository.AdicionaItem(carrinhoItemAdicionaDto);

                if (novoCarrinhoItem is null)
                {
                    return NoContent(); //Status 204
                }

                var produto = await _produtoRepository.GetItem(novoCarrinhoItem.ProdutoId);

                if (produto is null)
                {
                    throw new Exception($"Produto não localizado (Id:{carrinhoItemAdicionaDto.ProdutoId})");
                }

                var novoCarrinhoItemDto = novoCarrinhoItem.ConverterCarrinhoItemParaDto(produto);
                return CreatedAtAction(nameof(GetItem), new { id = novoCarrinhoItemDto.Id}, novoCarrinhoItemDto);
            }
            catch (Exception err)
            {
                _logger.LogError($"## Erro criar um novo item no carrinho");
                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<CarrinhoItemDto>> DeleteItem(int id)
        {
            try
            {
                var carrinhoItem = await _carrinhoCompraRepository.DeletaItem(id);

                if (carrinhoItem is null) return NotFound();

                var produto = await _produtoRepository.GetItem(carrinhoItem.ProdutoId);

                if (produto is null) return NotFound();

                var carrinhoItemDto = carrinhoItem.ConverterCarrinhoItemParaDto(produto);
                return Ok(carrinhoItemDto);

            }
            catch (Exception err)
            {
                _logger.LogError($"## Erro ao tentar deletar o item.");
                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

    }
}
