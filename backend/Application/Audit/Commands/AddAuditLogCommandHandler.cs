using MediatR;
using Talabeyah.OrderManagement.Domain.Entities;
using Talabeyah.OrderManagement.Domain.Interfaces;

namespace Talabeyah.OrderManagement.Application.Audit.Commands;

public class AddAuditLogCommandHandler : IRequestHandler<AddAuditLogCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddAuditLogCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(AddAuditLogCommand request, CancellationToken cancellationToken)
    {
        var log = new AuditLog(request.Change);
        await _unitOfWork.AuditLogRepository.AddAsync(log);
        return Unit.Value;
    }
} 