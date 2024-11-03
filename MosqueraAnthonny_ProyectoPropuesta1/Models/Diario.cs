namespace MosqueraAnthonny_ProyectoPropuesta1.Models
{
    public class Diario
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public string Contenido { get; set; }
        public DateTime FechaHora { get; set; }
    }
}
