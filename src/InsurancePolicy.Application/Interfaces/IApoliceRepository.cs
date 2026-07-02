using InsurancePolicy.Domain;

namespace InsurancePolicy.Application.Interfaces;

public interface IApoliceRepository
{
    Task<Apolice?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Apolice>> ListarTodasAsync(CancellationToken cancellationToken = default);
    Task<List<Apolice>> ListarVencendoEm30DiasAsync(CancellationToken cancellationToken = default);
    Task<int> ObterUltimaSequenciaDoAnoAsync(int ano, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Apolice apolice, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Apolice apolice, CancellationToken cancellationToken = default);
    Task<bool> RemoverAsync(Guid id, CancellationToken cancellationToken = default);
}
