using MediatR;
using Talabeyah.OrderManagement.Domain.Entities;
using Talabeyah.OrderManagement.Domain.Interfaces;

namespace Talabeyah.OrderManagement.Application.Audit.Commands;

public class AddAuditLogCommandHandler : IRequestHandler<AddAuditLogCommand>
{
    private readonly IAuditLogRepository _auditLogRepository;

    public AddAuditLogCommandHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<Unit> Handle(AddAuditLogCommand request, CancellationToken cancellationToken)
    {
        var log = new AuditLog(request.Change);
        await _auditLogRepository.AddAsync(log);
        return Unit.Value;
    }
} 