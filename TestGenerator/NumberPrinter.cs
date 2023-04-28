using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGenerator
{
	public static class NumberPrinter
	{
		private static readonly string[] Base =
		{
			"",
			"one",
			"two",
			"three",
			"four",
			"five",
			"six",
			"seven",
			"eight",
			"nine",
			"ten",
			"eleven",
			"twelve",
			"thirteen",
			"fourteen",
			"fifteen",
			"sixteen",
			"seventeen",
			"eighteen",
			"nineteen"
		};

		private static readonly string[] Tens =
		{
			"",
			"",
			"twenty",
			"thirty",
			"forty",
			"fifty",
			"sixty",
			"seventy",
			"eighty",
			"ninety"
		};

		private static string PrintTens(int number)
		{
			if(number < 20)
			{
				return Base[number];
			}
			else
			{
				int tens = number / 10;
				int ones = number % 10;
				if(ones == 0)
				{
					return Tens[tens];
				}
				else return $"{Tens[tens]} {Base[ones]}";
			}
		}

		private static string PrintHundreds(int number)
		{
			string h = Base[number / 100];
			string t = PrintTens(number % 100);
			if(String.IsNullOrEmpty(h))
			{
				return t;
			}
			else if(String.IsNullOrEmpty(t))
			{
				return $"{h} hundred";
			}
			else
			{
				return $"{h} hundred and {t}";
			}
		}

		private static int getMagnitude(int number)
		{
			if (number >= 1000000000)
			{
				return 1000000000;
			}
			else if (number >= 1000000)
			{
				return 1000000;
			}
			else if (number >= 1000)
			{
				return 1000;
			}
			else return 1;
		}

		private static string getModifier(int number)
		{
			if (number >= 1000000000)
			{
				return "billion";
			}
			else if (number >= 1000000)
			{
				return "million";
			}
			else if (number >= 1000)
			{
				return "thousand";
			}
			else return "";
		}

		public static string PrintNumber(this int number)
		{

			int mag = 1;
			int newnumber = number;
			List<string> parts = new List<string>();
			do
			{
				mag = getMagnitude(newnumber);
				string mod = getModifier(newnumber);
				int part = newnumber / mag;
				newnumber = newnumber % mag;
				string spart = PrintHundreds(part);
				if(!String.IsNullOrEmpty(spart))
				{
					parts.Add($"{spart} {mod}".Trim());
				}
			} while (mag > 1);
			if (parts.Count > 0)
			{
				return String.Join(", ", parts);
			}
			else return "zero";

		}
	}
}
