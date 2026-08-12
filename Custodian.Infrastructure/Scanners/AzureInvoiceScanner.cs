using Azure;
using Azure.AI.DocumentIntelligence;
using Custodian.Application.Common.Interfaces;
using Custodian.Application.DTOs;
using Microsoft.Extensions.Configuration;

namespace Custodian.Infrastructure.Scanners;

public class AzureInvoiceScanner: IInvoiceScanner
{
    private readonly DocumentIntelligenceClient _client;

    public AzureInvoiceScanner(IConfiguration configuration)
    {
        //---- Get Endpoint & Key ----
        var endpoint = configuration["DocumentIntelligence:Endpoint"];
        var apiKey = configuration["DocumentIntelligence:ApiKey"];

        //---- Create Scanner Entity ----
        var credential = new AzureKeyCredential(apiKey!);
        _client = new DocumentIntelligenceClient(new Uri(endpoint!), credential);
    }

    public async Task<ExtractedInvoiceDto> ScanAsync(Stream fileStream, CancellationToken cancellationToken)
    {
        //---- fetch the binary data of the file ----
        var fileData = BinaryData.FromStream(fileStream);
        Operation<AnalyzeResult> operation = await _client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-invoice", fileData, cancellationToken: cancellationToken);

        //---- If nothing extracted, return null object ----
        var invoiceData = operation.Value.Documents.FirstOrDefault();
        if (invoiceData == null) return new ExtractedInvoiceDto(null, null, null, null, null, null, new List<ExtractedLineItemDto>());

        //---- extract invoice fields from dictionary ----
        string? vendorName = invoiceData.Fields.TryGetValue("VendorName", out var vendorField) ? vendorField.ValueString : null;
        string? invoiceNumber = invoiceData.Fields.TryGetValue("InvoiceId", out var idField) ? idField.ValueString : null;
        DateTime? date = invoiceData.Fields.TryGetValue("InvoiceDate", out var dateField) ? dateField.ValueDate?.DateTime : null;
        DateTime? dueDate = invoiceData.Fields.TryGetValue("DueDate", out var dueDateField) ? dueDateField.ValueDate?.DateTime : null;
        string? currencyCode = invoiceData.Fields.TryGetValue("CurrencyCode", out var currencyField) ? currencyField.ValueString : "USD";
        decimal? total = invoiceData.Fields.TryGetValue("InvoiceTotal", out var totalField) ? (decimal?)totalField.ValueCurrency?.Amount : null;

        //---- extract line items ----
        var lineItems = new List<ExtractedLineItemDto>();

        if (invoiceData.Fields.TryGetValue("Items", out var itemsField) && itemsField.ValueList != null)
        {
            foreach (var item in itemsField.ValueList)
            {
                if (item.ValueDictionary == null) continue;

                var fields = item.ValueDictionary;

                //---- extract line item fields ----
                string? description = fields.TryGetValue("Description", out var descField) ? descField.ValueString : null;
                decimal? quantity = fields.TryGetValue("Quantity", out var qtyField) ? (decimal?)qtyField.ValueDouble : null;
                decimal? unitPrice = fields.TryGetValue("UnitPrice", out var priceField) ? (decimal?)priceField.ValueDouble : null;
                decimal? extractedTotal = fields.TryGetValue("Amount", out var amtField) ? (decimal?)amtField.ValueDouble : null;

                lineItems.Add(new ExtractedLineItemDto(description, quantity, unitPrice, extractedTotal));
            }
        }

        return new ExtractedInvoiceDto(vendorName, invoiceNumber, date, dueDate, currencyCode, total, lineItems);
    }
}