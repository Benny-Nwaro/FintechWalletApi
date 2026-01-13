using FintechWalletApi.Data;
using FintechWalletApi.DTOs;
using FintechWalletApi.Common;

namespace FintechWalletApi.Services;

public class TransactionService : ITransactionService
{
    private readonly AppDbContext _context;

    public TransactionService(AppDbContext context)
    {
        _context = context;
    }

public PagedResult<TransactionDto> GetWalletTransactions(
        Guid walletId,
        TransactionQueryParams query)
    {
        var transactionsQuery = _context.Transactions
            .Where(t => t.WalletId == walletId);

        if (!string.IsNullOrEmpty(query.Type))
            transactionsQuery = transactionsQuery
                .Where(t => t.Type == query.Type);

        if (query.FromDate.HasValue)
            transactionsQuery = transactionsQuery
                .Where(t => t.CreatedAt >= query.FromDate.Value);

        if (query.ToDate.HasValue)
            transactionsQuery = transactionsQuery
                .Where(t => t.CreatedAt <= query.ToDate.Value);

        if (query.MinAmount.HasValue)
            transactionsQuery = transactionsQuery
                .Where(t => Math.Abs(t.Amount) >= query.MinAmount.Value);

        if (query.MaxAmount.HasValue)
            transactionsQuery = transactionsQuery
                .Where(t => Math.Abs(t.Amount) <= query.MaxAmount.Value);

        var totalCount = transactionsQuery.Count();

        var items = transactionsQuery
            .OrderByDescending(t => t.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(t => new TransactionDto
            {
                Id = t.Id,
                Amount = t.Amount,
                Type = t.Type,
                Description = t.Description,
                CreatedAt = t.CreatedAt
            })
            .ToList();

        return new PagedResult<TransactionDto>
        {
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount,
            Items = items
        };
    }
}
