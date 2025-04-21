
using ANTLR;
using Antlr4.Runtime;



class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.Error.WriteLine("Usage: LanguageParser <filename>");
            Environment.Exit(1);
        }

        try
        {
            string code = File.ReadAllText(args[0]);
            ICharStream input = CharStreams.fromString(code);
            
            LanguageLexer lexer = new LanguageLexer(input);
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(new SyntaxErrorHandler());
            
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            
            LanguageParser parser = new LanguageParser(tokens);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(new SyntaxErrorHandler());
            
            var tree = parser.program();

            Console.WriteLine("Parsed successfully!");
            
            TypeChecker typeChecker = new TypeChecker();
            typeChecker.Visit(tree);

            if (typeChecker.Errors.Count > 0)
            {
                Console.Error.WriteLine("Type errors detected:");
                foreach (var error in typeChecker.Errors)
                {
                    Console.Error.WriteLine(error);
                }
                Environment.Exit(1);
            }

            Console.WriteLine("Type checking passed!");
            
            StackCodeGenerator codeGenerator = new StackCodeGenerator();
            codeGenerator.Visit(tree);
            List<String> stackCode = codeGenerator.output;
            string outputPath = "output.txt";
            
            File.WriteAllLines(outputPath, stackCode);

            Console.WriteLine($"Target code saved to {outputPath}");
            
            StackInterpreter interpreter = new StackInterpreter();
            interpreter.Execute("output.txt");
            
        }
        catch (SyntaxErrorException e)
        {
            Console.Error.WriteLine("Syntax error: " + e.Message);
            Environment.Exit(1);
        }
        catch (IOException e)
        {
            Console.Error.WriteLine("Error reading file: " + e.Message);
            Environment.Exit(1);
        }
    }
}
