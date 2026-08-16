using Custodian.Application.DTOs;
using Custodian.Application.Features.Invoices.CreateInvoiceCommand.Commands;
using Custodian.Application.Features.Invoices.CreateInvoiceCommand.Queries;
using Custodian.Application.Features.Invoices.ScanInvoice.Commands;
using Custodian.Application.Features.Invoices.UpdateInvoiceStatus.Commands;
using Custodian.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Custodian.Api.Endpoints
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InvoiceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("extract")]
        public async Task<IActionResult> ExtractInvoice([FromForm] IFormFile file)
        {
            var command = new ScanInvoiceCommand(file);
            var extractedData = await _mediator.Send(command);
            return Ok(extractedData);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceCommand command)
        {
            var invoiceId = await _mediator.Send(command);
            return Created($"api/invoice/{invoiceId}", new
            {
                Message = "Invoice Created Successfully.",
                InvoiceId = invoiceId
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoiceById([FromRoute] Guid id)
        {
            var query = new GetInvoiceByIdQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateInvoiceStatus([FromRoute] Guid id, [FromBody] UpdateInvoiceStatusDto dto)
        {
            var command = new UpdateInvoiceStatusCommand(id, dto.NewStatus, dto.RejectionReason);
            await _mediator.Send(command);
            return Ok(new { Message = $"Invoice status updated to {dto.NewStatus} successfully." });
        }
    }
}
