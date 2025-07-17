using MediatR;
using Talabeyah.OrderManagement.Domain.Interfaces;

namespace Talabeyah.OrderManagement.Application.Audit.Queries;

public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, List<AuditLogListItemDto>>
{
    private readonly IAuditLogRepository _auditLogRepository;
    public GetAuditLogsQueryHandler(IAuditLogRepository auditLogRepository) => _auditLogRepository = auditLogRepository;

    public async Task<List<AuditLogListItemDto>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        // For demo, assume repository exposes a method to get all logs (implement in infra next)
        var logs = await _auditLogRepository.GetAllAsync();
        return logs.Select(l => new AuditLogListItemDto(l.Id, l.Change, l.CreatedAt)).ToList();
    }
} 