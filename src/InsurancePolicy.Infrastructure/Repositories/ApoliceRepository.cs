using InsurancePolicy.Application.Interfaces;
using InsurancePolicy.Domain;
using InsurancePolicy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InsurancePolicy.Infrastructure.Repositories;

public class ApoliceRepository(AppDbContext context) : IApoliceRepository
{
    private readonly AppDbContext _context = context;

    public async Task<Apolice?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Apolices
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<List<Apolice>> ListarTodasAsync(CancellationToken cancellationToken = default)
        => await _context.Apolices
            .AsNoTracking()
            .OrderByDescending(a => a.DataInicioVigencia)
            .ToListAsync(cancellationToken);

    public async Task<List<Apolice>> ListarVencendoEm30DiasAsync(CancellationToken cancellationToken = default)
    {
        //  apólices ativas que vencem nos próximos 30 dias
        return await _context.Apolices
            .FromSqlRaw("""
                SELECT *
                FROM Apolices
                WHERE Status = {0}
                  AND DataFimVigencia >= date('now')
                  AND DataFimVigencia <= date('now', '+30 days')
                ORDER BY DataFimVigencia
                """, (int)StatusApolice.Ativa)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<int> ObterUltimaSequenciaDoAnoAsync(int ano, CancellationToken cancellationToken = default)
    {
        var prefixo = $"SEG-{ano}-";

        var numeros = await _context.Apolices
            .Where(a => a.NumeroApolice.StartsWith(prefixo))
            .Select(a => a.NumeroApolice)
            .ToListAsync(cancellationToken);

        if (numeros.Count == 0)
            return 0;

        return numeros
            .Select(numero => int.Parse(numero.Split('-')[2]))
            .Max();
    }

    public async Task AdicionarAsync(Apolice apolice, CancellationToken cancellationToken = default)
    {
        _context.Apolices.Add(apolice);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(Apolice apolice, CancellationToken cancellationToken = default)
    {
        _context.Apolices.Update(apolice);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var apolice = await _context.Apolices.FindAsync([id], cancellationToken);
        if (apolice is null)
            return false;

        _context.Apolices.Remove(apolice);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
