using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
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
 * Last Modified: Jul-18-2022
 * File: CalculatorApplication (MainPage.xaml.cs)
 * Desc: A calculator application that is graphically modelled with XAML and UWP
 *      and the logic is implemented using C#.
 * As adapted from: https://www.youtube.com/watch?v=xfvJSt2NMH4&ab_channel=AllTech (uwp calculator with c# and xaml)
 * Added Features: 
 *      - Improved styling
 *      - Percent Functionality
 *      - Decimal Functionality
 *      - Negate Functionality
 *      - Integrated All NumberPad Features (NumPad1-9, '+', '-', '/', '*', '.', and 'Enter' as '=')
 *      - Integrated Keydown features (C to clear, Backspace to delete from the right)
 *  #TODO:
 *      - integrate More keys (Shift+'5' = '%', Shift+'8' = '*') (tried/failed with method IsShiftKeyPressed)
 *              - Implement (- {unassigned}, / {unassigned}, = {unassigned}, Shift+'=' {unnassigned: results in '+'}
 *                  reference : https://docs.microsoft.com/en-us/uwp/api/windows.system.virtualkey?view=winrt-22621
 *      - integrate better button 'click' registration (why is this happening?)
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

        enum Operation
        { 
            MINUS = 1, 
            PLUS = 2, 
            DIV = 3, 
            TIMES = 4, 
            NUMBER = 5, 
            PERCENT = 6,
            DECIMAL = 7,
            NEGATE = 8
        };
            
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
                case Operation.PERCENT: txtResult.Text += "%"; break;
                case Operation.DECIMAL: txtResult.Text += "."; break;
                case Operation.NEGATE: txtResult.Text += '_'; break; 
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
            AddOperationToResult(Operation.DECIMAL);
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
                        case '.': op = Operation.DECIMAL; break;
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
            SetEquals();
        }

        void SetEquals()
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
            ToPercent();
        }

        private void btnNegate_Click(object sender, RoutedEventArgs e)
        {
            if (txtResult.Text.Substring(0, 1).Equals("0"))
            {
                // do nothing
            }
            else
            {
                String subtext = txtResult.Text.Substring(0, 1);
                if (subtext.Equals("-")) // if number is already negative, then make positive
                {
                    int strlen = txtResult.Text.Length;
                    txtResult.Text = txtResult.Text.Substring(1, strlen - 1);
                }
                else
                    txtResult.Text = "-" + txtResult.Text;
            }
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case VirtualKey.NumberPad0: AddNumberToResult(0); break;
                case VirtualKey.NumberPad1: AddNumberToResult(1); break;
                case VirtualKey.NumberPad2: AddNumberToResult(2); break;
                case VirtualKey.NumberPad3: AddNumberToResult(3); break;
                case VirtualKey.NumberPad4: AddNumberToResult(4); break;
                case VirtualKey.NumberPad5: AddNumberToResult(5); break;
                case VirtualKey.NumberPad6: AddNumberToResult(6); break;
                case VirtualKey.NumberPad7: AddNumberToResult(7); break;
                case VirtualKey.NumberPad8: AddNumberToResult(8); break;
                case VirtualKey.NumberPad9: AddNumberToResult(9); break;
                case VirtualKey.Number0: AddNumberToResult(0); break;
                case VirtualKey.Number1: AddNumberToResult(1); break;
                case VirtualKey.Number2: AddNumberToResult(2); break;
                case VirtualKey.Number3: AddNumberToResult(3); break;
                case VirtualKey.Number4: AddNumberToResult(4); break;
                case VirtualKey.Number5: AddNumberToResult(5); break;
                case VirtualKey.Number6: AddNumberToResult(6); break;
                case VirtualKey.Number7: AddNumberToResult(7); break;
                case VirtualKey.Number8: AddNumberToResult(8); break;
                case VirtualKey.Number9: AddNumberToResult(9); break;
                case VirtualKey.C: ClearText(); break; // C to clear
                case VirtualKey.Back: DeleteLast(); break;
                /* NumberPad Operations */
                case VirtualKey.Add: AddOperationToResult(Operation.PLUS); break;
                case VirtualKey.Subtract: AddOperationToResult(Operation.MINUS); break;
                case VirtualKey.Divide: AddOperationToResult(Operation.DIV); break;
                case VirtualKey.Multiply: AddOperationToResult(Operation.TIMES); break;
                case VirtualKey.Enter: SetEquals(); break;
                case VirtualKey.Decimal: AddOperationToResult(Operation.DECIMAL); break;

                default:
                    // trying to implement shift+normal keyboard buttons (for non-numpad keyboards)
                    if (IsShiftKeyPressed())
                    {
                        switch (e.Key)
                        {
                            case VirtualKey.Number5: ToPercent(); break;
                            case VirtualKey.Number8: AddOperationToResult(Operation.TIMES); break;
                        }
                    }; break;

            }
        }

        /*
         * ToPercent
         * Converts whatever number in the TextBox to its percent value
         */
        void ToPercent()
        {
            txtResult.Text = (Convert.ToDouble(txtResult.Text) / 100).ToString();
        }
        /*
         * 
         * DeleteLast
         * Deletes the last string in the TextBox if the string length is more than 1
         *      if string length = 1 or 0, then will clear the TextBox (replace whatever number/symbol with '0')
         */
        void DeleteLast()
        {
            String curr = txtResult.Text;
            int strlen = curr.Length;
            if(strlen == 0 || strlen == 1) // do nothing
            {
                ClearText();
            }
            else
            {
                curr = curr.Substring(0, strlen - 1);
                txtResult.Text = curr;
            }
            // else do nth
        }

        /**
         * ClearText
         *      sets the text to '0'
         */
        void ClearText()
        {
            txtResult.Text = "0";
        }

        /**
         * IsShiftKeyPressed()
         * returns true if a shift key is pressed (and held down), and false otherwise
         * Adapted from: https://docs.microsoft.com/en-us/windows/apps/design/input/keyboard-events
         */
        private static bool IsShiftKeyPressed()
        {
            var leftShiftState = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.LeftShift);
            var rightShiftState = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.RightShift);
            // var shiftState = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Shift);
            return (leftShiftState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down && (rightShiftState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DeleteLast();
        }
    }
}
