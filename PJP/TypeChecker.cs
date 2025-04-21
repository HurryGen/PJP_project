using System;
using System.Collections.Generic;
using ANTLR;
using Antlr4.Runtime.Tree;

public class TypeChecker : LanguageBaseVisitor<string>
{
    public List<string> Errors { get; } = new List<string>();
    
    private readonly Dictionary<string, string> symbolTable = new Dictionary<string, string>();

    public override string VisitDeclaration(LanguageParser.DeclarationContext context)
    {
        string declaredType = context.type().GetText();

        foreach (var id in context.variableList().IDENTIFIER())
        {
            string name = id.GetText();
            if (symbolTable.ContainsKey(name))
            {
                Errors.Add($"Line {context.Start.Line}:{id.Symbol.Column} Variable '{name}' is already declared.");
            }
            else
            {
                symbolTable[name] = declaredType;
            }
        }

        return null;
    }

    public override string VisitAssignExpr(LanguageParser.AssignExprContext context)
    {
        string leftType = Visit(context.expression(0));
        string rightType = Visit(context.expression(1));

        if (leftType != null && rightType != null)
        {
            if (leftType == rightType)
            {
                
            }
            else if (leftType == "float" && rightType == "int")
            {
                
            }
            else
            {
                Errors.Add($"Line {context.Start.Line}:{context.Start.Column} Type mismatch in assignment. Cannot assign '{rightType}' to '{leftType}'.");
            }
        }

        return leftType;
    }

    public override string VisitLiteralExpr(LanguageParser.LiteralExprContext context)
    {
        return Visit(context.literal());
    }

    public override string VisitLiteral(LanguageParser.LiteralContext context)
    {
        if (context.INT_LITERAL() != null)
            return "int";
        if (context.FLOAT_LITERAL() != null)
            return "float";
        if (context.BOOL_LITERAL() != null)
            return "bool";
        if (context.STRING_LITERAL() != null)
            return "string";
        
        return "unknown";
    }

    public override string VisitVariableExpr(LanguageParser.VariableExprContext context)
    {
        string name = context.IDENTIFIER().GetText();
        if (!symbolTable.TryGetValue(name, out string varType))
        {
            Errors.Add($"Line {context.Start.Line}:{context.Start.Column} Variable '{name}' is not declared.");
            return "unknown";
        }

        return varType;
    }
    public override string VisitAddExpr(LanguageParser.AddExprContext context)
    {
        string left = Visit(context.expression(0));
        string right = Visit(context.expression(1));
        string op = context.GetChild(1).GetText();


        if (op == ".")
        {
            if (left == "string" && right == "string")
                return "string";
            
            Errors.Add($"Line {context.Start.Line}:{context.Start.Column} Invalid operation '{op}' between '{left}' and '{right}'.");
            return "unknown";
        }
        
        if ((left == "int" && right == "int") || (left == "float" && right == "float"))
        {
            return (left == "float" || right == "float") ? "float" : "int";
        }
        if ((left == "int" && right == "float") || (left == "float" && right == "int"))
        {
            return "float";
        }
        if (left == "string" || right == "string")
        {
            Errors.Add($"Line {context.Start.Line}:{context.Start.Column} Invalid operation '{op}' between '{left}' and '{right}'.");
            return "unknown";
        }
        
        return left; 
    }

    public override string VisitMulExpr(LanguageParser.MulExprContext context)
    {
        string left = Visit(context.expression(0));
        string right = Visit(context.expression(1));
        string op = context.GetChild(1).GetText();

        
        if ((left == "int" && right == "int") || (left == "float" && right == "float"))
        {
            if (op == "%" && left != "int")
            {
                Errors.Add($"Line {context.Start.Line}:{context.Start.Column} Modulo operator (%) is only valid for integers.");
                return "unknown";
            }
            return (left == "float" || right == "float") ? "float" : "int";
        }

        
        if ((left == "int" && right == "float") || (left == "float" && right == "int"))
        {
            if (op == "%")
            {
                Errors.Add($"Line {context.Start.Line}:{context.Start.Column} Modulo operator (%) is only valid for integers.");
                return "unknown";
            }
            return "float";
        }

        Errors.Add($"Line {context.Start.Line}:{context.Start.Column} Type mismatch in {op} operation: {left} {op} {right}");
        return "unknown";
    }
    
    public override string VisitEqExpr(LanguageParser.EqExprContext context)
    {
        string left = Visit(context.expression(0));
        string right = Visit(context.expression(1));

        if (left != right)
        {
            Errors.Add($"Line {context.Start.Line}:{context.Start.Column} Type mismatch in equality check: {left} == {right}");
            return "unknown";
        }

        return "bool"; 
    }
    
    public override string VisitNotExpr(LanguageParser.NotExprContext context)
    {
        string exprType = Visit(context.expression());
        if (exprType != "bool")
        {
            Errors.Add($"Line {context.Start.Line}:{context.Start.Column} Type mismatch in NOT operation: !{exprType}");
            return "unknown";
        }

        return "bool"; 
    }
    
    public override string VisitOrExpr(LanguageParser.OrExprContext context)
    {
        string left = Visit(context.expression(0));
        string right = Visit(context.expression(1));

        if (left != right)
        {
            Errors.Add($"Line {context.Start.Line}:{context.Start.Column} Type mismatch in OR operation: {left} || {right}");
            return "unknown";
        }

        return left; 
    }
    
    public override string VisitUminusExpr(LanguageParser.UminusExprContext context)
    {
        string exprType = Visit(context.expression());
        if (exprType != "int" && exprType != "float")
        {
            Errors.Add($"Line {context.Start.Line}:{context.Start.Column} Type mismatch in unary minus operation: -{exprType}");
            return "unknown";
        }

        return exprType; 
    }
    
    public override string VisitParensExpr(LanguageParser.ParensExprContext context)
    {
        return Visit(context.expression());
    }
    
    public override string VisitRelExpr(LanguageParser.RelExprContext context)
    {
        string left = Visit(context.expression(0));
        string right = Visit(context.expression(1));
        string op = context.GetChild(1).GetText();

      
        if ((left == "int" && right == "int") ||
            (left == "float" && right == "float") ||
            (left == "int" && right == "float") ||
            (left == "float" && right == "int"))
        {
            return "bool";
        }

        Errors.Add($"Line {context.Start.Line}:{context.Start.Column} Type mismatch in relational operation: {left} {op} {right}");
        return "unknown";
    }
    
    public override string VisitIfStatement(LanguageParser.IfStatementContext context)
    {
        string conditionType = Visit(context.expression());
        if (conditionType != "bool")
        {
            Errors.Add($"Line {context.Start.Line}:{context.Start.Column} Type mismatch in IF condition: {conditionType}");
        }

        foreach (var statement in context.statement())
        {
            Visit(statement);
        }

        return null;
    }
    
    public override string VisitWhileStatement(LanguageParser.WhileStatementContext context)
    {
        string conditionType = Visit(context.expression());
        if (conditionType != "bool")
        {
            Errors.Add($"Line {context.Start.Line}:{context.Start.Column} Type mismatch in WHILE condition: {conditionType}");
        }
        Visit(context.statement());
        return null;
    }
    public override string VisitFopenStatement(LanguageParser.FopenStatementContext context)
    {
        string fileNameType = Visit(context.expression());
        string varName = context.IDENTIFIER().GetText();

        if (!symbolTable.TryGetValue(varName, out string varType))
        {
            Errors.Add($"Line {context.Start.Line}:{context.Start.Column} Variable '{varName}' is not declared.");
            return null;
        }

        if (varType != "file")
        {
            Errors.Add($"Line {context.Start.Line}:{context.Start.Column} Variable '{varName}' is not of type 'file'.");
            return null;
        }

        if (fileNameType != "string")
        {
            Errors.Add($"Line {context.Start.Line}:{context.Start.Column} fopen expects a string literal as file name.");
        }

        return null;
    }
    
    public override string VisitFileOutputExpr(LanguageParser.FileOutputExprContext context)
    {
        string left = Visit(context.expression(0));
        string right = Visit(context.expression(1));

        if (left != "file")
        {
            Errors.Add($"Line {context.Start.Line}:{context.Start.Column} Left side of '<<' must be a file, got '{left}'.");
            return "unknown";
        }

        
        if (right != "int" && right != "float" && right != "string")
        {
            Errors.Add($"Line {context.Start.Line}:{context.Start.Column} Cannot write '{right}' to file.");
        }

        return "file"; 
    }

   
    
    
    
}
