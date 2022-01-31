using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace MultiApp9000
{
	static class Program
	{
		private const int ER_OPTION_OOB = 1;

		private const int FN_EXIT = 0;
		private const int FN_HELLO = 1;
		private const int FN_NAME = 2;
		private const int FN_COLOR = 3;
		private const int FN_DATE = 4;
		private const int FN_COMPARE = 5;
		private const int FN_GUESS = 6;
		private const int FN_WRITEF = 7;
		private const int FN_READF = 8;
		private const int FN_MATH = 9;
		private const int FN_MULTI = 10;
		private const int FN_CHAR = 11;
		private const int FN_MENY = 12;

		private const string RETURN = "ENTER";

		private const string FILE_PATH = @"\MultiApp9000_save.txt";

		private static readonly ConsoleKey[][] FN_KEYS_ALLOWED = new ConsoleKey[][]
		{
			Array.Empty<ConsoleKey>(),
			Array.Empty<ConsoleKey>(),
			new ConsoleKey[] { ConsoleKey.Backspace, ConsoleKey.Enter, ConsoleKey.A},
			new ConsoleKey[] { ConsoleKey.Enter, ConsoleKey.UpArrow, ConsoleKey.DownArrow},
			Array.Empty<ConsoleKey>(),
			new ConsoleKey[] { ConsoleKey.Enter, ConsoleKey.D0, ConsoleKey.OemPeriod, ConsoleKey.OemComma},
			new ConsoleKey[] { ConsoleKey.Enter, ConsoleKey.D0},
			Array.Empty<ConsoleKey>(),
			Array.Empty<ConsoleKey>(),
			new ConsoleKey[] { ConsoleKey.Enter, ConsoleKey.D0, ConsoleKey.OemPeriod, ConsoleKey.OemComma},
			Array.Empty<ConsoleKey>(),
			Array.Empty<ConsoleKey>(),
			new ConsoleKey[] { ConsoleKey.Backspace, ConsoleKey.Enter, ConsoleKey.D0, ConsoleKey.A, ConsoleKey.UpArrow, ConsoleKey.DownArrow}
		};
		private static readonly Func<int>[] functions = { Exit, Hello, NameReg, ColorChange, Date, Compare, GuessNumber, WriteFile, ReadFile, NumberMath, MultiTable, CreateCharacter, Meny };
		private static int functionState = FN_MENY;
		private static readonly String[] options =
		{
			"Hello World (Valfritt meddelande -> \"# meddelande\")",
			"Registrera namn",
			"Ändra färg på consolen",
			"Dagens datum",
			"Undersök största värdet",
			"Gissa talet!",
			"Spara text i fil",
			"Läs text från en fil",
			"Applicera matte på ett tal",
			"Multiplikationstabeller",
			"Skapa karaktär",
			"Exit"
		};
		private static int optN = functions.Length;
		private static int offset;
		private static int selection = options.Length;
		private static String[] inputText = Array.Empty<String>();
		private static ConsoleColor BG_DEFAULT = Console.BackgroundColor;
		private static ConsoleColor FG_DEFAULT = Console.ForegroundColor;
		private static ConsoleColor bg_c = Console.BackgroundColor;
		private static ConsoleColor fg_c = Console.ForegroundColor;


		private static String helloMessage = "Hello World!";
		private static String forename;
		private static String surename;
		private static String middlenames;
		private static ConsoleColor color;
		private static int[][] multiTable;

		private static List<Character> characters;

		private static void Main(string[] args)
		{
			if (!Init())
			{
				Console.WriteLine("Något är allvarligt fel, säkerställ att detta program körs i korrekt universum.");
				Console.ReadKey();
				Environment.Exit(10);
			}

			bool update = true;
			while (true)
			{
				if (update)
				{
					Console.Clear();

					if (update = ChangeState(functions[functionState]()))
					{
						continue;
					}
				}

				update = KeyInput(Console.ReadKey(true));
			}
		}

		private static bool Init()
		{
			Console.WriteLine("Bleep...");
			System.Threading.Thread.Sleep(500);
			if (true != true) { return false; }

			Console.WriteLine("Bloop...");
			System.Threading.Thread.Sleep(500);
			if (1 + 1 != 2) { return false; }

			Console.WriteLine("\n\nLoading...");
			System.Threading.Thread.Sleep(1000);
			if (false != false) { return false; }

			Console.Clear();
			return true;
		}

		private static bool KeyInput(ConsoleKeyInfo keypress)
		{
			if (!FnUsesKey(keypress.Key))
			{
				return false;
			}
			switch ((int)keypress.Key)
			{
				case ((int)ConsoleKey.UpArrow):
					if (selection > 0) { selection--; break; }
					return false;
				case ((int)ConsoleKey.DownArrow):
					if (selection < optN - 1) { selection++; break; }
					return false;
				case ((int)ConsoleKey.Enter):
					return ChangeState(ParseInput(RETURN).Item2);
				case > 47 and < 91:
					return ChangeState(ParseInput(ReadInput(keypress)).Item2);
				default:
					return false;
			}
			return true;
		}

		private static bool FnUsesKey(ConsoleKey keypress)
		{
			switch ((int)keypress)
			{
				case > 47 and < 58:
					keypress = ConsoleKey.D0;
					break;
				case > 64 and < 90:
					keypress = ConsoleKey.A;
					break;
				default:
					break;
			}

			return Array.Exists(FN_KEYS_ALLOWED[functionState], key => key == keypress);
		}

		private static String ReadInput(ConsoleKeyInfo keypress)
		{
			Console.Write($"\b \b{keypress.KeyChar}");
			StringBuilder input = new StringBuilder().Append(keypress.KeyChar);

			ConsoleKeyInfo newPress;

			while ((newPress = Console.ReadKey()).Key != ConsoleKey.Enter)
			{
				if (newPress.Key == ConsoleKey.Backspace)
				{
					if (input.Length > 0)
					{
						Console.Write("\b \b");
						input.Remove(input.Length - 1, 1);
					}
					continue;
				}
				input.Append(newPress.KeyChar);
			}

			//Console.WriteLine(input.ToString());
			//Console.ReadKey();
			return input.ToString();
		}

		private static Tuple<String[], int> ParseInput(String input)
		{
			return ParseInput(input, false);
		}

		private static Tuple<String[], int> ParseInput(String input, bool stripNumbers)
		{
			if (input == RETURN)
			{
				inputText = Array.Empty<String>();
				return Tuple.Create(inputText, TranslateSelToFn());
			}

			if (stripNumbers)
			{
				input = Regex.Replace(input, @"[\d]", "");
			}

			List<String> words = new();

			foreach (String s in input.Trim().Split(' '))
			{
				if (s.Length != 0)
				{
					words.Add(s);
				}
			}

			if (words.Count == 0)
			{
				inputText = Array.Empty<String>();
				return Tuple.Create(inputText, -1);
			}
			else if (functionState == FN_MENY)
			{
				bool isTextFnCall = false;
				foreach (char c in words[0])
				{
					if (!(isTextFnCall = Char.IsDigit(c)))
					{
						break;
					}
				}

				if (isTextFnCall)
				{
					int i = int.Parse(words[0]);
					words.RemoveAt(0);
					if (i < 0 || i >= options.Length)
					{
						Error(ER_OPTION_OOB);
					}
					inputText = words.ToArray();
					return Tuple.Create(inputText, i);
				}
			}
			inputText = words.ToArray();
			return Tuple.Create(inputText, TranslateSelToFn());
		}

		private static void ReadNumber(StringBuilder inputNumber)
		{
			ConsoleKeyInfo keyInfo;
			while ((keyInfo = Console.ReadKey(true)).Key != ConsoleKey.Enter)
			{
				if (FnUsesKey(keyInfo.Key))
				{
					Console.Write(keyInfo.KeyChar);
					inputNumber.Append(keyInfo.KeyChar);
				}
			}
			if (FnUsesKey(ConsoleKey.OemComma))
			{
				inputNumber.Replace('.', ',');
				bool hasComma = false;
				for (int c = 0; c < inputNumber.Length; c++)
				{
					if (inputNumber[c] == ',')
					{
						if (hasComma)
						{
							inputNumber.Replace(",", "", c, inputNumber.Length - c);
							break;
						}
						else
						{
							hasComma = true;
						}
					}
				}
			}
		}

		private static int TranslateSelToFn() => selection + 1 == options.Length ? FN_EXIT : selection + 1 > options.Length ? FN_MENY : selection + 1;

		private static void SwapColors()
		{
			Console.BackgroundColor = fg_c;
			Console.ForegroundColor = bg_c;
			fg_c = Console.ForegroundColor;
			bg_c = Console.BackgroundColor;
		}

		private static void SetColors(ConsoleColor fg, ConsoleColor bg)
		{
			Console.ForegroundColor = fg_c = fg;
			Console.BackgroundColor = bg_c = bg;
		}

		private static void ReturnMessage()
		{
			optN = functions.Length;
			selection = options.Length;
			System.Threading.Thread.Sleep(1000);
			Console.WriteLine("\nTryck valfri tangent för att återgå till meny...");
			Console.ReadKey(true);
			Console.WriteLine();
		}

		private static bool ChangeState(int fNum)
		{
			if (fNum < optN && fNum >= 0)
			{
				if (fNum != functionState)
				{
					functionState = fNum;
					return true;
				}
			}
			else if (fNum == -1)
			{
				return true;
			}
			return false;
		}

		private static int Meny()
		{
			optN = options.Length;
			//selection = optN;
			Console.WriteLine("Välkommen till MultiApp9000\n\nVälj funktion med piltangenterna och enter eller valnummer\n\n\n");

			for (int i = 0; i <= optN; i++)
			{
				string optionNum = i == optN - 1 ? "0" : (i + 1).ToString();

				if (selection == i)
				{
					SwapColors();

					if (selection == optN) { Console.WriteLine("  "); SwapColors(); }
					else
					{
						Console.Write($"{optionNum} {options[i]}".Substring(0, 2));
						SwapColors();
						Console.WriteLine($"{optionNum} {options[i]}"[2..]);
					}
				}
				else if (i < optN) { Console.WriteLine($"{optionNum} {options[i]}"); }
			}
			return FN_MENY;
		}

		private static int Hello()
		{
			if (inputText != Array.Empty<String>())
			{
				helloMessage = new StringBuilder().Append(inputText).ToString();
			}
			Console.WriteLine(helloMessage);

			ReturnMessage();
			inputText = Array.Empty<String>();
			return FN_MENY;
		}

		private static int NameReg()
		{
			if (String.IsNullOrEmpty(forename))
			{
				List<String> names = new();
				Console.WriteLine("Grattis till ditt nya ägarskap av MultiApp9000!");
				if (inputText.Length != 0)
				{
					if (inputText.Length == 1)
					{
						names.Add(inputText[0]);
						Console.WriteLine("Mellannamn och efternamn krävs för effektiv spårning i cyberspace:");
						inputText = MissingNames();
						names.AddRange(inputText);
					}
					else
					{
						names.AddRange(inputText);
					}
					ApplyNames(names);
				}
				else
				{
					Console.WriteLine("\n\tVänligen registrera ditt fulla namn för förankring och evig spårning i cyberspace:");
					names.AddRange(MissingNames());
					ApplyNames(names);
				}
			}
			Console.WriteLine(new StringBuilder().Append("Hej och välkommen ").Append(forename).Append(middlenames == String.Empty ? ' ' : ' ' + middlenames + ' ').Append(surename).Append('!'));

			ReturnMessage();
			inputText = Array.Empty<String>();
			return FN_MENY;
		}

		private static String[] MissingNames()
		{
			String[] names;

			while ((names = ParseInput(ReadInput(Console.ReadKey(true)), true).Item1).Length == 0)
			{
				Console.WriteLine("Försök igen...");
			}
			return names;
		}

		private static void ApplyNames(List<String> names)
		{
			//foreach (String s in names)
			//{
			//    Console.WriteLine(s);
			//}
			//Console.ReadKey();

			forename = names[0];
			surename = names[^1];

			names.RemoveAt(0);
			names.RemoveAt(names.Count - 1);

			if (names.Count > 0)
			{
				middlenames = String.Join(' ', names);
			}
		}

		private static int ColorChange()
		{
			if (fg_c != FG_DEFAULT || bg_c != BG_DEFAULT)
			{
				Console.WriteLine("Vill du återställa färgerna till default? Y/N");
				if (Console.ReadKey().Key == ConsoleKey.Y)
				{
					Console.ResetColor();
					FG_DEFAULT = Console.ForegroundColor;
					BG_DEFAULT = Console.BackgroundColor;
					SetColors(FG_DEFAULT, BG_DEFAULT);
					return FN_MENY;
				}
			}

			String[] colorNames = Enum.GetNames<ConsoleColor>();
			optN = colorNames.Length;
			ConsoleColor newFg, newBg;

			Console.WriteLine("Välj färg på förgrunden, nuvarande är " + fg_c);
			offset = Console.GetCursorPosition().Top;
			selection = 0;

			SwapColors();
			Console.SetCursorPosition(0, offset);
			Console.WriteLine(colorNames[selection]);
			SwapColors();

			while (functionState == FN_COLOR)
			{
				KeyInput(Console.ReadKey(true));
				Console.SetCursorPosition(0, offset);
				Console.WriteLine("               ");
				SwapColors();
				Console.SetCursorPosition(0, offset);
				Console.WriteLine(colorNames[selection]);
				SwapColors();
			}
			newFg = (ConsoleColor)selection;

			functionState = FN_COLOR;
			Console.WriteLine("Välj färg på bakgrunden, nuvarande är " + bg_c);
			offset = Console.GetCursorPosition().Top;
			selection = 0;

			SwapColors();
			Console.SetCursorPosition(0, offset);
			Console.WriteLine(colorNames[selection]);
			SwapColors();

			while (functionState == FN_COLOR)
			{
				KeyInput(Console.ReadKey(true));
				Console.SetCursorPosition(0, offset);
				Console.WriteLine("               ");
				SwapColors();
				Console.SetCursorPosition(0, offset);
				Console.WriteLine(colorNames[selection]);
				SwapColors();
			}
			newBg = (ConsoleColor)selection;

			SetColors(newFg, newBg);

			ReturnMessage();
			return FN_MENY;
		}

		private static int Date()
		{
			Console.WriteLine("Dagens datum är: " + DateTime.Today.ToString());

			ReturnMessage();
			return FN_MENY;
		}

		private static int Compare()
		{
			Console.WriteLine("Jämför två tal.");

			StringBuilder inputNumber = new StringBuilder();
			Decimal[] numbers = new Decimal[2];

			for (int i = 0; i < numbers.Length; i++)
			{
				Console.Write("Tal " + (i + 1) + " ");
				ReadNumber(inputNumber);
				numbers[i] = decimal.Parse(inputNumber.ToString());
				inputNumber.Clear();
				Console.WriteLine();
			}

			Console.WriteLine(numbers[0] > numbers[1] ? "Tal ett är störst :" + numbers[0] : "Tal två är störst: " + numbers[1]);

			ReturnMessage();
			return FN_MENY;
		}

		private static int GuessNumber()
		{
			int rand = new Random(DateTime.Now.Millisecond).Next(1, 101);
			int guess = 0;
			int guesses = 0;
			StringBuilder inputNumber = new StringBuilder();

			Console.WriteLine("Gissa det slumpässiga värdet från 1 till 100!\n");

			while (guess != rand)
			{
				Console.Write((guesses + 1) + ": ");
				ReadNumber(inputNumber);
				guess = int.Parse(inputNumber.ToString());
				inputNumber.Clear();

				if (guess > 0 && guess < 101)
				{
					guesses++;
					if (guess < rand)
					{
						Console.WriteLine(" Fel! Talet är STÖRRE!");
					}
					else if (guess > rand)
					{
						Console.WriteLine(" Fel! Talet är MINDRE!");
					}
				}
				else
				{
					Console.WriteLine(" Felaktig gissning, det måste vara från 1 till 100!");
				}
			}
			Console.WriteLine(" RÄTT!! Grattis, det tog dig " + guesses + " försök.");

			ReturnMessage();

			return FN_MENY;
		}

		private static int WriteFile()
		{
			Console.WriteLine("Skriv en rad du vill spara i fil");
			
			File.AppendAllText(FILE_PATH, Console.ReadLine());
			ReturnMessage();
			return FN_MENY;
		}

		private	static int ReadFile()
		{
			inputText = File.ReadAllLines(FILE_PATH);
			foreach (String s in inputText)
			{
				Console.WriteLine(s);
			}

			ReturnMessage();
			return FN_MENY;
		}

		private static int NumberMath()
		{
			Console.WriteLine("Skriv ett tal som skall beräknas");
			StringBuilder inputNumber = new StringBuilder();
			double number;

			ReadNumber(inputNumber);

			number = double.Parse(inputNumber.ToString());
			Console.WriteLine("sqrt({0}) = {1}\n{0}^2 = {2}\n{0}^10 = {3}", number, Math.Sqrt(number), number*number, Math.Pow(number, 10));

			ReturnMessage();
			return FN_MENY;
		}

		private static int MultiTable()
		{
			if (multiTable == null)
			{
				multiTable = new int[10][];

				for (int x = 0; x < multiTable.Length; x++)
				{
					multiTable[x] = new int[10];
					for (int y = 0; y < multiTable[x].Length; y++)
					{
						multiTable[x][y] = (x+1) * (y+1);
					}
				}
			}

			Func<int[], string> tabbulera = y =>
			{
				StringBuilder s = new();
				Array.ForEach(y, n => s.Append(n < 100 ? (n < 10 ? "  " : " ") : String.Empty).Append(n).Append('\t'));
				return s.ToString();
			};

			Console.WriteLine(" *\t{0}", tabbulera(multiTable[0]));

			foreach (int[] row in multiTable)
			{
				StringBuilder stringify = new StringBuilder(row[0] < 10 ? ' ' + row[0] : row[0]);
				Console.WriteLine("{0}\t{1}", row[0] < 10 ? " " + row[0] : row[0], tabbulera(row));
			}

			ReturnMessage();
			return FN_MENY;
		}

		private static int CreateCharacter()
		{
			if (characters == null)
			{
				characters = new List<Character>();
			}

			Console.WriteLine("Skriv namn på din karaktär: ");
			characters.Add(new Character(Console.ReadLine()));

			Console.WriteLine("Skriv namn på din fiendekaraktär: ");
			characters.Add(new Character(Console.ReadLine()));

			ReturnMessage();
			return FN_MENY;
		}

		class Character
		{
			String name;
			int health, strength, luck;

			public Character(String name)
			{
				this.name = name;
				Random rand = new Random(DateTime.Now.Millisecond);
				health = rand.Next(10, 20);
				strength = rand.Next(5, 10);
				luck = rand.Next(10, 50);

				Console.WriteLine("{0} har {1} hälsa, en styrka på {2} och {3} i tur", name, health, strength, luck);
			}

		}

		private static int Exit()
		{
			Environment.Exit(0);
			return 0;
		}

		private static void Error(int n)
		{

		}
	}
}