using System;
using System.Collections.Generic;
using System.IO;

public class StackInterpreter
{
    private Stack<object> stack = new Stack<object>();
    private Dictionary<string, object> variables = new Dictionary<string, object>();
    private Dictionary<string, int> labels = new Dictionary<string, int>();

    public void Execute(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            
            if (line.StartsWith("label "))
            {
                var labelName = line.Substring(6);
                labels[labelName] = i; 
            }
        }

        int index = 0; 
        while (index < lines.Length)
        {
            var line = lines[index].Trim();
            var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) { index++; continue; } 

            var instruction = parts[0].ToLower();

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
                    break; 
                case "jmp":
                    HandleJmp(parts);
                    break;
                case "fjmp":
                    HandleFjmp(parts);
                    break;
                case "print":
                    HandlePrint(parts);
                    break;
                case "read":
                    HandleRead(parts);
                    break;
                case "fopen":
                    HandleFopen(parts);
                    break;
                case "fappend":
                    HandleFappend(parts);
                    break;
                default:
                    Console.WriteLine("Unknown instruction: " + instruction);
                    break;
            }
            index++;
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
        stack.Push(b + a); 
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

       
        for (int i = 0; i < count; i++)
        {
            var value = stack.Pop();
            valuesToPrint.Add(value);
        }

        
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
    

    private void HandleJmp(string[] parts)
    {
        var labelName = parts[1];
        if (labels.ContainsKey(labelName))
        {
            var targetLine = labels[labelName];
            
            Console.WriteLine($"Jumping to label: {labelName} at line {targetLine}");
        }
        else
        {
            Console.WriteLine($"Label '{labelName}' not found.");
        }
    }
    
    private void HandleFjmp(string[] parts)
    {
        var labelName = parts[1];
        var condition = (bool)stack.Pop(); 

        if (condition)
        {
            if (labels.ContainsKey(labelName))
            {
                var targetLine = labels[labelName];
              
                Console.WriteLine($"Conditional jump to label: {labelName} at line {targetLine}");
            }
            else
            {
                Console.WriteLine($"Label '{labelName}' not found.");
            }
        }
        else
        {
            Console.WriteLine($"Condition for label '{labelName}' not met.");
        }
    }
    
    private void HandleFopen(string[] parts)
    {
        var fileName = stack.Pop().ToString().Trim('"');
        
        File.Create(fileName).Close();
        stack.Push(fileName);
    }
    
    private void HandleFappend(string[] parts)
    {
        int count = int.Parse(parts[1]);

        var items = new List<object>();
        for (int i = 0; i < count; i++)
        {
            items.Add(stack.Pop());
        }

        items.Reverse(); 

        var fileIdentifier = items[0];
        string fileName;

        if (fileIdentifier is string varName && variables.ContainsKey(varName))
        {
            fileName = variables[varName].ToString();
        }
        else
        {
            fileName = fileIdentifier.ToString();
        }

        for (int i = 1; i < items.Count; i++) 
        {
            File.AppendAllText(fileName, items[i].ToString() + Environment.NewLine);
        }

        stack.Push(fileIdentifier); 
    }
}

