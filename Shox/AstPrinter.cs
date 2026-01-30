using System.Text;

namespace Shox;

public class AstPrinter : IVisitor<string>
{
    public string VisitBinaryExpr(Binary expr)
    {
        return Parenthesize(expr.Op.Lexeme, expr.Left, expr.Right);
    }

    public string VisitGroupingExpr(Grouping expr)
    {
        return Parenthesize("group ", expr.Expression);
    }

    public string VisitLiteralExpr(Literal expr)
    {
        return expr.Value == null ? "nil" : expr.Value.ToString()!;
    }

    public string VisitUnaryExpr(Unary expr)
    {
        return Parenthesize(expr.Op.Lexeme, expr.Right);
    }

    public string Print(Expr expr)
    {
        return expr.Accept(this);
    }

    private string Parenthesize(string name, params Expr[] exprs)
    {
        var builder = new StringBuilder();
        builder.Append('(').Append(name);
        foreach (var expr in exprs)
        {
            builder.Append(' ');
            builder.Append(expr.Accept(this));
        }

        builder.Append(')');

        return builder.ToString();
    }
}