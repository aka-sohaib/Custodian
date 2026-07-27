using System.ComponentModel.DataAnnotations;

namespace Custodian.Application.DTOs.Vendors
{
    public record CreateVendorDTO(
        [Required(ErrorMessage = "Vendor name is required.")]
        [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
        string Name,

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(256)]
        string Email,

        [Range(0, 365, ErrorMessage = "Payment term days must be between 0 and 365.")]
        int PaymentTermDays,

        [MaxLength(50, ErrorMessage = "Phone number cannot exceed 50 characters.")]
        string? Phone
    );
}
