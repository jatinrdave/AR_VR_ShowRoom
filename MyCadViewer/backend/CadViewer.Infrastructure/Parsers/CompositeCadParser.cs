using CadViewer.Domain.Entities;
using CadViewer.Domain.Interfaces;

namespace CadViewer.Infrastructure.Parsers
{
	public class CompositeCadParser : ICadParser
	{
		private readonly IReadOnlyList<ICadParser> _parsers;

		public CompositeCadParser(IEnumerable<ICadParser> parsers)
		{
			_parsers = parsers.ToList();
		}

		public bool CanParse(string fileName) => _parsers.Any(p => p.CanParse(fileName));

		public async Task<CadModel> ParseAsync(Stream fileStream, string fileName, CancellationToken cancellationToken)
		{
			var parser = _parsers.FirstOrDefault(p => p.CanParse(fileName));
			if (parser == null) throw new NotSupportedException($"No parser for {fileName}");
			return await parser.ParseAsync(fileStream, fileName, cancellationToken);
		}
	}
}