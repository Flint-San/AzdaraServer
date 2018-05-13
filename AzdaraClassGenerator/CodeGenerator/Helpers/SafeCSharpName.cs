
namespace Azdara.CodeGenerator.Helpers
{
    using Microsoft.CSharp;
    using System.Text.RegularExpressions;

    public static class SafeCSharpName
    {
        public static string CSharpName(this string name)
        {
            bool isValid = CSharpCodeProvider.CreateProvider("C#").IsValidIdentifier(name);

            if (!isValid)
            {
                // name contains invalid chars, remove them
                Regex regex = new Regex(@"[^\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Nl}\p{Mn}\p{Mc}\p{Cf}\p{Pc}\p{Lm}]");
                name = regex.Replace(name, "");

                // name doesn't begin with a letter, insert an underscore
                if (!char.IsLetter(name, 0))
                {
                    name = name.Insert(0, "_");
                }
            }

            return name.Replace(" ", "_");
        }
    }
}
