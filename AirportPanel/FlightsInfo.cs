using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportPanel
{
    class FlightsInfo
    {
        const int MaxFlights = 100;

        #region fields
        Flight[] _flights;

        #endregion
        #region properties
        public Flight[] Flights
        {
            get { return _flights; }
            set { _flights = value; }
        }
        #endregion

        #region Constructors

        public FlightsInfo()
        {
            _flights = new Flight[MaxFlights];
        }

        public FlightsInfo(int SizeOfFlights) : this()
        {
            if (SizeOfFlights > 0)
                InitializeFlightsByDefault(SizeOfFlights);
        }

        #endregion

        public Flight this[int index]
        {
            get
            {
                if ((_flights != null) && (_flights.Length > index))
                    return _flights[index];
                else
                    return new Flight();
            }
            set
            {
                if ((_flights != null) && (_flights.Length > index) && (value != null))
                    _flights[index] = value;
            }
        }

        #region managing flights

        const int fieldNameIndex = 0;
        const int fieldConditionIndex = 1;
        const int fieldValueIndex = 2;

        /// <summary>
        /// Does search in array of flights based on search values
        /// </summary>
        /// <param name="searchValues">Array of fields and their values</param>
        /// <returns>Array of flights found</returns>
        public Flight[] Find(string[] searchValues)
        {
            var flights = new Flight[_flights.Length];
            var indexOfFlights = 0;
            try
            {
                if (!string.IsNullOrWhiteSpace(searchValues[fieldNameIndex]))
                {
                    foreach (Flight flight in _flights)
                    {
                        if (flight != null)
                        {

                            var flightPropertyVal = flight[searchValues[fieldNameIndex]];
                            ConditionalTypes conditionalType = (ConditionalTypes)Enum.Parse(typeof(ConditionalTypes), searchValues[fieldConditionIndex]);

                            if (Search(conditionalType, searchValues[fieldValueIndex], flightPropertyVal))
                            {
                                flights[indexOfFlights] = flight;
                                indexOfFlights++;
                            }

                        }
                    }
                }
            }
            catch { }
            
            return flights.Where(arg => arg != null).ToArray();
        }

        /// <summary>
        /// Does search in flight itself based on field name and it's value 
        /// </summary>
        /// <param name="conditionalType">type of comparsion</param>
        /// <param name="searchValue">value for search</param>
        /// <param name="compareValue">original value</param>
        /// <returns>Positive if found</returns>
        private bool Search(ConditionalTypes conditionalType, object searchValue, object compareValue)
        {
            switch (Type.GetTypeCode(compareValue.GetType()))
            {
                case TypeCode.Int32:
                    {
                        switch (conditionalType)
                        {
                            case ConditionalTypes.eq:
                                return (int)searchValue == (int)compareValue;
                            case ConditionalTypes.gt:
                                return (int)searchValue > (int)compareValue;
                            case ConditionalTypes.lt:
                                return (int)searchValue < (int)compareValue;
                        }
                    }
                    break;
                case TypeCode.String:
                    {
                        switch (conditionalType)
                        {
                            case ConditionalTypes.eq:
                                return string.Compare((string)searchValue, (string)compareValue, false) == 0;
                            case ConditionalTypes.gt:
                                return string.Compare((string)searchValue, (string)compareValue, false) < 0;
                            case ConditionalTypes.lt:
                                return string.Compare((string)searchValue, (string)compareValue, false) > 0;
                        }
                    }
                    break;
                case TypeCode.DateTime:
                    {
                        switch (conditionalType)
                        {
                            case ConditionalTypes.eq:
                                return DateTime.Compare(Convert.ToDateTime(searchValue), (DateTime)compareValue) == 0;
                            case ConditionalTypes.gt:
                                return DateTime.Compare(Convert.ToDateTime(searchValue), (DateTime)compareValue) > 0;
                            case ConditionalTypes.lt:
                                return DateTime.Compare(Convert.ToDateTime(searchValue), (DateTime)compareValue) < 0;
                        }
                    }
                    break;
            }
            return false;
        }

        /// <summary>
        /// Adds the flight to array
        /// </summary>
        /// <param name="addValues">Flight's fields and their values</param>
        /// <returns>Positive if added</returns>
        internal bool Add(string[][] addValues)
        {
            var index = _flights.Count(arg => arg != null);
            if ((index > -1) && (index < _flights.Length))
            {
                var flight = new Flight();
                try
                {
                    foreach (string[] addValue in addValues)
                    {
                        switch (Type.GetTypeCode(flight[addValue[fieldNameIndex]].GetType()))
                        {
                            case TypeCode.Int32:
                                try
                                {
                                    flight[addValue[fieldNameIndex]] = Convert.ToInt32(addValue[fieldValueIndex]);
                                }
                                catch { }
                                break;
                            case TypeCode.String:
                                try
                                {
                                    flight[addValue[fieldNameIndex]] = addValue[fieldValueIndex];
                                }
                                catch { }
                                break;
                            case TypeCode.DateTime:
                                try
                                {
                                    flight[addValue[fieldNameIndex]] = Convert.ToDateTime(addValue[fieldValueIndex]);
                                }
                                catch { }
                                break;
                        }
                    }

                    if (flight)
                    {
                        _flights[index] = flight;
                        return true;
                    }
                }
                catch { }
            }
            return false;
        }

        /// <summary>
        /// Updates the flight
        /// </summary>
        /// <param name="flight">Flight for update</param>
        /// <param name="updateValues">Fields and their values to update</param>
        /// <returns>Positive if updated</returns>
        internal bool Update(Flight flight, string[][] updateValues)
        {
            var index = Array.IndexOf(_flights, flight);
            if (index > -1)
            {
                try
                {
                    foreach (string[] updateValue in updateValues)
                    {
                        switch (Type.GetTypeCode(flight[updateValue[fieldNameIndex]].GetType()))
                        {
                            case TypeCode.Int32:
                                try
                                {
                                    flight[updateValue[fieldNameIndex]] = Convert.ToInt32(updateValue[fieldValueIndex]);
                                }
                                catch { }
                                break;
                            case TypeCode.String:
                                try
                                {
                                    flight[updateValue[fieldNameIndex]] = updateValue[fieldValueIndex];
                                }
                                catch { }
                                break;
                            case TypeCode.DateTime:
                                try
                                {
                                    flight[updateValue[fieldNameIndex]] = Convert.ToDateTime(updateValue[fieldValueIndex]);
                                }
                                catch { }
                                break;
                        }
                    }

                    if (flight)
                    {
                        _flights[index] = flight;
                        return true;
                    }
                }
                catch { }
            }
            return false;
        }

        /// <summary>
        /// Removes the flight from array
        /// </summary>
        /// <param name="flight">Flight to remove</param>
        /// <returns>Positive if removed</returns>
        internal bool Remove(Flight flight)
        {
            if (flight != null)
            {
                _flights = _flights.Where(arg => arg != null && arg.Id != flight.Id).ToArray();
                return true;
            }
            return false;
        }

        #endregion        

        #region Flights Data
        /// <summary>
        /// Initialize Default array of flights wit ha fake data
        /// </summary>
        /// <param name="sizeOfFlights">Amout of fake flights</param>
        private void InitializeFlightsByDefault(int sizeOfFlights)
        {
            if (sizeOfFlights > MaxFlights)
                sizeOfFlights = MaxFlights;

            for (int i = 0; i < sizeOfFlights; i++)
            {
                _flights[i] = new Flight
                {
                    Airline = "test",
                    ArrivalCity = "Kharkiv",
                    DepartureCity = "Kiev",
                    Gate = "G" + 1 * i,
                    Arrival = DateTime.Now,
                    Departure = DateTime.Now,
                    FlightNumber = "72" + 2 * i,
                    FlightStatus = (i < 9) ? (FlightStatus)i : (FlightStatus)8,
                    Terminal = "F" + i
                };
            }
        }

        /// <summary>
        /// Opens file with flight's data
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <returns>Positive if flights are loaded</returns>
        public bool OpenFromFile(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    string text = File.ReadAllText(path);
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        _flights = JsonConvert.DeserializeObject<Flight[]>(text);
                        return true;
                    }
                }
                catch { }
            }
            return false;
        }

        /// <summary>
        /// Saves flights to the file
        /// </summary>
        /// <param name="path">Path to file where flight's data should be saved</param>
        /// <returns>Positive if flights are saved</returns>
        internal bool SaveToFile(string path)
        {
            try
            {
                var written = false;
                var flightsJson = JsonConvert.SerializeObject(_flights);
                //File.WriteAllText(path, flightsJson);
                using (StreamWriter file = new StreamWriter(path, true))
                {
                    file.WriteLine(flightsJson);
                    written = true;
                }
                if (written)
                    return true;
            }
            catch { }
            return false;
        }
        #endregion


    }
}
