namespace BlogWebApi.Dtos
{
    public class Response
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Username { get; set; }
        public List<string> Roles { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresOn { get; set; }
    }
}
