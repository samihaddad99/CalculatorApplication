using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

/**
 * Author: Sami Haddad
 * Date: Jul-13-2022
 * File: CalculatorApplication (MainPage.xaml.cs)
 * Desc: A calculator application that is graphically modelled with XAML and UWP
 *      and the logic is implemented using C#.
 * Source(s): https://www.youtube.com/watch?v=xfvJSt2NMH4&ab_channel=AllTech (uwp calculator with c# and xaml)
 * #todo:
 *      - fix button collision boundary (where it registers the click [possibly something to do with using grid and stretch?])
 *      - implement different styles for the operators (+, -, /, x, =)
 *      - implement different styles for the modifiers (%, ., +/-)
 *      - implement logic for the modifiers (%, ., +/-)
 */

namespace CalculatorApplication
{
    
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // same value must be accessed from all methods
        private string _output = "";
        public string Output
        {
            get { return _output; }
            set { _output = value; }
        }

        public MainPage()
        {
            this.InitializeComponent();

            txtResult.Text = 0.ToString(); // set textbox to empty
        }

        private void AddNumberToResult(double number)
        {
            // check if the text inside the txtBlock is a number
            if (char.IsNumber(txtResult.Text.Last()))
            {
                if(txtResult.Text.Length == 1 && txtResult.Text == "0") // empty check
                {
                    txtResult.Text = string.Empty;
                }
                txtResult.Text += number;
            }
            else
            {
                if(number != 0)
                {
                    // append the number to the result
                    txtResult.Text += number;
                }
            }
        }

        enum Operation { MINUS = 1, PLUS = 2, DIV = 3, TIMES = 4, NUMBER = 5 };
        private void AddOperationToResult(Operation operation)
        {
            if (txtResult.Text.Length == 1 && txtResult.Text == "0") // empty check
            {
                txtResult.Text = String.Empty;
            }

            switch (operation)
            {
                case Operation.MINUS: txtResult.Text += "-"; break;
                case Operation.PLUS: txtResult.Text += "+"; break;
                case Operation.DIV: txtResult.Text += "/"; break;
                case Operation.TIMES: txtResult.Text += "x"; break;
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtResult.Text = 0.ToString(); // set textbox to empty
        }

        private void btn9_Click(object sender, RoutedEventArgs e)
        {
            AddNumberToResult(9);
        }

        private void btn8_Click(object sender, RoutedEventArgs e)
        {
            AddNumberToResult(8);
        }

        private void btn7_Click(object sender, RoutedEventArgs e)
        {
            AddNumberToResult(7);
        }

        private void btn6_Click(object sender, RoutedEventArgs e)
        {
            AddNumberToResult(6);
        }

        private void btn5_Click(object sender, RoutedEventArgs e)
        {
            AddNumberToResult(5);
        }

        private void btn4_Click(object sender, RoutedEventArgs e)
        {
            AddNumberToResult(4);
        }

        private void btn3_Click(object sender, RoutedEventArgs e)
        {
            AddNumberToResult(3);
        }

        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            AddNumberToResult(2);
        }

        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            AddNumberToResult(1);
        }

        private void btn0_Click(object sender, RoutedEventArgs e)
        {
            AddNumberToResult(0);
        }

        private void btnDecimal_Click(object sender, RoutedEventArgs e)
        {

        }

        private class Operand
        {
            public Operation operation = Operation.NUMBER;
            public double value = 0;

            public Operand left = null;
            public Operand right = null;
        }

        private Operand BuildTreeOperand()
        {
            Operand tree = null;

            string expression = txtResult.Text;
            if (!char.IsNumber(expression.Last()))
            {
                expression = expression.Substring(0, expression.Length - 1);
            }

            string numberStr = string.Empty;
            foreach(char c in expression.ToCharArray())
            {
                if(char.IsNumber(c) || c == '.' || numberStr == string.Empty && c == '-')
                {
                    numberStr += c;
                }
                else
                {
                    AddOperandToTree(ref tree, new Operand() { value = double.Parse(numberStr) });
                    numberStr = string.Empty;

                    Operation op = Operation.MINUS;
                    switch (c)
                    {
                        case '-': op = Operation.MINUS; break;
                        case '+': op = Operation.PLUS; break;
                        case '/': op = Operation.DIV; break;
                        case 'x': op = Operation.TIMES; break;
                    }
                    AddOperandToTree(ref tree, new Operand() { operation = op });
                }
            }
            AddOperandToTree(ref tree, new Operand() { value = double.Parse(numberStr) });

            return tree;
        }

        private void AddOperandToTree(ref Operand tree, Operand elem)
        {
            if (tree == null)
            {
                tree = elem;
            }
            else
            {
                if(elem.operation < tree.operation) // base case
                {
                    Operand auxTree = tree;
                    tree = elem;
                    elem.left = auxTree;
                }
                else
                {
                    AddOperandToTree(ref tree.right, elem); // keep recursively calling until base case is reached
                }
            }
        }

        private double Calc(Operand tree)
        {
            if(tree.left == null && tree.right == null)
            {
                return tree.value;
            }
            else
            {
                double subResult = 0;
                switch (tree.operation)
                {
                    case Operation.MINUS: subResult = Calc(tree.left) - Calc(tree.right); break;
                    case Operation.PLUS: subResult = Calc(tree.left) + Calc(tree.right); break;
                    case Operation.DIV: subResult = Calc(tree.left) / Calc(tree.right); break;
                    case Operation.TIMES: subResult = Calc(tree.left) * Calc(tree.right); break;
                }
                return subResult;
            }
        }
        private void btnEquals_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtResult.Text)) return;

            Operand tree = BuildTreeOperand();

            double value = Calc(tree);

            txtResult.Text = value.ToString();
        }

        private void btnPlus_Click(object sender, RoutedEventArgs e)
        {
            AddOperationToResult(Operation.PLUS);
        }

        private void btnMinus_Click(object sender, RoutedEventArgs e)
        {
            AddOperationToResult(Operation.MINUS);
        }

        private void btnMul_Click(object sender, RoutedEventArgs e)
        {
            AddOperationToResult(Operation.TIMES);
        }

        private void btnDiv_Click(object sender, RoutedEventArgs e)
        {
            AddOperationToResult(Operation.DIV);
        }

        private void btnPercent_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnNegate_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
