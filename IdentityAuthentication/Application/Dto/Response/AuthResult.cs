namespace WebApplication5.Application.Dto.Response
{
    public class AuthResult
    {
        public bool Succeeded { get; set; }
        public AuthResponseDTO Response { get; set; }
        public string Error { get; set; }

        public static AuthResult Success(AuthResponseDTO response) => new AuthResult { Succeeded = true, Response = response };
        public static AuthResult Failure(string error) => new AuthResult { Succeeded = false, Error = error };

    }
}
