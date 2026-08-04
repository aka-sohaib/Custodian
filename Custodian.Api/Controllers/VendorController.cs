using Custodian.Application.DTOs.Vendors;
using Custodian.Domain.Entities;
using Custodian.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Custodian.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VendorsController : ControllerBase
{
    private readonly IVendorRepository _vendorRepository;

    // Constructor Injection: ASP.NET gives us the registered repository automatically
    public VendorsController(IVendorRepository vendorRepository)
    {
        _vendorRepository = vendorRepository;
    }

    // GET: api/vendors
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var vendors = await _vendorRepository.GetAllAsync();
        return Ok(vendors); // Returns HTTP 200 with the JSON array
    }

    // GET: api/vendors/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var vendor = await _vendorRepository.GetByIdAsync(id);
        if (vendor == null)
        {
            return NotFound();
        }
        return Ok(vendor);
    }

    // POST: api/vendors
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVendorDTO dto)
    {
        // 1. Use your Domain Factory Method (which enforces validation rules)
        var newVendor = Vendor.Create(dto.Name, dto.Phone!, dto.Email);

        // 2. Persist to database via repository
        await _vendorRepository.AddAsync(newVendor);

        // 3. Return HTTP 201 Created pointing to the new resource
        return CreatedAtAction(nameof(GetById), new { id = newVendor.Id }, newVendor);
    }
}