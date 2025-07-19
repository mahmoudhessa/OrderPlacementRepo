using MediatR;

namespace Talabeyah.OrderManagement.Application.Audit.Queries;

public record GetAuditLogsQuery(string? Change = null, int Page = 1, int PageSize = 10) : IRequest<List<AuditLogListItemDto>>;

public record AuditLogListItemDto(int Id, string Change, DateTime CreatedAt); 