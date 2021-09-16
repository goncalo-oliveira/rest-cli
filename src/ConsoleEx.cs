using System;
using System.Collections.Generic;
using System.Linq;

namespace RestCli
{
    public class ColorExpression
    {
        public ConsoleColor Color { get; set; }
        public Func<object, bool> Condition { get; set; }

        public static ColorExpression Create( ConsoleColor color, Func<object, bool> condition )
        {
            return new ColorExpression
            {
                Color = color,
                Condition = condition
            };
        }
    }

    public static class ConsoleEx
    {
        private static readonly ConsoleColor foregroundColor;
        //private static IEnumerable<ColorExpression> colorExpressions;

        static ConsoleEx()
        {
            foregroundColor = Console.ForegroundColor;
        }

        // public void SetColorExpressions( IEnumerable<ColorExpression> expressions )
        // {
        //     colorExpressions = expressions;
        // }

        public static void SetColor( ConsoleColor color )
        {
            Console.ForegroundColor = color;
        }

        public static void ResetColor()
        {
            Console.ForegroundColor = foregroundColor;
        }

        public static void WriteLine( IEnumerable<ColorExpression> colorExpressions
            , object expressionValue
            , string message )
        {
            if ( !( colorExpressions?.Any() == true ) )
            {
                throw new InvalidOperationException( "No color expressions were set." );
            }

            var expression = colorExpressions.Where( x => x.Condition.Invoke( expressionValue ) )
                .SingleOrDefault();

            if ( expression != null )
            {
                SetColor( expression.Color );
            }

            Console.WriteLine( message );

            ResetColor();
        }
    }
}
