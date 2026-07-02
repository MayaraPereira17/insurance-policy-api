using InsurancePolicy.Domain;

namespace InsurancePolicy.Application.DTOs;

public class AtualizarApoliceRequest
{
    public string CpfCnpjSegurado { get; set; } = string.Empty;
    public string PlacaVeiculo { get; set; } = string.Empty;
    public decimal ValorPremio { get; set; }
    public DateOnly DataInicioVigencia { get; set; }
    public DateOnly DataFimVigencia { get; set; }
    public StatusApolice Status { get; set; }
}
