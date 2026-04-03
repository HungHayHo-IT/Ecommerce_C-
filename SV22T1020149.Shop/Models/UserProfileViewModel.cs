using System.ComponentModel.DataAnnotations;

namespace SV22T1020149.Shop.Models
{
    public class UserProfileViewModel
    {
        public int CustomerID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        public string CustomerName { get; set; } = "";

        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string Phone { get; set; } = "";

        public string Address { get; set; } = "";

        public string Province { get; set; } = "";

        // --- Phần đổi mật khẩu ---
        public string? OldPassword { get; set; }

        [MinLength(6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự")]
        public string? NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string? ConfirmPassword { get; set; }
    }
}