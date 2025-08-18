using CadViewer.Domain.Entities;
using CadViewer.Domain.Interfaces;
using netDxf;
using System.Numerics;

namespace CadViewer.Infrastructure.Parsers
{
	public class DxfParser : ICadParser
	{
		public bool CanParse(string fileName)
		{
			var ext = Path.GetExtension(fileName).ToLowerInvariant();
			return ext == ".dxf";
		}

		public Task<CadModel> ParseAsync(Stream fileStream, string fileName, CancellationToken cancellationToken)
		{
			fileStream.Position = 0;
			var temp = Path.GetTempFileName();
			using (var fs = File.Create(temp))
			{
				fileStream.CopyTo(fs);
			}
			var doc = DxfDocument.Load(temp);
			var model = new CadModel(new Domain.ValueObjects.ModelId(Guid.NewGuid()), fileName);
			// Layers
			foreach (var layer in doc.Layers)
			{
				var color = layer.Color.IsByLayer ? null : $"#{layer.Color.ToArgb() & 0xFFFFFF:X6}";
				model.AddLayer(new LayerInfo(layer.Name, color, layer.IsVisible));
			}
			// Lines
			foreach (var ln in doc.Lines)
			{
				var s = new Vector3((float)ln.StartPoint.X, (float)ln.StartPoint.Y, (float)ln.StartPoint.Z);
				var e = new Vector3((float)ln.EndPoint.X, (float)ln.EndPoint.Y, (float)ln.EndPoint.Z);
				model.AddLineSegment(new LineSegment3D(s, e, ln.Layer?.Name));
			}
			File.Delete(temp);
			return Task.FromResult(model);
		}
	}
}