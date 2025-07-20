using MediatR;
using Talabeyah.OrderManagement.Domain.Entities;
using Talabeyah.OrderManagement.Domain.Interfaces;
using Talabeyah.OrderManagement.Application.Contracts;
using Microsoft.Extensions.Logging;

namespace Talabeyah.OrderManagement.Application.Products.Commands;

public class AddProductCommandHandler : IRequestHandler<AddProductCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextAccessor _userContextAccessor;
    private readonly ILogger<AddProductCommandHandler> _logger;
    
    public AddProductCommandHandler(IUnitOfWork unitOfWork, IUserContextAccessor userContextAccessor, ILogger<AddProductCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _userContextAccessor = userContextAccessor;
        _logger = logger;
    }

    public async Task<int> Handle(AddProductCommand request, CancellationToken cancellationToken)
    {
        var userContext = _userContextAccessor.GetUserContext();
        
        _logger.LogInformation("AddProductCommand - UserContext: {UserContext}", userContext != null ? "NotNull" : "Null");
        if (userContext != null)
        {
            _logger.LogInformation("AddProductCommand - UserId: {UserId}, Email: {Email}", userContext.UserId, userContext.Email);
            _logger.LogInformation("AddProductCommand - Roles: {Roles}", string.Join(", ", userContext.Roles));
        }
        
        if (userContext == null || !(userContext.Roles.Contains("Admin") || userContext.Roles.Contains("InventoryManager")))
        {
            _logger.LogWarning("AddProductCommand - Authorization failed. User: {UserId}, Roles: {Roles}", 
                userContext?.UserId, userContext?.Roles != null ? string.Join(", ", userContext.Roles) : "null");
            throw new UnauthorizedAccessException("User does not have permission to add products.");
        }
        
        _logger.LogInformation("AddProductCommand - Authorization successful for user: {UserId}", userContext.UserId);
        
        var product = new Product
        {
            Name = request.Name,
            Inventory = request.Inventory
        };
        await _unitOfWork.ProductRepository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return product.Id;
    }
} 