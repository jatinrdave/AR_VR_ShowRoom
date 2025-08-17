using CadViewer.Domain.Entities;
using CadViewer.Domain.Interfaces;
using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Scenes;

namespace CadViewer.Infrastructure.Converters
{
	public class BasicGltfConverter : IModelConverter
	{
		public Task<byte[]> ConvertToGltfAsync(CadModel model, CancellationToken cancellationToken)
		{
			var scene = new SceneBuilder();
			var material = new MaterialBuilder().WithUnlitShader().WithDoubleSide(true);

			var byLayer = model.LineSegments.GroupBy(s => s.LayerName ?? "_default");
			foreach (var group in byLayer)
			{
				var mesh = new MeshBuilder<VertexPosition, VertexEmpty, VertexEmpty>($"layer_{group.Key}");
				var prim = mesh.UsePrimitive(material, SharpGLTF.Geometry.PrimitiveType.LINES);
				foreach (var seg in group)
				{
					var i0 = prim.AddVertex(new VertexPosition(seg.Start));
					var i1 = prim.AddVertex(new VertexPosition(seg.End));
					prim.AddLine(i0, i1);
				}
				scene.AddRigidMesh(mesh, System.Numerics.Matrix4x4.Identity, nodeName: $"Layer:{group.Key}");
			}

			var modelRoot = scene.ToGltf2();
			using var ms = new MemoryStream();
			modelRoot.SaveGLB(ms);
			return Task.FromResult(ms.ToArray());
		}

		public Task<string> ConvertToSvgAsync(CadModel model, CancellationToken cancellationToken)
		{
			var sb = new System.Text.StringBuilder();
			sb.Append("<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\">");
			foreach (var seg in model.LineSegments)
			{
				sb.Append($"<line x1=\"{seg.Start.X}\" y1=\"{-seg.Start.Y}\" x2=\"{seg.End.X}\" y2=\"{-seg.End.Y}\" stroke=\"black\" stroke-width=\"1\" />");
			}
			sb.Append("</svg>");
			return Task.FromResult(sb.ToString());
		}
	}
}