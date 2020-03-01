using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectA
{
    public partial class Form1 : Form
    {
		
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
			
			try
			{
				var stack = new Stack<Value>();
				var parsedExpression = new PostfixNotationExpression().ConvertToPostfixNotation(textBox1.Text);
				var queue = new Queue<Node>(parsedExpression);
				var needInvert = false;
				
				while (queue.Count > 0)
				{
					var node = queue.Dequeue();
					if (!(node is OperationBase))
					{
						if (node is InverValue)
							needInvert = !needInvert;
						else
						{
							var value = node as Value;
							if (needInvert)
								value.InvertValue();
							stack.Push(value);
							needInvert = false;
						}
					}

					else
					{
						stack.Push(!(node is OperationBase) ? node as Value : (node as OperationBase).Accept(stack));
					}
				}

				if (stack.Count != 1)
					throw new Exception("Выражение составлено неверно");
				label1.Text = $"Ответ:  {stack.Pop().Val}";
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.ToString());
			}
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			
		}
		

	
	}
}
