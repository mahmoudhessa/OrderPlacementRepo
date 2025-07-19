using MediatR;
using Talabeyah.OrderManagement.Domain.Interfaces;

namespace Talabeyah.OrderManagement.Application.Audit.Queries;

public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, List<AuditLogListItemDto>>
{
    private readonly IAuditLogRepository _auditLogRepository;
    public GetAuditLogsQueryHandler(IAuditLogRepository auditLogRepository) => _auditLogRepository = auditLogRepository;

    public async Task<List<AuditLogListItemDto>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var logs = await _auditLogRepository.GetAllAsync();
        if (!string.IsNullOrWhiteSpace(request.Change))
            logs = logs.Where(l => l.Change.Contains(request.Change, StringComparison.OrdinalIgnoreCase)).ToList();
        logs = logs.OrderByDescending(l => l.CreatedAt).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
        return logs.Select(l => new AuditLogListItemDto(l.Id, l.Change, l.CreatedAt)).ToList();
    }
} 