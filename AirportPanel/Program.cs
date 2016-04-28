using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;

namespace AirportPanel
{
    class Program
    {
        static FlightsInfo flightsInfo;
        static Flight[] tmpFlightsInfo;
        static char[] currentOptions;

        public static readonly char[] defaultOptions = { 'a', 's', 'c', 'j', 'o' };
        public static readonly char[] editOptions = { 'e', 'd', 's' };
        public static readonly char[] fullOptions = { 'f', 'e', 'd', 'a', 's', 'c', 'j', 'o' };


        static string[,] header = new string[,]
        {
            { nameof(Flight.Arrival), nameof(Flight.Departure), nameof(Flight.FlightNumber), nameof(Flight.ArrivalCity),nameof(Flight.DepartureCity), nameof(Flight.Airline), nameof(Flight.Terminal), nameof(Flight.Gate),nameof(Flight.FlightStatus) }
        };

        static void Main(string[] args)
        {

            InitializeConsole();

            var countFlights = 0;
            if (args.Length > 0)
                int.TryParse(args[0], out countFlights);
            flightsInfo = new FlightsInfo(countFlights);


            currentOptions = defaultOptions;

        start:
            ShowOptions();
            GetOption();
            
            goto start;
        }

        private static void GetOption()
        {
            var key = Console.ReadKey().Key;
            ClearCurrentConsoleLine();

            switch (key)
            {
                case ConsoleKey.F:
                    Search();
                    break;
                case ConsoleKey.S:
                    PrintResult(flightsInfo.Flights);
                    currentOptions = fullOptions;
                    break;
                case ConsoleKey.D:
                    Remove();
                    PrintResult(flightsInfo.Flights);
                    currentOptions = fullOptions;
                    break;
                case ConsoleKey.E:
                    Edit();
                    PrintResult(flightsInfo.Flights);
                    currentOptions = fullOptions;
                    break;
                case ConsoleKey.A:
                    Add();
                    PrintResult(flightsInfo.Flights);
                    currentOptions = fullOptions;
                    break;
                case ConsoleKey.C:
                    Console.Clear();
                    break;
                case ConsoleKey.J:
                    SaveToFile();
                    break;
                case ConsoleKey.O:
                    OpenFromFile();
                    break;
                case ConsoleKey.Escape:
                    Environment.Exit(0);
                    break;
                default:
                    ShowMessage("Please select right option", ConsoleColor.Red);
                    break;
            }
        }
        
        #region Data File
        private static void OpenFromFile()
        {
            ShowMessage("Open from file", ConsoleColor.Blue);
            Console.WriteLine("Please enter full path to the file");
            if (flightsInfo.OpenFromFile(Console.ReadLine()))
                ShowMessage("Successful", ConsoleColor.Green);
            else
                ShowMessage("Error", ConsoleColor.Red);
        }

        private static void SaveToFile()
        {
            ShowMessage("Saving to file", ConsoleColor.Blue);
            Console.WriteLine("Please enter full path to the file");
            if (flightsInfo.SaveToFile(Console.ReadLine()))
                ShowMessage("Successful", ConsoleColor.Green);
            else
                ShowMessage("Error", ConsoleColor.Red);
        }

        #endregion

        #region Operations
        private static void Add()
        {
            ShowMessage("Adding the flight", ConsoleColor.Blue);
            ShowMessage("Please use the column names for Adding this flight (ColumnName = Value)", ConsoleColor.Yellow);
            Console.WriteLine(ToStringTable(header));
            var updateInfo = GetFlightInfo();
            if (flightsInfo.Add(updateInfo.Where(arg => arg != null).ToArray()))
                ShowMessage("Successful", ConsoleColor.Green);
            else
                ShowMessage("Error", ConsoleColor.Red);
        }

        private static void Edit()
        {
            ShowMessage("Please select an ID", ConsoleColor.Blue);
            Flight flight = null;
            int index = -1;

            if ((int.TryParse(Console.ReadLine(), out index)) && (index > -1))
            {
                if ((tmpFlightsInfo != null) && (tmpFlightsInfo.Length > 0))
                {
                    if (index < tmpFlightsInfo.Length)
                    {
                        flight = tmpFlightsInfo[index];
                    }
                }
                else if (flightsInfo.Flights.Length > index)
                {
                    flight = flightsInfo[index];
                }
            }
            else
            {
                ShowMessage("Wrong id, the flight wasn't found", ConsoleColor.Red);
            }
            if (flight != null)
            {
                ShowMessage("Please use the column names for Editing this flight (ColumnName = Value)", ConsoleColor.Yellow);
                PrintResult(new Flight[] { flight });
                var updateInfo = GetFlightInfo();
                if (updateInfo.Length > 0)
                {
                    if (flightsInfo.Update(flight, updateInfo))
                        ShowMessage("Successful", ConsoleColor.Green);
                    else
                        ShowMessage("Error", ConsoleColor.Red);
                }
                else
                    ShowMessage("Error", ConsoleColor.Red);

            }
        }

        private static void Remove()
        {
            ShowMessage("Please select an ID", ConsoleColor.Blue);
            Flight flight = null; ;
            int index = -1;

            if ((int.TryParse(Console.ReadLine(), out index)) && (index > -1))
            {
                if ((tmpFlightsInfo != null) && (tmpFlightsInfo.Length > 0))
                {
                    if (index < tmpFlightsInfo.Length)
                    {
                        flight = tmpFlightsInfo[index];
                    }
                }
                else if (flightsInfo.Flights.Length > index)
                {
                    flight = flightsInfo[index];
                }
            }
            else
            {
                ShowMessage("Wrong id, the flight wasn't found", ConsoleColor.Red);
            }
            if (flight != null)
            {
                if (flightsInfo.Remove(flight))
                    ShowMessage("Successful", ConsoleColor.Green);
                else
                    ShowMessage("Error", ConsoleColor.Red);
            }else
                ShowMessage("Error", ConsoleColor.Red);
        }

        private static void Search()
        {
            ShowMessage("Search engine", ConsoleColor.Blue);            
            ShowMessage("Please use the column names for writing search pattern (ColumnName eq/gt/lt Value)", ConsoleColor.Yellow);
            Console.WriteLine(ToStringTable(header));

            string[] searchLine = GetFieldAndValues(Console.ReadLine());
            tmpFlightsInfo = flightsInfo.Find(searchLine);
            if ((tmpFlightsInfo != null) && (tmpFlightsInfo.Count(arg => arg != null) > 0))
            {
                ShowMessage("Flight found", ConsoleColor.Green);
                PrintResult(tmpFlightsInfo);
                currentOptions = editOptions;
            }
            else
            {
                ShowMessage("Flight wasn't found", ConsoleColor.Red);
                currentOptions = fullOptions;
            }
        }

        #endregion

        #region parsing values
        private static string[][] GetFlightInfo()
        {
            const int maxInfoSize = 100;
            string[][] updateInfo = new string[maxInfoSize][];
            string[] updateLine = null;
            int index = 0;
            do
            {
                updateLine = GetFieldAndValues(Console.ReadLine());
                if (updateLine.Length == 3)
                    updateInfo[index] = updateLine;
                else
                    break;
                index++;

            } while (index < maxInfoSize);

            return updateInfo.Where(arg => arg != null).ToArray();
        }

        private static string[] GetFieldAndValues(string searchString)
        {
            const int searchArraySize = 3;
            string[] searchCriteria = new string[searchArraySize];

            var searchArray = searchString.Trim().Split(' ');

            if (searchArray.Length > 3)
            {
                searchArray[2] = string.Join(" ", searchArray.Where((key, arg) => Convert.ToInt32(arg) > 1).ToArray());
            }

            if (searchArray.Length > 2)
            {
                for (int i = 0; i < searchArraySize; i++)
                {
                    searchCriteria[i] = searchArray[i].Trim();
                }
            }

            return searchCriteria.Where(arg => arg != null).ToArray();
        }

        #endregion

        #region utils
        private static void ShowMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void ShowOptions()
        {            
            Dictionary<char, string> possibleMovments = new Dictionary<char, string>()
            {
                {'f', "Find the flight" },
                {'e', "Edit the flight" },
                {'d', "Delete the flight" },
                {'a', "Add the flight" },
                {'s', "Show all flights" },
                {'c', "Clear this area" },
                {'j', "Save to file" },
                {'o', "Open from file" }
            };

            Console.WriteLine("Instructions");
            foreach (KeyValuePair<char, string> move in possibleMovments)
            {
                if ((currentOptions != null) && (!currentOptions.Contains(move.Key)))
                    continue;

                Console.WriteLine("Key: {0}, Option: {1}", move.Key, move.Value);
            }
            Console.WriteLine("Or press Esc for exit");
        }

        #endregion

        #region view results

        private static void PrintResult(Flight[] flights)
        {
            ShowMessage("All Flights", ConsoleColor.Blue);
            flights = flights.Where(arg => arg != null).ToArray();
            var resultToPrint = new string[flights.Length + 1, header.GetLength(1) + 1];
            resultToPrint[0, 0] = "ID";
            for (int i = 0; i < header.GetLength(1); i++)
                resultToPrint[0, i + 1] = header[0, i];

            for (int i = 0; i < flights.Length; i++)
            {
                resultToPrint[i + 1, 0] = i.ToString();
                resultToPrint[i + 1, 1] = string.Format("{0:MM/dd/yy H:mm:ss zzz}", flights[i].Arrival);
                resultToPrint[i + 1, 2] = string.Format("{0:MM/dd/yy H:mm:ss zzz}", flights[i].Departure);
                resultToPrint[i + 1, 3] = flights[i].FlightNumber;
                resultToPrint[i + 1, 4] = flights[i].ArrivalCity;
                resultToPrint[i + 1, 5] = flights[i].DepartureCity;
                resultToPrint[i + 1, 6] = flights[i].Airline;
                resultToPrint[i + 1, 7] = flights[i].Terminal;
                resultToPrint[i + 1, 8] = flights[i].Gate;
                resultToPrint[i + 1, 9] = flights[i].FlightStatus.ToString();
            }

            Console.WriteLine(ToStringTable(resultToPrint));
        }

        public static string ToStringTable(string[,] arrValues)
        {
            int[] maxColumnsWidth = GetMaxColumnWidth(arrValues);
            var headerSpliter = new string('-', maxColumnsWidth.Sum(i => i + 3) - 1);

            var sb = new StringBuilder();
            for (int rowIndex = 0; rowIndex < arrValues.GetLength(0); rowIndex++)
            {
                for (int colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++)
                {
                    if (arrValues[rowIndex, colIndex] != null)
                    {
                        // Print cell
                        string cell = arrValues[rowIndex, colIndex];
                        cell = cell.PadRight(maxColumnsWidth[colIndex]);
                        sb.Append(" | ");
                        sb.Append(cell);
                    }
                }


                if (arrValues[rowIndex, 0] != null)
                {
                    // Print end of line
                    sb.Append(" | ");
                    sb.AppendLine();

                    // Print splitter
                    if (rowIndex == 0)
                    {
                        sb.AppendFormat(" |{0}| ", headerSpliter);
                        sb.AppendLine();
                    }
                }
            }

            return sb.ToString();
        }

        static public int[] GetMaxColumnWidth(string[,] arrayValues)
        {
            var maxColumnsWidth = new int[arrayValues.GetLength(1)];
            for (int colIndex = 0; colIndex < arrayValues.GetLength(1); colIndex++)
            {
                for (int rowIndex = 0; rowIndex < arrayValues.GetLength(0); rowIndex++)
                {
                    if (arrayValues[rowIndex, colIndex] != null)
                    {
                        int newLength = arrayValues[rowIndex, colIndex].Length;
                        int oldLength = maxColumnsWidth[colIndex];

                        if (newLength > oldLength)
                        {
                            maxColumnsWidth[colIndex] = newLength;
                        }
                    }
                }
            }

            return maxColumnsWidth;
        }
        #endregion

        #region console view


        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        static public void InitializeConsole()
        {
            IntPtr hConsole = GetStdHandle(-11);   // get console handle
            COORD xy = new COORD(100, 100);
            SetConsoleDisplayMode(hConsole, 1, out xy); // set the console to fullscreen
            //SetConsoleDisplayMode(hConsole, 2);   // set the console to windowed


        }

        [StructLayout(LayoutKind.Sequential)]
        public struct COORD
        {

            public short X;
            public short Y;
            public COORD(short x, short y)
            {
                this.X = x;
                this.Y = y;
            }

        }
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetStdHandle(int handle);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleDisplayMode(
            IntPtr ConsoleOutput
            , uint Flags
            , out COORD NewScreenBufferDimensions
            );
        #endregion
    }


}
