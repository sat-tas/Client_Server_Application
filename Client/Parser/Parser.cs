using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Parser
{
    public class Parser
    {

        //Используюется для первичного разбиения строки
        List<string> funcArray = new List<string>();
        //Используется для вторичного разбиения строки
        List<List<string>> funcArrI = new List<List<string>>();
        //Поддеживаемые операции
        string operatorsTemplates = "+-*/^";
        //Итоговый список операций
        List<List<myStruct>> parssed = new List<List<myStruct>>();
        //Значения первичных функций
        double[] resultArr;
        //Значения фторичных функций
        double[] midArray;

        //Массив фунций
        Arg1Func arg1Func;
        //Словарь, используется для сопостнавления функции и её номера в массиве
        Dictionary<string, int> funcs = new Dictionary<string, int> {
                                                                            { "sin", 0 },
                                                                            { "cos", 1 },
                                                                            { "tg", 2 },
                                                                            { "ctg", 3 },
                                                                            { "ln", 4 } };


        //Аналог указателя на функцию
        public delegate double Arg1Func(double x);

        //Добавление функций в массив
        public void Init()
        {
            arg1Func += Math.Sin;
            arg1Func += Math.Cos;
            arg1Func += Math.Tan;
            arg1Func += Ctan;
               arg1Func += Math.Log;
        }

        //функция катангенса
        public static double Ctan(double x)
        {
            return 1.0 / Math.Tan(x);
        }

        //Структура описывает опирацию
        public struct myStruct
        {
            //Что сделать(знак)
            public string op;
            //Первый аргумент
            public string arg1;
            //Второй аргумент
            public string arg2;
        }

        public Parser(string input)
        {
            Init();
            input += "+0";
            string replace = "func";
            FindArg(ref input, funcArray, replace);
            FindOP(ref input, '^', funcArray, replace);
            FindOP(ref input, '*', funcArray, replace);
            FindOP(ref input, '/', funcArray, replace);
            FindOP(ref input, '-', funcArray, replace);
            FindOP(ref input, '+', funcArray, replace);


            replace = "mfunc";
            for (int i = 0; i < funcArray.Count; i++)
            {
                string func = funcArray[i];
                int result;
                funcArrI.Add(new List<string>());
                parssed.Add(new List<myStruct>());

                if (Int32.TryParse(func, out result) || func == "x")
                {
                    myStruct my = new myStruct();
                    my.arg1 = func;
                    parssed[i].Add(my);
                }
                else
                {
                    FindArg(ref func, funcArrI[i], replace);
                    FindOP(ref func, '^', funcArrI[i], replace);
                    FindOP(ref func, '*', funcArrI[i], replace);
                    FindOP(ref func, '/', funcArrI[i], replace);
                    FindOP(ref func, '+', funcArrI[i], replace);
                    FindOP(ref func, '-', funcArrI[i], replace);

                    Regex argument = new Regex(@"([\+\^\*\-\/])", RegexOptions.Compiled);
                    for (int j = 0; j < funcArrI[i].Count; j++)
                    {
                        string[] s = argument.Split(funcArrI[i][j]);
                        myStruct my = new myStruct();
                        if (s.Length == 1)
                        {
                            my.arg1 = s[0];
                        }
                        else
                        {
                            my.arg1 = s[0];
                            my.op = s[1];
                            my.arg2 = s[2];
                        }
                        parssed[i].Add(my);
                    }
                }
            }
        }


        public double Calculate(double x)
        {
            int len = parssed.Count;
            resultArr = new double[len];
            for (int i = 0; i < len; i++)
            {
                midArray = new double[parssed[i].Count];
                for (int j = 0; j < parssed[i].Count; j++)
                {
                    double arg1 = 0;
                    double arg2 = 0;

                    ParsArg(parssed[i][j].arg1, x,out arg1);
                    if (parssed[i][j].arg2 == null || parssed[i][j].op == null)
                    {
                        midArray[j] = arg1;
                        continue;
                    }
                    ParsArg(parssed[i][j].arg2, x, out arg2);

                    switch (parssed[i][j].op)
                    {
                        case "+":
                            {
                                midArray[j] = arg1 + arg2;
                                break;
                            }
                        case "-":
                            {
                                midArray[j] = arg1 - arg2;
                                break;
                            }
                        case "*":
                            {
                                midArray[j] = arg1 * arg2;
                                break;
                            }
                        case "/":
                            {
                                midArray[j] = arg1 / arg2;
                                break;
                            }
                        case "^":
                            {
                                midArray[j] = Math.Pow(arg1, arg2);
                                break;
                            }
                        default:
                            break;
                    }
                }
                resultArr[i] = midArray[midArray.Length - 1];
            }
            return resultArr[resultArr.Length - 1];
        }   

        private void ParsArg(string input,double defaultValue,out double arg)
        {

            if (double.TryParse(input, out arg))
            {

            }
            else if (input == "x")
            {
                arg = defaultValue;
            }
            else
            {
                Regex argument = new Regex(@"(?<MathFunc>\w*)\[(?<FuncArg>\D*)(?<FuncId>\d*)\]", RegexOptions.Compiled);
                MatchCollection match = argument.Matches(input);
                GroupCollection groups = match[0].Groups;
                if (groups["MathFunc"].Value != "")
                {
                    int funcid;
                    funcs.TryGetValue(groups["MathFunc"].Value,out funcid);
                    double prewarg;
                    int id = Int32.Parse(groups["FuncId"].Value);
                    if (groups["FuncArg"].Value == "func")
                        prewarg = resultArr[id];
                    else
                        prewarg = midArray[id];
                    var func = arg1Func.GetInvocationList();
                    arg = (double)func[funcid].DynamicInvoke(prewarg);
                }
                else
                {
                    int id = Int32.Parse(groups["FuncId"].Value);
                    if (groups["FuncArg"].Value == "func")
                        arg = resultArr[id];
                    else
                        arg = midArray[id];

                }
            }

        }

        private void FindOP(ref string input, char operation, List<string> funcArr, string replace)
        {
            int nom = funcArr.Count - 1;
            while (input.IndexOf(operation) != -1)
            {
                int mid = input.IndexOf(operation);
                int first = mid - 1;
                int last = mid + 1;
                while (first > 0 && !operatorsTemplates.Contains(input[first--].ToString())) ;
                while (last < input.Length && !operatorsTemplates.Contains(input[last++].ToString())) ;
                if (first != 0)
                    first += 2;
                if (last != input.Length)
                    last -= 1;
                string arg = input.Substring(first, last - first);
                funcArr.Add(arg);
                input = input.Remove(first, last - first);
                input = input.Insert(first, $"[{replace}{++nom}]");
            }

        }

        private static void FindArg(ref string input, List<string> funcArr, string replace)
        {
            int nom = funcArr.Count;
            while (input.IndexOf('(') != -1)
            {
                int first = input.IndexOf('(');
                int last = first + 1;
                if (first != -1)
                {
                    int count = 1;
                    string arg;

                    while (count > 0)
                    {
                        if (input[last] == ')')
                            count--;
                        else if (input[last] == '(')
                            count++;
                        last++;
                    }

                    arg = input.Substring(first + 1, last - first - 2);
                    funcArr.Add(arg);
                    input = input.Remove(first, last - first);
                    input = input.Insert(first, $"[{replace}{nom++}]");
                }
            }
        }

    }
}
