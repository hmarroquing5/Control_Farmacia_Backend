namespace ControlFarmacia.API.Models
{
    public class Usuario
    {
        // CAMBIO: de int a string
        public string Id { get; set; } = string.Empty; 
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; 
    }
}