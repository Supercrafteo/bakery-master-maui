namespace PasteleriaMaui.Models
{
    public class PastelVisualItem
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string BizcochoNombre { get; set; } = string.Empty;
        public string GlaseadoNombre { get; set; } = string.Empty;
        public string RellenoSugerido { get; set; } = string.Empty;
        public string FotoUrl { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string ColorAcento { get; set; } = "#8B5E34";
        public string Emoji { get; set; } = "🎂";
        public int CategoriaId { get; set; } = 1;
    }
}
