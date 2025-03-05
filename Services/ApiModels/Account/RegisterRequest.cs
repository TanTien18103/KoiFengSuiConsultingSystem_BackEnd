namespace Services.ApiModels.Account
{
    public class RegisterRequest
    {
        public string FullName { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public bool? Gender { get; set; }
    }
}
