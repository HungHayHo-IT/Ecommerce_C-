using System.ComponentModel.DataAnnotations;

namespace SV22T1020149.Shop.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui lòng nh?p h? và tên")]
        [Display(Name = "H? và tên")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng ch?n t?nh/thành")]
        [Display(Name = "T?nh / Thành")]
        public string Province { get; set; } = string.Empty;

        [Display(Name = "??a ch?")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Vui lòng nh?p s? ?i?n tho?i")]
        [Phone(ErrorMessage = "S? ?i?n tho?i không h?p l?")]
        [Display(Name = "S? ?i?n tho?i")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nh?p email")]
        [EmailAddress(ErrorMessage = "Email không h?p l?")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nh?p m?t kh?u")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "M?t kh?u ph?i có ít nh?t 6 ký t?")]
        [DataType(DataType.Password)]
        [Display(Name = "M?t kh?u")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng xác nh?n m?t kh?u")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "M?t kh?u xác nh?n không kh?p")]
        [Display(Name = "Xác nh?n m?t kh?u")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
