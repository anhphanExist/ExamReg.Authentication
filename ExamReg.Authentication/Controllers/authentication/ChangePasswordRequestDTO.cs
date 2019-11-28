using ExamReg.Authentication.Common;

namespace ExamReg.Authentication.Controllers.authentication
{
    public class ChangePasswordRequestDTO : DataDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
    }
}