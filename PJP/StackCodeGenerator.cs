using System;
using System.Collections.Generic;
using ANTLR;
using Antlr4.Runtime.Tree;
using System.Globalization;
using Antlr4.Runtime;

public class StackCodeGenerator : LanguageBaseVisitor<string>
{
    private readonly Dictionary<string, string> symbolTable = new Dictionary<string, string>();
    public List<string> output = new List<string>();

    public override string VisitDeclaration(LanguageParser.DeclarationContext context)
    {
        string declaredType = context.type().GetText();

        foreach (var id in context.variableList().IDENTIFIER())
        {
            string name = id.GetText();
            symbolTable[name] = declaredType;

            switch (declaredType)
            {
                case "int":
                    Console.WriteLine($"push I 0");
                    output.Add($"push I 0");
                    break;
                case "float":
                    Console.WriteLine($"push F 0.0");
                    output.Add($"push F 0.0");
                    break;
                case "string":
                    Console.WriteLine($"push S \"\"");
                    output.Add($"push S \"\"");
                    break;
                case "bool":
                    Console.WriteLine($"push B false");
                    output.Add($"push B false");
                    break;
            }

            Console.WriteLine($"save {name}");
            output.Add($"save {name}");
        }

        return null;
    }

    public override string VisitAssignExpr(LanguageParser.AssignExprContext context)
    {
        List<string> targets = new List<string>();
        LanguageParser.AssignExprContext currentAssign = context;

        while (currentAssign != null)
        {
            if (currentAssign.expression(0) is LanguageParser.VariableExprContext varExpr)
            {
                targets.Add(varExpr.IDENTIFIER().GetText());
                break;
            }
            else if (currentAssign.expression(0) is LanguageParser.AssignExprContext nextAssign)
            {
                if (nextAssign.expression(1) is LanguageParser.VariableExprContext midVarExpr)
                {
                    targets.Add(midVarExpr.IDENTIFIER().GetText());
                }

                currentAssign = nextAssign;
            }
            else
            {
                break;
            }
        }

        string valueType = Visit(context.expression(1));
        
        targets.Reverse();

        if (valueType == "int" && targets.Any(name => symbolTable.TryGetValue(name, out var t) && t == "float"))
        {
            Console.WriteLine("itof");
            output.Add("itof");
            valueType = "float";
        }

        // Uložení výsledku výrazu do všech proměnných
        for (int i = targets.Count - 1; i >= 0; i--)
        {
            if (i < targets.Count - 1)
            {
                Console.WriteLine($"load {targets[i + 1]}");
                output.Add($"load {targets[i + 1]}");
            }
            Console.WriteLine($"save {targets[i]}");
            output.Add($"save {targets[i]}");
        }

        Console.WriteLine($"load {targets[0]}");
        output.Add($"load {targets[0]}");
        Console.WriteLine("pop");
        output.Add("pop");

        return valueType;
    }

    public override string VisitLiteralExpr(LanguageParser.LiteralExprContext context) => Visit(context.literal());

    public override string VisitLiteral(LanguageParser.LiteralContext context)
    {
        if (context.INT_LITERAL() != null)
        {
            Console.WriteLine($"push I {context.INT_LITERAL().GetText()}");
            output.Add($"push I {context.INT_LITERAL().GetText()}");
            return "int";
        }
        if (context.FLOAT_LITERAL() != null)
        {
            Console.WriteLine($"push F {context.FLOAT_LITERAL().GetText()}");
            output.Add($"push F {context.FLOAT_LITERAL().GetText()}");
            return "float";
        }
        if (context.BOOL_LITERAL() != null)
        {
            Console.WriteLine($"push B {context.BOOL_LITERAL().GetText()}");
            output.Add($"push B {context.BOOL_LITERAL().GetText()}");
            return "bool";
        }
        if (context.STRING_LITERAL() != null)
        {
            Console.WriteLine($"push S {context.STRING_LITERAL().GetText()}");
            output.Add($"push S {context.STRING_LITERAL().GetText()}");
            return "string";
        }

        return "unknown";
    }

    public override string VisitVariableExpr(LanguageParser.VariableExprContext context)
    {
        string name = context.IDENTIFIER().GetText();
        if (symbolTable.TryGetValue(name, out string varType))
        {
            Console.WriteLine($"load {name}");
            output.Add($"load {name}");
            
            
            return varType;
        }

        return "unknown";
    }

    public override string VisitParensExpr(LanguageParser.ParensExprContext context) =>
        Visit(context.expression());

    public override string VisitAddExpr(LanguageParser.AddExprContext context)
    {
        string left = Visit(context.expression(0));
        string right = Visit(context.expression(1));
        string op = context.GetChild(1).GetText();

        // Apply itof if necessary
        if (left == "int" && right == "float")
        {
            Console.WriteLine("itof");
            output.Add("itof");
            left = "float";
        }
        else if (left == "float" && right == "int")
        {
            Console.WriteLine("itof");
            output.Add("itof");
            right = "float";
        }

        if (op == "+")
        {
            Console.WriteLine(left == "float" ? "add F" : "add I");
            output.Add(left == "float" ? "add F" : "add I");
            return left;
        }
        else if (op == ".")
        {
            Console.WriteLine("concat");
            output.Add("concat");
            return "string";
        }
        else if (op == "-")
        {
            Console.WriteLine(left == "float" ? "sub F" : "sub I");
            output.Add(left == "float" ? "sub F" : "sub I");
            return left;
        }

        return "unknown";
    }

    public override string VisitMulExpr(LanguageParser.MulExprContext context)
    {
        string left = Visit(context.expression(0));
        string right = Visit(context.expression(1));
        string op = context.GetChild(1).GetText();

        if (left == "int" && right == "float")
        {
            Console.WriteLine("itof");
            output.Add("itof");
            left = "float";
        }
        else if (left == "float" && right == "int")
        {
            Console.WriteLine("itof");
            output.Add("itof");
            right = "float";
        }

        switch (op)
        {
            case "*":
                Console.WriteLine(left == "float" ? "mul F" : "mul I");
                output.Add(left == "float" ? "mul F" : "mul I");
                return left;
            case "/":
                Console.WriteLine(left == "float" ? "div F" : "div I");
                output.Add(left == "float" ? "div F" : "div I");
                return left;
            case "%":
                Console.WriteLine("mod");
                output.Add("mod");
                return "int";
        }

        return "unknown";
    }
    
    private string GetExprType(ParserRuleContext expr)
    {

        if (expr is LanguageParser.LiteralExprContext literalExpr)
        {
            if (literalExpr.literal().INT_LITERAL() != null)
                return "int";
            if (literalExpr.literal().FLOAT_LITERAL() != null)
                return "float";
            if (literalExpr.literal().BOOL_LITERAL() != null)
                return "bool";
            if (literalExpr.literal().STRING_LITERAL() != null)
                return "string";
        }

        return expr.GetText();
    }
    public override string VisitRelExpr(LanguageParser.RelExprContext context)
    {
        
        string leftType = GetExprType(context.expression(0));
        string rightType = GetExprType(context.expression(1));
        
        Visit(context.expression(0));

        string op = context.GetChild(1).GetText();
       
        if (leftType == "int" && rightType == "float")
        {
            Console.WriteLine("itof");
            output.Add("itof");
            
            leftType = "float";
        }
        else if (leftType == "float" && rightType == "int")
        {
            Console.WriteLine("itof");
            output.Add("itof");
            rightType = "float";
        }
        
        
        Visit(context.expression(1));
        
        if (op == "<")
        {
            Console.WriteLine(leftType == "float" ? "lt F" : "lt I");
            output.Add(leftType == "float" ? "lt F" : "lt I");
        }
        else if (op == ">")
        {
            Console.WriteLine(leftType == "float" ? "gt F" : "gt I");
            output.Add(leftType == "float" ? "gt F" : "gt I");
        }

        return "bool";
    }
    public override string VisitAndExpr(LanguageParser.AndExprContext context)
    {
        string left = Visit(context.expression(0));
        string right = Visit(context.expression(1));
        Console.WriteLine("and");
        output.Add("and");
        return "bool";
    }
    
    public override string VisitOrExpr(LanguageParser.OrExprContext context)
    {
        string left = Visit(context.expression(0));
        string right = Visit(context.expression(1));
        Console.WriteLine("or");
        output.Add("or");
        return "bool";
    }
    
    public override string VisitNotExpr(LanguageParser.NotExprContext context)
    {
        string type = Visit(context.expression());
        Console.WriteLine("not");
        output.Add("not");
        return "bool";
    }
    
    public override string VisitEqExpr(LanguageParser.EqExprContext context)
    {
        string left = Visit(context.expression(0));
        string right = Visit(context.expression(1));
        string op = context.GetChild(1).GetText();

        if (left == "int" && right == "float")
        {
            Console.WriteLine("itof");
            output.Add("itof");
            left = "float";
        }
        else if (left == "float" && right == "int")
        {
            Console.WriteLine("itof");
            output.Add("itof");
            right = "float";
        }

        if(op == "==")
        {
            if(left == "string" && right == "string")
            {
                Console.WriteLine("eq S");
                output.Add("eq S");
            }
            else if (left == "int" && right == "int")
            {
                Console.WriteLine("eq I");
                output.Add("eq I");
            }
            else if (left == "float" && right == "float")
            {
                Console.WriteLine("eq F");
                output.Add("eq F");
            }
            else
            {
                Console.WriteLine("eq B");
                output.Add("eq B");
            }
        }
        if(op == "!=")
        {
            if(left == "string" && right == "string")
            {
                Console.WriteLine("eq S");
                output.Add("eq S");
            }
            else if (left == "int" && right == "int")
            {
                Console.WriteLine("eq I");
                output.Add("eq I");
            }
            else if (left == "float" && right == "float")
            {
                Console.WriteLine("eq F");
                output.Add("eq F");
            }
            else
            {
                Console.WriteLine("eq B");
                output.Add("eq B");
            }
            Console.WriteLine("not");
            output.Add("not");
        }
        return "bool";
    }

    public override string VisitUminusExpr(LanguageParser.UminusExprContext context)
    {
        string type = Visit(context.expression());
        Console.WriteLine($"uminus {(type == "float" ? "F" : "I")}");
        output.Add($"uminus {(type == "float" ? "F" : "I")}");
        return type;
    }

    public override string VisitWriteStatement(LanguageParser.WriteStatementContext context)
    {
        int count = 0;
        foreach (var expr in context.expressionList().expression())
        {
            Visit(expr);
            count++;
        }

        Console.WriteLine($"print {count}");
        output.Add($"print {count}");
        return null;
    }
    public override string VisitReadStatement(LanguageParser.ReadStatementContext context)
    {
        foreach (var id in context.variableList().IDENTIFIER())
        {
            string name = id.GetText();
            if (symbolTable.TryGetValue(name, out string type))
            {
                switch (type)
                {
                    case "int":
                        Console.WriteLine("read I");
                        output.Add("read I");
                        break;
                    case "float":
                        Console.WriteLine("read F");
                        output.Add("read F");
                        break;
                    case "string":
                        Console.WriteLine("read S");
                        output.Add("read S");
                        break;
                    case "bool":
                        Console.WriteLine("read B");
                        output.Add("read B");
                        break;
                    default:
                        break;
                }
                Console.WriteLine($"save {name}");
                output.Add($"save {name}");
            }
        }
        return null;
    }
    private int labelCounter = 0;

    private int GetNextLabel()
    {
        return labelCounter++;
    }
    
    public override string VisitIfStatement(LanguageParser.IfStatementContext context)
    {
        int elseLabel = GetNextLabel();
        int endLabel = GetNextLabel();

        Visit(context.expression());

        Console.WriteLine($"fjmp {elseLabel}");
        output.Add($"fjmp {elseLabel}");

        Visit(context.statement(0)); 

        Console.WriteLine($"jmp {endLabel}");
        output.Add($"jmp {endLabel}");

        Console.WriteLine($"label {elseLabel}");
        output.Add($"label {elseLabel}");

        if (context.statement().Length > 1)
        {
            Visit(context.statement(1)); 
        }

        Console.WriteLine($"label {endLabel}");
        output.Add($"label {endLabel}");

        return null;
    }
    
    public override string VisitWhileStatement(LanguageParser.WhileStatementContext context)
    {
        int startLabel = GetNextLabel();
        int endLabel = GetNextLabel();

        Console.WriteLine($"label {startLabel}");
        output.Add($"label {startLabel}");

        Visit(context.expression());

        Console.WriteLine($"fjmp {endLabel}");
        output.Add($"fjmp {endLabel}");

        Visit(context.statement());

        Console.WriteLine($"jmp {startLabel}");
        output.Add($"jmp {startLabel}");
        Console.WriteLine($"label {endLabel}");
        output.Add($"label {endLabel}");

        return null;
    }
    
    
}
