using Antlr4.Runtime;



public class SyntaxErrorHandler : IAntlrErrorListener<int>, IAntlrErrorListener<IToken>
{
    public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        throw new SyntaxErrorException($"Line {line}:{charPositionInLine} {msg}");
    }

    public void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        throw new SyntaxErrorException($"Line {line}:{charPositionInLine} {msg}");
    }
}