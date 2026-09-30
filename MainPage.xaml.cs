using System;
using Microsoft.Maui.Controls;

namespace Calculatrice
{
    public partial class MainPage : ContentPage
    {
        private string _currentExpression = "";
        private bool _useDegrees = true;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnBtnClicked(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            _currentExpression += btn.Text;
            ExpressionLabel.Text = _currentExpression;
        }

        private void OnFunctionClicked(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            _currentExpression += $"{btn.Text}(";
            ExpressionLabel.Text = _currentExpression;
        }

        private void OnClearClicked(object sender, EventArgs e)
        {
            _currentExpression = "";
            ExpressionLabel.Text = "";
            ResultLabel.Text = "0";
        }

        private void OnDeleteClicked(object sender, EventArgs e)
        {
            if (_currentExpression.Length > 0)
            {
                _currentExpression = _currentExpression.Substring(0, _currentExpression.Length - 1);
                ExpressionLabel.Text = _currentExpression;
            }
        }

        private void OnModeClicked(object sender, EventArgs e)
        {
            _useDegrees = !_useDegrees;
            ModeBtn.Text = _useDegrees ? "DEG" : "RAD";
        }

        private void OnCalculateClicked(object sender, EventArgs e)
        {
            try
            {
                double result = EvaluateExpression(_currentExpression);
                ResultLabel.Text = result.ToString("G10");
            }
            catch
            {
                ResultLabel.Text = "Erreur";
            }
        }

        // Évaluateur mathématique simple
        private double EvaluateExpression(string expr)
        {
            if (string.IsNullOrWhiteSpace(expr)) return 0;

            expr = expr.Replace("×", "*")
                       .Replace("÷", "/")
                       .Replace("π", Math.PI.ToString(System.Globalization.CultureInfo.InvariantCulture))
                       .Replace("e", Math.E.ToString(System.Globalization.CultureInfo.InvariantCulture));

            // Résolution simplifiée des fonctions scientifiques
            expr = ProcessFunctions(expr);

            var dt = new System.Data.DataTable();
            var val = dt.Compute(expr, "");
            return Convert.ToDouble(val);
        }

        private string ProcessFunctions(string expr)
        {
            // Traitement simplifié des fonctions
            while (expr.Contains("sin(")) expr = ReplaceTrig(expr, "sin", val => _useDegrees ? Math.Sin(val * Math.PI / 180.0) : Math.Sin(val));
            while (expr.Contains("cos(")) expr = ReplaceTrig(expr, "cos", val => _useDegrees ? Math.Cos(val * Math.PI / 180.0) : Math.Cos(val));
            while (expr.Contains("tan(")) expr = ReplaceTrig(expr, "tan", val => _useDegrees ? Math.Tan(val * Math.PI / 180.0) : Math.Tan(val));
            while (expr.Contains("√(")) expr = ReplaceTrig(expr, "√", val => Math.Sqrt(val));
            while (expr.Contains("log(")) expr = ReplaceTrig(expr, "log", val => Math.Log10(val));
            while (expr.Contains("ln(")) expr = ReplaceTrig(expr, "ln", val => Math.Log(val));

            return expr;
        }

        private string ReplaceTrig(string expr, string funcName, Func<double, double> func)
        {
            int start = expr.IndexOf($"{funcName}(");
            if (start == -1) return expr;

            int openParen = start + funcName.Length;
            int closeParen = expr.IndexOf(')', openParen);

            if (closeParen == -1) return expr;

            string argStr = expr.Substring(openParen + 1, closeParen - openParen - 1);
            double argVal = Convert.ToDouble(new System.Data.DataTable().Compute(argStr, ""));
            double resVal = func(argVal);

            return expr.Substring(0, start) + resVal.ToString(System.Globalization.CultureInfo.InvariantCulture) + expr.Substring(closeParen + 1);
        }
    }
}