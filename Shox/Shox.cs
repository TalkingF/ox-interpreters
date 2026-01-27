namespace Shox;

internal class Shox
{
    private static bool _hadError;

    // Main entry point
    private static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.Error.WriteLine("Usage: Shox [Script]");
            Environment.Exit(1);
        }

        if (args.Length == 1)
            Run(args[0]);

        else
            RunAsRepl();
    }

    // Runs file if present
    private static void RunFile(string path)
    {
        if (Path.Exists(path))
        {
            var contents = File.ReadAllBytes(path);

            var code = contents.ToString();

            if (code is null) return;

            Run(code);

            if (_hadError) Environment.Exit(1);
        }
    }

    //Run program as a REPL, interprets line by line
    private static void RunAsRepl()
    {
        Console.Write("> ");
        while (Console.ReadLine() is { } line)
        {
            Run(line);
            _hadError = false;
            Console.Write("> ");
        }
    }

    private static void Run(string contents)
    {
    }


    public static void Error(int line, string message)
    {
    }

    private static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine("[line: " + line + "] Error " + where + ": " +
                                message);
        _hadError = true;
    }
}