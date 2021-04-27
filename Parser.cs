using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Method_pro
{    
    public class Parser
    {
        public const char START_ARG = '(';
        public const char END_ARG = ')';
        public const char END_LINE = '\n';

        class Cell
        {
            internal Cell(double value, char action)
            {
                Value = value;
                Action = action;
            }

            internal double Value { get; set; }
            internal char Action { get; set; }
        }

        public static double process(double x0, string data)
        {
            string expression = preprocess(data);
            int from = 0;

            return loadAndCalculate(x0, data, ref from, END_LINE);
        }

        static string preprocess(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentException("Загруженны пустые данные");
            }

            int parentheses = 0;
            StringBuilder result = new StringBuilder(data.Length);

            for (int i = 0; i < data.Length; i++)
            {
                char ch = data[i];
                switch (ch)
                {
                    case ' ':
                    case '\t':
                    case '\n': continue;
                    case END_ARG:
                        parentheses--;
                        break;
                    case START_ARG:
                        parentheses++;
                        break;
                }
                result.Append(ch);
            }

            if (parentheses != 0)
            {
                throw new ArgumentException("Неравновномерные скобки");
            }

            return result.ToString();
        }

        public static double loadAndCalculate(double x0, string data, ref int from, char to = END_LINE)
        {
            if (from >= data.Length || data[from] == to)
            {
                throw new ArgumentException("Загруженные неверные данные: " + data);
            }

            List<Cell> listToMerge = new List<Cell>(16);
            StringBuilder item = new StringBuilder();

            do
            {
                char ch = data[from++];
                if (stillCollecting(item.ToString(), ch, to))
                {
                    item.Append(ch);
                    if (from < data.Length && data[from] != to)
                    {
                        continue;
                    }
                }
                ParserFunction func = new ParserFunction(data, ref from, item.ToString(), ch);
                double value = func.getValue(x0, data, ref from);

                char action = validAction(ch) ? ch
                                              : updateAction(data, ref from, ch, to);

                listToMerge.Add(new Cell(value, action));
                item.Clear();

            } while (from < data.Length && data[from] != to);

            if (from < data.Length &&
               (data[from] == END_ARG || data[from] == to))
            {
                from++;
            }

            Cell baseCell = listToMerge[0];
            int index = 1;

            return merge(baseCell, ref index, listToMerge);
        }

        static bool stillCollecting(string item, char ch, char to)
        {
            char stopCollecting = (to == END_ARG || to == END_LINE) ?
                                   END_ARG : to;
            return (item.Length == 0 && (ch == '-' || ch == END_ARG)) ||
                  !(validAction(ch) || ch == START_ARG || ch == stopCollecting);
        }

        static bool validAction(char ch)
        {
            return ch == '*' || ch == '/' || ch == '+' || ch == '-' || ch == '^';
        }

        static char updateAction(string item, ref int from, char ch, char to)
        {
            if (from >= item.Length || item[from] == END_ARG || item[from] == to)
            {
                return END_ARG;
            }

            int index = from;
            char res = ch;
            while (!validAction(res) && index < item.Length)
            {
                res = item[index++];
            }

            from = validAction(res) ? index
                                    : index > from ? index - 1
                                                   : from;
            return res;
        }

        static double merge(Cell current, ref int index, List<Cell> listToMerge,
                     bool mergeOneOnly = false)
        {
            while (index < listToMerge.Count)
            {
                Cell next = listToMerge[index++];

                while (!canMergeCells(current, next))
                {
                    merge(next, ref index, listToMerge, true);
                }
                mergeCells(current, next);
                if (mergeOneOnly)
                {
                    return current.Value;
                }
            }

            return current.Value;
        }

        static void mergeCells(Cell leftCell, Cell rightCell)
        {
            switch (leftCell.Action)
            {
                case '^':
                    leftCell.Value = Math.Pow(leftCell.Value, rightCell.Value);
                    break;
                case '*':
                    leftCell.Value *= rightCell.Value;
                    break;
                case '/':
                    if (rightCell.Value == 0)
                    {
                        throw new ArgumentException("Деление на ноль");
                    }
                    leftCell.Value /= rightCell.Value;
                    break;
                case '+':
                    leftCell.Value += rightCell.Value;
                    break;
                case '-':
                    leftCell.Value -= rightCell.Value;
                    break;
            }
            leftCell.Action = rightCell.Action;
        }

        static bool canMergeCells(Cell leftCell, Cell rightCell)
        {
            return getPriority(leftCell.Action) >= getPriority(rightCell.Action);
        }

        static int getPriority(char action)
        {
            switch (action)
            {
                case '^': return 4;
                case '*':
                case '/': return 3;
                case '+':
                case '-': return 2;
            }
            return 0;
        }
    }

    public class ParserFunction
    {
        public ParserFunction()
        {
            m_impl = this;
        }

        internal ParserFunction(string data, ref int from, string item, char ch)
        {

            if (item.Length == 0 && ch == Parser.START_ARG)
            {
                m_impl = s_idFunction;
                return;
            }

            if (m_functions.TryGetValue(item, out m_impl))
            {
                return;
            }

            s_strtodFunction.Item = item;
            m_impl = s_strtodFunction;
        }

        public static void addFunction(string name, ParserFunction function)
        {
            m_functions[name] = function;
        }

        public double getValue(double x0, string data, ref int from)
        {
            return m_impl.evaluate(x0, data, ref from);
        }

        protected virtual double evaluate(double x0,  string data, ref int from)
        {
            return 0;
        }

        private ParserFunction m_impl;
        private static Dictionary<string, ParserFunction> m_functions = new Dictionary<string, ParserFunction>();

        private static StrtodFunction s_strtodFunction = new StrtodFunction();
        private static IdentityFunction s_idFunction = new IdentityFunction();
    }

    class StrtodFunction : ParserFunction
    {
        protected override double evaluate(double x0,  string data, ref int from)
        {
            double num;
            if (!Double.TryParse(Item, out num))
            {
                throw new ArgumentException("Could not parse token [" + Item + "]");
            }
            return num;
        }
        public string Item { private get; set; }
    }

    class IdentityFunction : ParserFunction
    {
        protected override double evaluate(double x0,  string data, ref int from)
        {
            return Parser.loadAndCalculate(x0,  data, ref from, Parser.END_ARG);
        }
    }

    class PiFunction : ParserFunction
    {
        protected override double evaluate(double x0,  string data, ref int from)
        {
            return 3.141592653589793;
        }
    }
    class ExpFunction : ParserFunction
    {
        protected override double evaluate(double x0,  string data, ref int from)
        {
            double arg = Parser.loadAndCalculate(x0, data, ref from, Parser.END_ARG);
            return Math.Exp(arg);
        }
    }
    class PowFunction : ParserFunction
    {
        protected override double evaluate(double x0, string data, ref int from)
        {
            double arg1 = Parser.loadAndCalculate(x0,  data, ref from, ',');
            double arg2 = Parser.loadAndCalculate(x0,  data, ref from, Parser.END_ARG);

            return Math.Pow(arg1, arg2);
        }
    }
    class SinFunction : ParserFunction
    {
        protected override double evaluate(double x0, string data, ref int from)
        {
            double arg = Parser.loadAndCalculate(x0,  data, ref from, Parser.END_ARG);
            return Math.Sin(arg);
        }
    }
    class CosFunction : ParserFunction
    {
        protected override double evaluate(double x0,  string data, ref int from)
        {
            double arg = Parser.loadAndCalculate(x0,  data, ref from, Parser.END_ARG);
            return Math.Cos(arg);
        }
    }
    class SqrtFunction : ParserFunction
    {
        protected override double evaluate(double x0,  string data, ref int from)
        {
            double arg = Parser.loadAndCalculate(x0,  data, ref from, Parser.END_ARG);
            return Math.Sqrt(arg);
        }
    }
    class AbsFunction : ParserFunction
    {
        protected override double evaluate(double x0,  string data, ref int from)
        {
            double arg = Parser.loadAndCalculate(x0, data, ref from, Parser.END_ARG);
            return Math.Abs(arg);
        }
    }
    class XFunction : ParserFunction
    {
        protected override double evaluate(double x0,  string data, ref int from)
        {
            return x0;
        }
    }
    
}
