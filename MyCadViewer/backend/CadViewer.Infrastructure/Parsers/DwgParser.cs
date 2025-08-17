using CadViewer.Domain.Entities;
using CadViewer.Domain.Interfaces;
using ACadSharp;
using ACadSharp.IO;
using System.Numerics;

namespace CadViewer.Infrastructure.Parsers
{
	public class DwgParser : ICadParser
	{
		public bool CanParse(string fileName)
		{
			var ext = Path.GetExtension(fileName).ToLowerInvariant();
			return ext == ".dwg";
		}

		public async Task<CadModel> ParseAsync(Stream fileStream, string fileName, CancellationToken cancellationToken)
		{
			var temp = Path.GetTempFileName();
			await using (var fs = File.Create(temp))
			{
				await fileStream.CopyToAsync(fs, cancellationToken);
			}

			using var reader = DwgReader.Create(temp);
			var db = reader.Read();
			var model = new CadModel(new Domain.ValueObjects.ModelId(Guid.NewGuid()), fileName);

			foreach (var layer in db.Layers)
			{
				model.AddLayer(new LayerInfo(layer.Name, null, layer.Flags.HasFlag(StandardFlagsEntity.EntityIsOn)));
			}

			foreach (var ent in db.Entities)
			{
				if (ent is ACadSharp.Entities.Line line)
				{
					var s = new Vector3((float)line.StartPoint.X, (float)line.StartPoint.Y, (float)line.StartPoint.Z);
					var e = new Vector3((float)line.EndPoint.X, (float)line.EndPoint.Y, (float)line.EndPoint.Z);
					model.AddLineSegment(new LineSegment3D(s, e, line.Layer?.Name));
				}
			}

			File.Delete(temp);
			return model;
		}
	}
}