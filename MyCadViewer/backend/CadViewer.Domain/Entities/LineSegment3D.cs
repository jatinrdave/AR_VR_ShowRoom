using System.Numerics;

namespace CadViewer.Domain.Entities
{
    public class LineSegment3D
    {
        public LineSegment3D(Vector3 start, Vector3 end, string? layerName = null)
        {
            Start = start;
            End = end;
            LayerName = layerName;
        }

        public Vector3 Start { get; }

        public Vector3 End { get; }

        public string? LayerName { get; }
    }
}