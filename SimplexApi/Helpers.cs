using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimplexApi.Models;

namespace SimplexApi
{
    public static class Helpers
    {
        /// <summary>
        /// Return the text that reference the RestrictionType
        /// </summary>
        public static string GetRestrictionTypeString(RestrictionType restType)
        {
            switch (restType)
            {
                case RestrictionType.EqualTo:
                    return "=";
                case RestrictionType.LessThan:
                    return "<=";
                case RestrictionType.MoreThan:
                    return ">=";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Get the number string
        /// </summary>
        public static string GetString(this decimal number)
        {
            if (number.IsNegative())
                return " - " + number * -1;
            return " + " + number;
        }

        /// <summary>
        /// Check if is a negative number
        /// </summary>
        public static bool IsNegative(this decimal number)
        {
            return number < 0;
        }

        /// <summary>
        /// Superscript the numbers in a text.
        /// </summary>
        public static string SubscriptNumber(this string textNumber)
        {
            string finalString = "";
            foreach (var letter in textNumber)
            {
                switch (letter)
                {
                    case '0':
                        finalString += "₀";
                        break;
                    case '1':
                        finalString += "₁";
                        break;
                    case '2':
                        finalString += "₂";
                        break;
                    case '3':
                        finalString += "₃";
                        break;
                    case '4':
                        finalString += "₄";
                        break;
                    case '5':
                        finalString += "₅";
                        break;
                    case '6':
                        finalString += "₆";
                        break;
                    case '7':
                        finalString += "₇";
                        break;
                    case '8':
                        finalString += "₈";
                        break;
                    case '9':
                        finalString += "₉";
                        break;
                    default:
                        finalString += letter;
                        break;
                }
            }
            return finalString;
        }
    }
}