namespace GeneAutomate.BDD
{
    public class Formatter
    {
        public static string FormatParameter(string f, int i)
        {
            return $"{f.Replace('(', '_').Replace(')', '_')}_{i}";
        }

        public static string OptionalRelation(string from, string to) 
            => $"R_{from}_{to}";

        public static string Function(int funcNumber, string key) 
            => $"#F{funcNumber}_{key}";
    }
}