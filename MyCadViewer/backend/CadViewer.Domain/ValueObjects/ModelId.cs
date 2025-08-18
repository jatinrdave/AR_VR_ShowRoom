namespace CadViewer.Domain.ValueObjects
{
    public readonly struct ModelId
    {
        public ModelId(Guid value)
        {
            Value = value;
        }

        public Guid Value { get; }

        public static ModelId New() => new ModelId(Guid.NewGuid());

        public override string ToString() => Value.ToString("N");

        public static bool TryParse(string? text, out ModelId id)
        {
            id = default;
            if (Guid.TryParse(text, out var g))
            {
                id = new ModelId(g);
                return true;
            }
            return false;
        }
    }
}