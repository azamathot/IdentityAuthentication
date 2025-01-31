namespace WebApplication5.Application.Dto.Response
{
    public class AuthResponseDTO
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string Error { get; set; } // Добавим поле для ошибки

        public AuthResponseDTO(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }

        public AuthResponseDTO(string error)
        {
            Error = error;
        }
    }
}
