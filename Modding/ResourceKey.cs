using System.Text.RegularExpressions;

namespace CodeName.EventSystem.Modding
{
    /// <summary>
    ///     Currently used to centralize code related to resource key parsing.
    /// </summary>
    public readonly struct ResourceKey
    {
        private readonly string internalKey;

        public ResourceKey(string key)
        {
            internalKey = key;
        }

        public string GetModId()
        {
            var modIdMatch = Regexes.SelectModId.Match(internalKey);

            return modIdMatch.Success ? modIdMatch.Groups["ModId"].Value : string.Empty;
        }

        public ResourceKey ReplaceCsharpUnsafeCharacters(char replacement = '_')
        {
            return new ResourceKey(Regexes.ReplaceCsharpUnsafeCharacters.Replace(internalKey, replacement.ToString()).Trim(replacement));
        }

        public override string ToString()
        {
            return internalKey;
        }

        public static implicit operator string(ResourceKey value)
        {
            return value.internalKey;
        }

        public static class Regexes
        {
            public static Regex SelectModId { get; } = new(@"^(?<ModId>[A-Za-z0-9_]+?):");
            public static Regex ReplaceCsharpUnsafeCharacters { get; } = new(@"[^A-Za-z0-9_]+");
        }
    }
}
