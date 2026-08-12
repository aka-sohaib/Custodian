using Custodian.Application.Features.Invoices.ScanInvoice.Commands;
using FluentValidation;

namespace Custodian.Application.Features.Invoices.ScanInvoice.Commands;

public class ScanInvoiceCommandValidator : AbstractValidator<ScanInvoiceCommand>
{
    public ScanInvoiceCommandValidator()
    {
        RuleFor(x => x.File)
            .NotNull().WithMessage("No file was uploaded.")
            .Must(file => file.Length > 0).WithMessage("The uploaded file is empty.")
            .Must(file => file.Length <= 1024 * 1024).WithMessage("File size cannot exceed 1MB.")
            .Must(file => file.ContentType == "application/pdf" ||
                          file.ContentType == "image/jpeg" ||
                          file.ContentType == "image/png")
            .WithMessage("Only PDF, JPG, and PNG files are supported.");
    }
}