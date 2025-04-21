using System;
using System.Collections.Generic;
using System.IO;

public class StackInterpreter
{
    private Stack<object> stack = new Stack<object>();
    private Dictionary<string, object> variables = new Dictionary<string, object>();

    public void Execute(string filePath)
    {
        var lines = File.ReadAllLines(filePath);

        foreach (var line in lines)
        {
            var parts = line.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) continue; // Skip empty lines

            var instruction = parts[0];

            switch (instruction.ToLower())
            {
                case "push":
                    HandlePush(parts);
                    break;
                case "pop":
                    stack.Pop();
                    break;
                case "add":
                    HandleAdd(parts);
                    break;
                case "sub":
                    HandleSub(parts);
                    break;
                case "mul":
                    HandleMul(parts);
                    break;
                case "div":
                    HandleDiv(parts);
                    break;
                case "mod":
                    HandleMod();
                    break;
                case "uminus":
                    HandleUminus(parts);
                    break;
                case "concat":
                    HandleConcat();
                    break;
                case "and":
                    HandleAnd();
                    break;
                case "or":
                    HandleOr();
                    break;
                case "gt":
                    HandleGt(parts);
                    break;
                case "lt":
                    HandleLt(parts);
                    break;
                case "eq":
                    HandleEq(parts);
                    break;
                case "not":
                    HandleNot();
                    break;
                case "itof":
                    HandleItof();
                    break;
                case "load":
                    HandleLoad(parts);
                    break;
                case "save":
                    HandleSave(parts);
                    break;
                case "label":
                    break; // Labels are only used for jumping, so we ignore them for now
                case "jmp":
                    break; // Jump handling is omitted in this example
                case "fjmp":
                    break; // Conditional jump handling is omitted in this example
                case "print":
                    HandlePrint(parts);
                    break;
                case "read":
                    HandleRead(parts);
                    break;
                default:
                    Console.WriteLine("Unknown instruction: " + instruction);
                    break;
            }
        }
    }

    private void HandlePush(string[] parts)
    {
        var type = parts[1];
        var value = string.Join(" ", parts, 2, parts.Length - 2);

        switch (type.ToLower())
        {
            case "i":
                stack.Push(int.Parse(value));
                break;
            case "f":
                // Use InvariantCulture to avoid locale-related issues
                stack.Push(float.Parse(value, System.Globalization.CultureInfo.InvariantCulture));
                break;
            case "s":
                stack.Push(value);
                break;
            case "b":
                stack.Push(bool.Parse(value));
                break;
            default:
                Console.WriteLine("Unknown push type: " + type);
                break;
        }
    }

    private void HandleAdd(string[] parts)
    {
        var type = parts[1];
        var a = stack.Pop();
        var b = stack.Pop();

        if (type == "I")
        {
            stack.Push((int)b + (int)a);
        }
        else if (type == "F")
        {
            stack.Push((float)b + (float)a);
        }
    }

    private void HandleSub(string[] parts)
    {
        var type = parts[1];
        var a = stack.Pop();
        var b = stack.Pop();

        if (type == "I")
        {
            stack.Push((int)b - (int)a);
        }
        else if (type == "F")
        {
            stack.Push((float)b - (float)a);
        }
    }

    private void HandleMul(string[] parts)
    {
        var type = parts[1];
        var a = stack.Pop();
        var b = stack.Pop();

        if (type == "I")
        {
            stack.Push((int)b * (int)a);
        }
        else if (type == "F")
        {
            stack.Push((float)b * (float)a);
        }
    }

    private void HandleDiv(string[] parts)
    {
        var type = parts[1];
        var a = stack.Pop();
        var b = stack.Pop();

        if (type == "I")
        {
            stack.Push((int)b / (int)a);
        }
        else if (type == "F")
        {
            stack.Push((float)b / (float)a);
        }
    }

    private void HandleMod()
    {
        var a = stack.Pop();
        var b = stack.Pop();
        stack.Push((int)b % (int)a);
    }

    private void HandleUminus(string[] parts)
    {
        var type = parts[1];
        var a = stack.Pop();

        if (type == "I")
        {
            stack.Push(-(int)a);
        }
        else if (type == "F")
        {
            stack.Push(-(float)a);
        }
    }

    private void HandleConcat()
    {
        var a = stack.Pop().ToString();
        var b = stack.Pop().ToString();
        stack.Push(b + a); // String concatenation
    }

    private void HandleAnd()
    {
        var a = (bool)stack.Pop();
        var b = (bool)stack.Pop();
        stack.Push(b && a);
    }

    private void HandleOr()
    {
        var a = (bool)stack.Pop();
        var b = (bool)stack.Pop();
        stack.Push(b || a);
    }

    private void HandleGt(string[] parts)
    {
        var type = parts[1];
        var a = stack.Pop();
        var b = stack.Pop();

        if (type == "I")
        {
            stack.Push((int)b > (int)a);
        }
        else if (type == "F")
        {
            stack.Push((float)b > (float)a);
        }
    }

    private void HandleLt(string[] parts)
    {
        var type = parts[1];
        var a = stack.Pop();
        var b = stack.Pop();

        if (type == "I")
        {
            stack.Push((int)b < (int)a);
        }
        else if (type == "F")
        {
            stack.Push((float)b < (float)a);
        }
    }

    private void HandleEq(string[] parts)
    {
        var type = parts[1];
        var a = stack.Pop();
        var b = stack.Pop();

        if (type == "I")
        {
            stack.Push((int)b == (int)a);
        }
        else if (type == "F")
        {
            stack.Push((float)b == (float)a);
        }
        else if (type == "S")
        {
            stack.Push((string)b == (string)a);
        }
    }

    private void HandleNot()
    {
        var a = (bool)stack.Pop();
        stack.Push(!a);
    }

    private void HandleItof()
    {
        var a = (int)stack.Pop();
        stack.Push((float)a);
    }

    private void HandleLoad(string[] parts)
    {
        var varName = parts[1];
        stack.Push(variables[varName]);
    }

    private void HandleSave(string[] parts)
    {
        var varName = parts[1];
        var value = stack.Pop();
        variables[varName] = value;
    }

    private void HandlePrint(string[] parts)
    {
        var count = int.Parse(parts[1]);
        var valuesToPrint = new List<object>();

        // Collect the values from the stack into a list
        for (int i = 0; i < count; i++)
        {
            var value = stack.Pop();
            valuesToPrint.Add(value);
        }

        // Print the values in reverse order
        valuesToPrint.Reverse();
        foreach (var value in valuesToPrint)
        {
            Console.Write(value + " ");
        }
        Console.WriteLine();
    }

    private void HandleRead(string[] parts)
    {
        var type = parts[1];
        string input = Console.ReadLine();

        switch (type.ToLower())
        {
            case "i":
                stack.Push(int.Parse(input));
                break;
            case "f":
                stack.Push(float.Parse(input));
                break;
            case "s":
                stack.Push(input);
                break;
            case "b":
                stack.Push(bool.Parse(input));
                break;
            default:
                Console.WriteLine("Unknown read type: " + type);
                break;
        }
    }
}

