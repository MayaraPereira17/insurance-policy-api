namespace InsurancePolicy.Application.DTOs;

public class CriarApoliceRequest
{
    public string CpfCnpjSegurado { get; set; } = string.Empty;
    public string PlacaVeiculo { get; set; } = string.Empty;
    public decimal ValorPremio { get; set; }
    public DateOnly DataInicioVigencia { get; set; }
    public DateOnly DataFimVigencia { get; set; }
}
