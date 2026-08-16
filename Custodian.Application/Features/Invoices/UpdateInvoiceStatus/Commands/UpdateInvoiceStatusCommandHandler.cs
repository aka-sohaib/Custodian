using Custodian.Application.Common.Exceptions;
using Custodian.Application.Common.Interfaces;
using Custodian.Domain.Enums;
using Custodian.Domain.Interfaces;
using MediatR;

namespace Custodian.Application.Features.Invoices.UpdateInvoiceStatus.Commands;

public class UpdateInvoiceStatusCommandHandler : IRequestHandler<UpdateInvoiceStatusCommand, bool>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IVendorUserRepository _vendorUserRepository;
    private readonly IInternalUserRepository _internalUserRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateInvoiceStatusCommandHandler(
        IInvoiceRepository invoiceRepository,
        IVendorUserRepository vendorUserRepository,
        IInternalUserRepository internalUserRepository,
        ICurrentUserService currentUserService)
    {
        _invoiceRepository      = invoiceRepository;
        _vendorUserRepository   = vendorUserRepository;
        _internalUserRepository = internalUserRepository;
        _currentUserService     = currentUserService;
    }

    public async Task<bool> Handle(UpdateInvoiceStatusCommand command, CancellationToken cancellationToken)
    {
        //---- Extract user ID from JWT bearer context ----
        var userId = _currentUserService.UserId;
        if (userId == Guid.Empty)
        {
            throw new UnauthorizedException("User is not authenticated.");
        }

        //---- Determine user type (Vendor vs Company Internal User) ----
        var vendorUser   = await _vendorUserRepository.GetByIdAsync(userId);
        var internalUser = vendorUser == null ? await _internalUserRepository.GetByIdAsync(userId) : null;

        if (vendorUser == null && internalUser == null)
        {
            throw new NotFoundException($"User with ID '{userId}' was not found.");
        }

        bool isVendor = vendorUser != null;

        //---- Enforce user type permissions ----
        if (isVendor)
        {
            //---- Vendors can ONLY submit or cancel their invoices ----
            if (command.NewStatus != Status.Submitted && command.NewStatus != Status.Cancelled)
            {
                throw new UnauthorizedException("Vendor users are only authorized to submit or cancel their invoices.");
            }
        }
        else
        {
            //---- Internal company users cannot re-submit invoices ----
            if (command.NewStatus == Status.Submitted)
            {
                throw new BadRequestException("Company users cannot re-submit an invoice. Invoices are submitted by vendors.");
            }
        }

        //---- Fetch invoice by ID ----
        var invoice = await _invoiceRepository.GetByIdAsync(command.InvoiceId)
            ?? throw new NotFoundException($"Invoice with ID '{command.InvoiceId}' was not found.");

        //---- Transition domain state based on NewStatus ----
        switch (command.NewStatus)
        {
            case Status.Submitted:
                invoice.Submit();
                break;

            case Status.UnderReview:
                invoice.BeginReview();
                break;

            case Status.Approved:
                invoice.Approve();
                break;

            case Status.Rejected:
                if (string.IsNullOrWhiteSpace(command.RejectionReason))
                {
                    throw new BadRequestException("Rejection reason is required when rejecting an invoice.");
                }
                invoice.Reject(command.RejectionReason);
                break;

            case Status.Paid:
                invoice.MarkAsPaid();
                break;

            case Status.Cancelled:
                invoice.Cancel();
                break;

            case Status.Draft:
                throw new BadRequestException("Cannot revert an active invoice back to Draft status.");

            default:
                throw new BadRequestException($"Unsupported status transition: {command.NewStatus}");
        }

        //---- Update repository ----
        await _invoiceRepository.UpdateAsync(invoice);

        return true;
    }
}
