using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class MyPars
    {

        private static readonly Dictionary<char, byte> _opdict = new Dictionary<char, byte> { { '+', 1 }, { '-', 2 }, { '/', 3 }, { '*', 4 }, { '(', 0 } };

        private static readonly HashSet<char> _operators = new HashSet<char> { '+', '-', '/', '*' };

        private static double CalculateSimple(double a, double b, char op) //Выполняем нужное вычисление в зависимости от операции
        {
            double result = 0;
            switch (op)
            {
                case '+':
                    result = a + b;
                    break;
                case '-':
                    result = a - b;
                    break;
                case '*':
                    result = a * b;
                    break;
                case '/':
                    result = a / b;
                    break;
            }
            return result;
        }

        private static double StackCalc(double a, double b, char op) //Так как из стека достаем в обратном порядке, требуется инвертировать левый и правый операнды
        {
            return CalculateSimple(b, a, op);
        }

        private static bool BadBreckets(string s) //Проверяем баланс скобок в строке
        {
            int i = 0;
            foreach (var c in s)
            {
                if (c == '(')
                    i++;
                else if (c == ')')
                    i--;
            }
            return i != 0;
        }

        private static string Reverse(string s) //Обращаем строку в обратный порядок
        {
            int length = s.Length;
            var sb = new StringBuilder(s.Length, s.Length);
            for (int i = length - 1; i >= 0; i--)
                sb.Append(s[i]);
            return sb.ToString();
        }

        public static string ToRPN(string input) //Для более удобного анализа переводим в ОПН
        {
            string result = "";
            var stack = new Stack<char>();
            foreach (char c in input)
            {
                if (c == '(')
                    stack.Push(c);
                else if (c == ')')//Если встречаем закрывающую скобку, то вытряхиваем в строку все, до появления открывабщей скобки
                {
                    while (stack.Count > 0)
                    {
                        char a = stack.Pop();
                        if (a == '(') break;
                        result += " " + a;
                    }
                }
                else if (_operators.Contains(c))//Иначе если это оператор, вытряхиваем все операторы с большим приоритетом в строку, после этого засовываемся сами в стек
                {
                    result += " ";
                    while (stack.Count > 0 && _opdict[c] < _opdict[stack.Peek()])
                    {
                        result += stack.Pop() + " ";
                    }
                    stack.Push(c);
                }
                else result += c;  //Если это не скобки и не операторы, то это цифра, добавляем к выходной строке
            }
            while (stack.Count > 0) //Вытряхиваем в строку оставшиеся символы
                result += " " + stack.Pop();
            return result;
        }

        public static string ToPN(string input)
        {
            return Reverse(ToRPN(input));
        }

        public static double? Calculate(string input) //Вычисляем выражение. Если формат ввода неправильный, возвращаем null
        {
            if (BadBreckets(input) || String.IsNullOrEmpty(input))
                return null;
            var strings = ToRPN(input).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var stack = new Stack<double>(input.Length / 2);
            foreach (var s in strings)
            {
                if (_opdict.ContainsKey(s[0])) //Если оператор, то выполняем его над последними двумя элементами стека.
                {
                    if (stack.Count < 2) return null;
                    stack.Push(StackCalc(stack.Pop(), stack.Pop(), s[0]));
                }
                else //Иначе это должно быть число, если вдруг что-то другое, то возвращаем null
                {
                    double a;
                    if (!double.TryParse(s, out a)) return null;
                    stack.Push(a);
                }
            }
            return Math.Round(stack.Peek(), 15); //В стеке остается единственный элемент, значение выражения, который возвращаем
        }



    }
}
