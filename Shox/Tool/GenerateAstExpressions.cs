using System.Text;

namespace Shox.Tool;

public static class GenerateAstExpressions

{
    private const string ExpressionPath = "Tool/ast-gen.txt";
    private const string OutputPath = "AstExpressions.cs";

    //Generates a csharp file with all the records and overrides for configured
    //ast expressions. For best effect its recommended that you format the output.
    public static void Generate()
    {
        const string initial = """
                               namespace Shox;
                               public abstract record Expr
                               {
                               public abstract T Accept<T>(IVisitor<T> visitor);
                               }

                               public interface IVisitor<out T>
                               {

                               """;

        var writeText = new StringBuilder(initial);

        var data = GetTypes();

        //generate visitor interfaces
        foreach (var tup in data)
            writeText.Append("T Visit").Append(tup.Item1).Append("Expr(")
                .Append(tup.Item1).AppendLine(" expr);");

        writeText.AppendLine("}");

        //generate records
        foreach (var tup in data)
        {
            //record class gen
            writeText.Append("public record ").Append(tup.Item1).Append('(');

            foreach (var (name, type) in tup.Item2)
                writeText.Append(type).Append(' ').Append(name).Append(", ");

            writeText.Remove(writeText.Length - 2, 2);

            writeText.AppendLine("): Expr");

            //override accept function
            writeText.AppendLine("{");

            writeText.AppendLine(
                "public override T Accept<T>(IVisitor<T> visitor) {");

            writeText.Append("return visitor.Visit").Append(tup.Item1)
                .AppendLine("Expr(this);");
            writeText.AppendLine("}");
            writeText.AppendLine("}");

            File.WriteAllText(OutputPath, writeText.ToString());
        }
    }


    //opens the ast expressions file and creates a list. Each entry in the list comprises
    //a tuple representing the expression name and a dictionary mapping name of the var to type.
    private static List<(string, Dictionary<string, string>)> GetTypes()
    {
        var data = new List<(string, Dictionary<string, string>)>();
        var lines = File.ReadLines(ExpressionPath);


        foreach (var line in lines)
        {
            var dict = new Dictionary<string, string>();

            var content = line.Split(' ');

            for (var i = 1; i < content.Length; i += 2)
                //e.g. Binary: Left
                dict[content[i + 1]] = content[i];

            var tup = (content[0], dict);
            data.Add(tup);
        }

        return data;
    }
}