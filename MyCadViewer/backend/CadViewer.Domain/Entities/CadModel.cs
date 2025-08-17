using System.Numerics;

namespace CadViewer.Domain.Entities
{
    public class CadModel
    {
        public CadModel(ValueObjects.ModelId id, string originalFileName)
        {
            Id = id;
            OriginalFileName = originalFileName;
        }

        public ValueObjects.ModelId Id { get; }

        public string OriginalFileName { get; }

        public IReadOnlyList<LayerInfo> Layers => _layers.AsReadOnly();

        public IReadOnlyList<LineSegment3D> LineSegments => _lineSegments.AsReadOnly();

        private readonly List<LayerInfo> _layers = new();

        private readonly List<LineSegment3D> _lineSegments = new();

        public void AddLayer(LayerInfo layer)
        {
            if (_layers.Any(l => string.Equals(l.Name, layer.Name, StringComparison.OrdinalIgnoreCase))) return;
            _layers.Add(layer);
        }

        public void AddLineSegment(LineSegment3D segment)
        {
            _lineSegments.Add(segment);
        }
    }
}