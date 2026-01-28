namespace Shox;

public abstract record Expr;

public record Binary(Expr Left, Token Op, Expr Right) : Expr;

public record Grouping(Expr Expression) : Expr;

public record Unary(Token Op, Expr Right) : Expr;

public record Literal(object Value) : Expr;