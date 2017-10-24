namespace GeneAutomate.BDD
{
    public class Formater
    {
        public static string FormatParameter(string f, int i)
        {
            return $"{f.Replace('(', '_').Replace(')', '_')}_{i}";
        }
    }
}