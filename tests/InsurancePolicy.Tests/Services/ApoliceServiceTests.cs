using InsurancePolicy.Application.DTOs;
using InsurancePolicy.Application.Interfaces;
using InsurancePolicy.Application.Services;
using InsurancePolicy.Domain;
using Moq;

namespace InsurancePolicy.Tests.Services;

public class ApoliceServiceTests
{
    [Fact(DisplayName = "Cria apólice válida com status Ativa")]
    public async Task CriarAsync_RetornaApoliceAtiva()
    {
        var (service, mockRepo) = CriarServiceComMock();

        var resultado = await service.CriarAsync(CriarRequestValido());

        Assert.Equal(StatusApolice.Ativa, resultado.Status);
        Assert.StartsWith("SEG-", resultado.NumeroApolice);
        Assert.Equal("BRA2E19", resultado.PlacaVeiculo);
        Assert.Equal("52998224725", resultado.CpfCnpjSegurado);
        mockRepo.Verify(r => r.AdicionarAsync(It.IsAny<Apolice>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "CPF com máscara é salvo só com dígitos")]
    public async Task CriarAsync_CpfComMascara_RemoveFormatacao()
    {
        var (service, _) = CriarServiceComMock();
        var request = CriarRequestValido();
        request.CpfCnpjSegurado = "529.982.247-25";

        var resultado = await service.CriarAsync(request);

        Assert.Equal("52998224725", resultado.CpfCnpjSegurado);
    }

    [Fact(DisplayName = "CNPJ com máscara é salvo só com dígitos")]
    public async Task CriarAsync_CnpjComMascara_RemoveFormatacao()
    {
        var (service, _) = CriarServiceComMock();
        var request = CriarRequestValido();
        request.CpfCnpjSegurado = "06.570.588/0001-03";

        var resultado = await service.CriarAsync(request);

        Assert.Equal("06570588000103", resultado.CpfCnpjSegurado);
    }

    [Fact(DisplayName = "CNPJ alfanumérico com máscara é aceito")]
    public async Task CriarAsync_CnpjAlfanumerico_AceitaFormato()
    {
        var (service, _) = CriarServiceComMock();
        var request = CriarRequestValido();
        request.CpfCnpjSegurado = "8H.9XJ.6M0/0001-73";

        var resultado = await service.CriarAsync(request);

        Assert.Equal("8H9XJ6M0000173", resultado.CpfCnpjSegurado);
    }

    [Fact(DisplayName = "Primeira apólice do ano recebe sequência 0001")]
    public async Task CriarAsync_GeraPrimeiroNumeroDoAno()
    {
        var (service, _) = CriarServiceComMock(ultimaSequencia: 0);
        var ano = DateTime.UtcNow.Year;

        var resultado = await service.CriarAsync(CriarRequestValido());

        Assert.Equal($"SEG-{ano}-0001", resultado.NumeroApolice);
    }

    [Fact(DisplayName = "Nova apólice incrementa a sequência do ano")]
    public async Task CriarAsync_IncrementaSequencia()
    {
        var (service, _) = CriarServiceComMock(ultimaSequencia: 2);
        var ano = DateTime.UtcNow.Year;

        var resultado = await service.CriarAsync(CriarRequestValido());

        Assert.Equal($"SEG-{ano}-0003", resultado.NumeroApolice);
    }

    [Fact(DisplayName = "Rejeita quando data fim é igual ou anterior à data início")]
    public async Task CriarAsync_RejeitaDataFimInvalida()
    {
        var (service, _) = CriarServiceComMock();
        var request = CriarRequestValido();
        request.DataFimVigencia = request.DataInicioVigencia;

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.CriarAsync(request));

        Assert.Contains("data de término", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact(DisplayName = "Rejeita prêmio zero ou negativo")]
    public async Task CriarAsync_RejeitaPremioInvalido()
    {
        var (service, _) = CriarServiceComMock();
        var request = CriarRequestValido();
        request.ValorPremio = 0;

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.CriarAsync(request));

        Assert.Contains("prêmio", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact(DisplayName = "Rejeita CPF com quantidade de dígitos inválida")]
    public async Task CriarAsync_RejeitaCpfInvalido()
    {
        var (service, _) = CriarServiceComMock();
        var request = CriarRequestValido();
        request.CpfCnpjSegurado = "123";

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.CriarAsync(request));

        Assert.Contains("CPF", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact(DisplayName = "Rejeita CNPJ com quantidade de caracteres inválida")]
    public async Task CriarAsync_RejeitaCnpjInvalido()
    {
        var (service, _) = CriarServiceComMock();
        var request = CriarRequestValido();
        request.CpfCnpjSegurado = "123456789012";

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.CriarAsync(request));

        Assert.Contains("CNPJ", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact(DisplayName = "Rejeita placa fora do padrão brasileiro")]
    public async Task CriarAsync_RejeitaPlacaInvalida()
    {
        var (service, _) = CriarServiceComMock();
        var request = CriarRequestValido();
        request.PlacaVeiculo = "ABC123";

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.CriarAsync(request));

        Assert.Contains("placa", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact(DisplayName = "Retorna apólice quando o id existe")]
    public async Task ObterPorIdAsync_EncontraApolice()
    {
        var id = Guid.NewGuid();
        var apolice = CriarApolice(id);
        var mockRepo = new Mock<IApoliceRepository>();
        mockRepo.Setup(r => r.ObterPorIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apolice);
        var service = new ApoliceService(mockRepo.Object);

        var resultado = await service.ObterPorIdAsync(id);

        Assert.NotNull(resultado);
        Assert.Equal(id, resultado.Id);
        Assert.Equal("SEG-2026-0001", resultado.NumeroApolice);
    }

    [Fact(DisplayName = "Retorna null quando o id não existe")]
    public async Task ObterPorIdAsync_RetornaNullSeNaoExistir()
    {
        var mockRepo = new Mock<IApoliceRepository>();
        mockRepo.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Apolice?)null);
        var service = new ApoliceService(mockRepo.Object);

        var resultado = await service.ObterPorIdAsync(Guid.NewGuid());

        Assert.Null(resultado);
    }

    [Fact(DisplayName = "Atualiza placa, prêmio e status da apólice")]
    public async Task AtualizarAsync_AtualizaCampos()
    {
        var id = Guid.NewGuid();
        var apolice = CriarApolice(id);
        var mockRepo = new Mock<IApoliceRepository>();
        mockRepo.Setup(r => r.ObterPorIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apolice);
        var service = new ApoliceService(mockRepo.Object);
        var request = new AtualizarApoliceRequest
        {
            CpfCnpjSegurado = "52998224725",
            PlacaVeiculo = "xyz9a87",
            ValorPremio = 400m,
            DataInicioVigencia = new DateOnly(2026, 1, 1),
            DataFimVigencia = new DateOnly(2026, 7, 20),
            Status = StatusApolice.Cancelada
        };

        var resultado = await service.AtualizarAsync(id, request);

        Assert.NotNull(resultado);
        Assert.Equal("XYZ9A87", resultado.PlacaVeiculo);
        Assert.Equal(400m, resultado.ValorPremio);
        Assert.Equal(StatusApolice.Cancelada, resultado.Status);
        mockRepo.Verify(r => r.AtualizarAsync(It.IsAny<Apolice>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Retorna null ao atualizar apólice inexistente")]
    public async Task AtualizarAsync_RetornaNullSeNaoExistir()
    {
        var mockRepo = new Mock<IApoliceRepository>();
        mockRepo.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Apolice?)null);
        var service = new ApoliceService(mockRepo.Object);

        var resultado = await service.AtualizarAsync(Guid.NewGuid(), CriarAtualizarRequestValido());

        Assert.Null(resultado);
    }

    [Fact(DisplayName = "Rejeita atualização com dados inválidos")]
    public async Task AtualizarAsync_RejeitaDadosInvalidos()
    {
        var id = Guid.NewGuid();
        var mockRepo = new Mock<IApoliceRepository>();
        mockRepo.Setup(r => r.ObterPorIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarApolice(id));
        var service = new ApoliceService(mockRepo.Object);
        var request = CriarAtualizarRequestValido();
        request.ValorPremio = -10;

        await Assert.ThrowsAsync<ArgumentException>(() => service.AtualizarAsync(id, request));
    }

    [Fact(DisplayName = "Remove apólice via repositório")]
    public async Task RemoverAsync_ChamaRepositorio()
    {
        var id = Guid.NewGuid();
        var mockRepo = new Mock<IApoliceRepository>();
        mockRepo.Setup(r => r.RemoverAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var service = new ApoliceService(mockRepo.Object);

        var resultado = await service.RemoverAsync(id);

        Assert.True(resultado);
        mockRepo.Verify(r => r.RemoverAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Lista todas as apólices mapeadas")]
    public async Task ListarTodasAsync_RetornaApolicesMapeadas()
    {
        var apolices = new List<Apolice> { CriarApolice(Guid.NewGuid()), CriarApolice(Guid.NewGuid()) };
        var mockRepo = new Mock<IApoliceRepository>();
        mockRepo.Setup(r => r.ListarTodasAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(apolices);
        var service = new ApoliceService(mockRepo.Object);

        var resultado = await service.ListarTodasAsync();

        Assert.Equal(2, resultado.Count);
        Assert.All(resultado, item => Assert.False(string.IsNullOrEmpty(item.NumeroApolice)));
    }

    [Fact(DisplayName = "Lista apólices vencendo nos próximos 30 dias")]
    public async Task ListarVencendoAsync_RetornaApolicesMapeadas()
    {
        var apolices = new List<Apolice> { CriarApolice(Guid.NewGuid()) };
        var mockRepo = new Mock<IApoliceRepository>();
        mockRepo.Setup(r => r.ListarVencendoEm30DiasAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(apolices);
        var service = new ApoliceService(mockRepo.Object);

        var resultado = await service.ListarVencendoEm30DiasAsync();

        Assert.Single(resultado);
        Assert.Equal(StatusApolice.Ativa, resultado[0].Status);
    }

    private static (ApoliceService service, Mock<IApoliceRepository> mock) CriarServiceComMock(int ultimaSequencia = 0)
    {
        var mockRepo = new Mock<IApoliceRepository>();
        mockRepo
            .Setup(r => r.ObterUltimaSequenciaDoAnoAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ultimaSequencia);

        return (new ApoliceService(mockRepo.Object), mockRepo);
    }

    private static CriarApoliceRequest CriarRequestValido() => new()
    {
        CpfCnpjSegurado = "52998224725",
        PlacaVeiculo = "BRA2E19",
        ValorPremio = 289.90m,
        DataInicioVigencia = new DateOnly(2026, 1, 1),
        DataFimVigencia = new DateOnly(2026, 7, 20)
    };

    private static AtualizarApoliceRequest CriarAtualizarRequestValido() => new()
    {
        CpfCnpjSegurado = "52998224725",
        PlacaVeiculo = "BRA2E19",
        ValorPremio = 289.90m,
        DataInicioVigencia = new DateOnly(2026, 1, 1),
        DataFimVigencia = new DateOnly(2026, 7, 20),
        Status = StatusApolice.Ativa
    };

    private static Apolice CriarApolice(Guid id) => new()
    {
        Id = id,
        NumeroApolice = "SEG-2026-0001",
        CpfCnpjSegurado = "52998224725",
        PlacaVeiculo = "BRA2E19",
        ValorPremio = 289.90m,
        DataInicioVigencia = new DateOnly(2026, 1, 1),
        DataFimVigencia = new DateOnly(2026, 7, 20),
        Status = StatusApolice.Ativa
    };
}
