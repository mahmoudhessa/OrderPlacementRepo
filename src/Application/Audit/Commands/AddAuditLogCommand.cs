using MediatR;

namespace Talabeyah.OrderManagement.Application.Audit.Commands;

public record AddAuditLogCommand(string Change) : IRequest; 