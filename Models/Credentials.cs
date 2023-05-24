namespace DiplomaWeb.Models {
    public class Credentials {
        public string Login { get; set; }
        public string Password { get; set; }



        public Credentials(string login, string password) {
            Login = login;
            Password = password;
        }

    }
}
