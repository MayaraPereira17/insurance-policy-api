using InsurancePolicy.Application.DTOs;
using InsurancePolicy.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InsurancePolicy.Api.Controllers;

[ApiController]
[Route("api/apolices")]
public class ApolicesController(IApoliceService apoliceService) : ControllerBase
{
    private readonly IApoliceService _apoliceService = apoliceService;

    [HttpPost]
    public async Task<ActionResult<ApoliceResponse>> Criar(
        [FromBody] CriarApoliceRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var apolice = await _apoliceService.CriarAsync(request, cancellationToken);
            return CreatedAtAction(nameof(ObterPorId), new { id = apolice.Id }, apolice);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<ApoliceResponse>>> Listar(CancellationToken cancellationToken)
        => await _apoliceService.ListarTodasAsync(cancellationToken);

    [HttpGet("vencendo")]
    public async Task<ActionResult<List<ApoliceResponse>>> ListarVencendo(CancellationToken cancellationToken)
        => await _apoliceService.ListarVencendoEm30DiasAsync(cancellationToken);

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApoliceResponse>> ObterPorId(Guid id, CancellationToken cancellationToken)
    {
        var apolice = await _apoliceService.ObterPorIdAsync(id, cancellationToken);
        return apolice is null ? NotFound() : apolice;
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApoliceResponse>> Atualizar(
        Guid id,
        [FromBody] AtualizarApoliceRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var apolice = await _apoliceService.AtualizarAsync(id, request, cancellationToken);
            return apolice is null ? NotFound() : apolice;
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid id, CancellationToken cancellationToken)
    {
        var removido = await _apoliceService.RemoverAsync(id, cancellationToken);
        return removido ? NoContent() : NotFound();
    }
}
