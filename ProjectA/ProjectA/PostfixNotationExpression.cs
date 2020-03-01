
using System;
using System.Collections.Generic;
using System.Linq;


namespace ProjectA
{


	/// <summary>
	/// Если текущий элемент — цифра или переменная, то положим в стек значение этого числа/переменной.
	/// Если текущий элемент — открывающая скобка, то положим её в стек.
	/// Если текущий элемент — закрывающая скобка, то будем выталкивать из стека и выполнять все операции до тех пор, пока мы не извлечём открывающую скобку (т.е встречая закрывающую скобку, мы выполняем все операции, находящиеся внутри этой скобки). 
	/// Если текущий элемент — операция, то, пока на вершине стека находится операция с таким же или большим приоритетом, будем выталкивать и выполнять её.
	/// После того, как мы обработаем всю строку, в стеке операций ещё могут остаться некоторые операции, которые ещё не были вычислены, и нужно выполнить их все(т.е.действуем аналогично случаю, когда встречаем закрывающую скобку).
	/// </summary>
	public class PostfixNotationExpression
	{


		public PostfixNotationExpression()
		{
			operators = GetType().Assembly.DefinedTypes.Where(it => typeof(OperationBase).IsAssignableFrom(it)
			&& !it.IsAbstract).Select(Activator.CreateInstance).Cast<OperationBase>().ToList();

		}

		private readonly List<OperationBase> operators;


		private IEnumerable<string> Separate(string input)
		{

			var pos = 0;
			while (pos < input.Length)
			{
				var s = string.Empty + input[pos];

				if (operators.All(it => it.Name != input[pos].ToString()))
				{
					if (char.IsDigit(input[pos]))
						for (var i = pos + 1; i < input.Length && (char.IsDigit(input[i]) || input[i] == ',' || input[i] == '.'); i++)
							s += input[i];
					else if (char.IsLetter(input[pos]))
						for (var i = pos + 1; i < input.Length && (char.IsLetter(input[i]) || char.IsDigit(input[i])); i++)
							s += input[i];
				}

				yield return s;
				pos += s.Length;
			}
		}

		internal List<Node> ConvertToPostfixNotation(string input)
		{
			var outputSeparated = new List<Node>();
			var stack = new Stack<Node>();

			foreach (var c in Separate(input.Trim(' ')))
			{
				var op = operators.FirstOrDefault(it => it.Name == c);
				
				if (op != null)
				{
					if (op is Subtract && (!outputSeparated.Any() || outputSeparated.LastOrDefault() is OperationBase || stack.Count != 0))
					{
						outputSeparated.Add(new InverValue());
					}
					else if (stack.Count > 0 && !(op is LeftBracket))
					{
						if (op is RigthBracket)
						{
							var s = stack.Pop();
							while (!(s is LeftBracket))
							{
								outputSeparated.Add(s);
								s = stack.Pop();
							}
						}
						else if (op.Priority > stack.Peek().Priority)
							stack.Push(op.Clone());
						else
						{
							while (stack.Count > 0 && op.Priority <= stack.Peek().Priority)
								outputSeparated.Add(stack.Pop());
							stack.Push(op.Clone());
						}
					}
					else
						stack.Push(op.Clone());
				}
				else
				{
					outputSeparated.Add(new Value(c));
				}
			}

			if (stack.Count > 0)
				foreach (var c in stack)
					outputSeparated.Add(c);

			return outputSeparated;
		}
	}
}
