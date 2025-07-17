using MediatR;

namespace Talabeyah.OrderManagement.Application.Audit.Queries;

public record GetAuditLogsQuery() : IRequest<List<AuditLogListItemDto>>;

public record AuditLogListItemDto(int Id, string Change, DateTime CreatedAt); 