namespace CadViewer.Domain.Entities
{
    public class LayerInfo
    {
        public LayerInfo(string name, string? colorHex = null, bool isVisible = true)
        {
            Name = name;
            ColorHex = colorHex;
            IsVisible = isVisible;
        }

        public string Name { get; }

        public string? ColorHex { get; }

        public bool IsVisible { get; set; }
    }
}