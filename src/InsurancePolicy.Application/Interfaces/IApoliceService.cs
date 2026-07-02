using InsurancePolicy.Application.DTOs;

namespace InsurancePolicy.Application.Interfaces;

public interface IApoliceService
{
    Task<ApoliceResponse> CriarAsync(CriarApoliceRequest request, CancellationToken cancellationToken = default);
    Task<ApoliceResponse?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ApoliceResponse>> ListarTodasAsync(CancellationToken cancellationToken = default);
    Task<ApoliceResponse?> AtualizarAsync(Guid id, AtualizarApoliceRequest request, CancellationToken cancellationToken = default);
    Task<bool> RemoverAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ApoliceResponse>> ListarVencendoEm30DiasAsync(CancellationToken cancellationToken = default);
}
