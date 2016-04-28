using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportPanel
{
    class Flight
    {
        #region fields
        string _flightNumber;
        DateTime _arrival;
        DateTime _departure;

        string _arrivalCity;
        string _departureCity;

        string _airline;
        string _terminal;
        string _gate;
        FlightStatus _flightStatus;
        #endregion

        #region properties

        public Guid Id { get; private set; }

        public string FlightNumber {
            get{ return _flightNumber; }
            set { _flightNumber = value; }
        }
        public DateTime Arrival
        {
            get { return _arrival; }
            set { _arrival = value; }
        }
        public DateTime Departure
        {
            get { return _departure; }
            set { _departure = value; }
        }

        public string ArrivalCity
        {
            get { return _arrivalCity; }
            set { _arrivalCity = value; }
        }
        public string DepartureCity
        {
            get { return _departureCity; }
            set { _departureCity = value; }
        }

        public string Airline
        {
            get { return _airline; }
            set { _airline = value; }
        }
        public string Terminal
        {
            get { return _terminal; }
            set { _terminal = value; }
        }
        public string Gate
        {
            get { return _gate; }
            set { _gate = value; }
        }
        public FlightStatus FlightStatus
        {
            get { return _flightStatus; }
            set { _flightStatus = value; }
        }
        #endregion

        public Flight()
        {
            Id = Guid.NewGuid();
            Airline = string.Empty;
            ArrivalCity = string.Empty;
            DepartureCity = string.Empty;
            FlightNumber = string.Empty;
            Gate = string.Empty;
            Terminal = string.Empty;
        }

        public object this[string propertyName]
        {
            get { return GetType().GetProperty(propertyName).GetValue(this, null); }
            set { GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }       

        public static bool operator true(Flight flight)
        {
            return !string.IsNullOrWhiteSpace(flight.Airline) &&
                flight.Arrival != null &&
                !string.IsNullOrWhiteSpace(flight.ArrivalCity) &&
                flight.Departure != null &&
                !string.IsNullOrWhiteSpace(flight.DepartureCity) &&
                !string.IsNullOrWhiteSpace(flight.FlightNumber) &&
                !string.IsNullOrWhiteSpace(flight.Gate) &&
                flight.Id != null &&
                !string.IsNullOrWhiteSpace(flight.Terminal);
        }

        public static bool operator false(Flight flight)
        {
            return string.IsNullOrWhiteSpace(flight.Airline) ||
                flight.Arrival == null ||
                string.IsNullOrWhiteSpace(flight.ArrivalCity) ||
                flight.Departure == null ||
                string.IsNullOrWhiteSpace(flight.DepartureCity) ||
                string.IsNullOrWhiteSpace(flight.FlightNumber) ||
                string.IsNullOrWhiteSpace(flight.Gate) ||
                flight.Id == null ||
                string.IsNullOrWhiteSpace(flight.Terminal);
        }
    }

    enum FlightStatus
    {
        CheckIn,
        GateClosed,
        Arrived,
        DepartedAt,
        Unknown,
        Canceled,
        ExpectedAt,
        Delayed,
        InFlight
    }
}
