using Custodian.Application.Common.Interfaces;
using Custodian.Domain.Entities;
using Custodian.Domain.Enums;
using Custodian.Domain.Interfaces;
using MediatR;

namespace Custodian.Application.Features.Invoices.CreateInvoiceCommand.Commands;

public class CreateInvoiceCommandHandler: IRequestHandler<CreateInvoiceCommand, Guid>
{
    private readonly IInternalUserRepository _internalUserRepository;
    private readonly IVendorUserRepository _vendorRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateInvoiceCommandHandler(IInternalUserRepository internalUserRepository, IVendorUserRepository vendorRepository, IInvoiceRepository invoiceRepository, ICurrentUserService currentUserService)
    {
        _internalUserRepository = internalUserRepository;
        _vendorRepository = vendorRepository;
        _invoiceRepository = invoiceRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreateInvoiceCommand command, CancellationToken cancellationToken)
    {
        //---- extract user from http context object ---
        var userId   = _currentUserService.UserId;
        var userRole = _currentUserService.Role;
        
        if (userId == Guid.Empty)
            throw new UnauthorizedAccessException("user is not authenticated.");

        //---- Check if user is a VendorUser ----
        var vendorUser = await _vendorRepository.GetByIdAsync(userId);
        bool isVendor  = vendorUser != null;

        if (isVendor && !command.OrganizationConnectionId.HasValue)
        {
            throw new UnauthorizedAccessException("Vendors must submit invoices under their registered organization connection.");
        }

        //---- create invoice ----
        var invoice = Invoice.Create(command.InvoiceNumber,
                                     command.OrganizationConnectionId,
                                     command.UnregisteredVendorName,
                                     userId,
                                     command.CurrencyCode,
                                     command.DueDate);

        //---- extract line itemds from dto & add to invoice ----
        foreach(var itemDto in command.LineItems)
        {
            var lineItem = LineItem.Create(invoice.Id, itemDto.Description, itemDto.Quantity, itemDto.UnitPrice);

            invoice.LineItems.Add(lineItem);
        }

        await _invoiceRepository.AddAsync(invoice);

        return invoice.Id;
    }
}
