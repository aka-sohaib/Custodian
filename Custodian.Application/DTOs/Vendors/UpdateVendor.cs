using System.ComponentModel.DataAnnotations;

namespace Custodian.Application.DTOs.Vendors
{
    public record UpdateVendorDTO(
        [Required(ErrorMessage = "Vendor name is required.")]
        [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
        string Name,

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(256)]
        string Email,

        [MaxLength(50, ErrorMessage = "Phone number cannot exceed 50 characters.")]
        string? Phone
    );
}
