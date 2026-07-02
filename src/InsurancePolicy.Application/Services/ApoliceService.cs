using System.Text.RegularExpressions;
using InsurancePolicy.Application.DTOs;
using InsurancePolicy.Application.Interfaces;
using InsurancePolicy.Application.Validators;
using InsurancePolicy.Domain;

namespace InsurancePolicy.Application.Services;

public partial class ApoliceService(IApoliceRepository repository) : IApoliceService
{
    private readonly IApoliceRepository _repository = repository;

    public async Task<ApoliceResponse> CriarAsync(CriarApoliceRequest request, CancellationToken cancellationToken = default)
    {
        ValidarDados(request.CpfCnpjSegurado, request.PlacaVeiculo, request.ValorPremio,
            request.DataInicioVigencia, request.DataFimVigencia);

        var apolice = new Apolice
        {
            Id = Guid.NewGuid(),
            NumeroApolice = await GerarNumeroApoliceAsync(cancellationToken),
            CpfCnpjSegurado = DocumentoSeguradoValidator.ValidarENormalizar(request.CpfCnpjSegurado),
            PlacaVeiculo = request.PlacaVeiculo.ToUpperInvariant(),
            ValorPremio = request.ValorPremio,
            DataInicioVigencia = request.DataInicioVigencia,
            DataFimVigencia = request.DataFimVigencia,
            Status = StatusApolice.Ativa
        };

        await _repository.AdicionarAsync(apolice, cancellationToken);

        return MapearParaResponse(apolice);
    }

    public async Task<ApoliceResponse?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var apolice = await _repository.ObterPorIdAsync(id, cancellationToken);
        return apolice is null ? null : MapearParaResponse(apolice);
    }

    public async Task<List<ApoliceResponse>> ListarTodasAsync(CancellationToken cancellationToken = default)
    {
        var apolices = await _repository.ListarTodasAsync(cancellationToken);
        return apolices.Select(MapearParaResponse).ToList();
    }

    public async Task<ApoliceResponse?> AtualizarAsync(Guid id, AtualizarApoliceRequest request, CancellationToken cancellationToken = default)
    {
        var apolice = await _repository.ObterPorIdAsync(id, cancellationToken);
        if (apolice is null)
            return null;

        ValidarDados(request.CpfCnpjSegurado, request.PlacaVeiculo, request.ValorPremio,
            request.DataInicioVigencia, request.DataFimVigencia);

        apolice.CpfCnpjSegurado = DocumentoSeguradoValidator.ValidarENormalizar(request.CpfCnpjSegurado);
        apolice.PlacaVeiculo = request.PlacaVeiculo.ToUpperInvariant();
        apolice.ValorPremio = request.ValorPremio;
        apolice.DataInicioVigencia = request.DataInicioVigencia;
        apolice.DataFimVigencia = request.DataFimVigencia;
        apolice.Status = request.Status;

        await _repository.AtualizarAsync(apolice, cancellationToken);

        return MapearParaResponse(apolice);
    }

    public Task<bool> RemoverAsync(Guid id, CancellationToken cancellationToken = default)
        => _repository.RemoverAsync(id, cancellationToken);

    public async Task<List<ApoliceResponse>> ListarVencendoEm30DiasAsync(CancellationToken cancellationToken = default)
    {
        var apolices = await _repository.ListarVencendoEm30DiasAsync(cancellationToken);
        return apolices.Select(MapearParaResponse).ToList();
    }

    private async Task<string> GerarNumeroApoliceAsync(CancellationToken cancellationToken)
    {
        var ano = DateTime.UtcNow.Year;
        var ultimaSequencia = await _repository.ObterUltimaSequenciaDoAnoAsync(ano, cancellationToken);
        var proximaSequencia = ultimaSequencia + 1;

        return $"SEG-{ano}-{proximaSequencia:D4}";
    }

    private static void ValidarDados(
        string cpfCnpj,
        string placa,
        decimal valorPremio,
        DateOnly dataInicio,
        DateOnly dataFim)
    {
        if (dataFim <= dataInicio)
            throw new ArgumentException("A data de término deve ser posterior à data de início.");

        if (valorPremio <= 0)
            throw new ArgumentException("O valor do prêmio deve ser maior que zero.");

        DocumentoSeguradoValidator.ValidarENormalizar(cpfCnpj);

        if (!PlacaValida().IsMatch(placa.ToUpperInvariant()))
            throw new ArgumentException("Placa inválida. Use o formato ABC1234 ou ABC1D23.");
    }

    private static ApoliceResponse MapearParaResponse(Apolice apolice) => new()
    {
        Id = apolice.Id,
        NumeroApolice = apolice.NumeroApolice,
        CpfCnpjSegurado = apolice.CpfCnpjSegurado,
        PlacaVeiculo = apolice.PlacaVeiculo,
        ValorPremio = apolice.ValorPremio,
        DataInicioVigencia = apolice.DataInicioVigencia,
        DataFimVigencia = apolice.DataFimVigencia,
        Status = apolice.Status
    };

    [GeneratedRegex(@"^[A-Z]{3}[0-9][A-Z0-9][0-9]{2}$")]
    private static partial Regex PlacaValida();
}
