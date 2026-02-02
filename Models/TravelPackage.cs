
using System;
using System.ComponentModel.DataAnnotations;

namespace CW_AvaloniaProject.Models
{
  public abstract class TravelPackage
  {
    // Fields (Encapsulation)
    private Guid _id;
    private string _destination;
    private DateTime _startDate;
    private int _durationDays;
    private decimal _basePrice;

    // Selectors and Modifiers (Properties)
    public Guid Id
    {
      get { return _id; }
      private set { _id = value; } // Read-only from outside
    }

    public string Destination
    {
      get { return _destination; }
      set
      {
        if (string.IsNullOrWhiteSpace(value))
          throw new ArgumentException("Destination cannot be empty.");
        _destination = value;
      }
    }

    public DateTime StartDate
    {
      get { return _startDate; }
      set { _startDate = value; }
    }

    public int DurationDays
    {
      get { return _durationDays; }
      set
      {
        if (value < 1) throw new ArgumentException("Duration must be at least 1 day.");
        _durationDays = value;
      }
    }

    public decimal BasePrice
    {
      get { return _basePrice; }
      set
      {
        if (value < 0) throw new ArgumentException("Price cannot be negative.");
        _basePrice = value;
      }
    }

    // Constructor without parameters
    public TravelPackage()
    {
      Id = Guid.NewGuid();
      Destination = "Unknown";
      StartDate = DateTime.Now;
      DurationDays = 1;
      BasePrice = 0;
    }

    // Constructor with parameters
    public TravelPackage(string destination, DateTime startDate, int durationDays, decimal basePrice)
    {
      Id = Guid.NewGuid();
      Destination = destination;
      StartDate = startDate;
      DurationDays = durationDays;
      BasePrice = basePrice;
    }

    ~TravelPackage()
    {
      System.Diagnostics.Debug.WriteLine($"Object {Id} (TravelPackage) is being finalized.");
    }

    public abstract decimal CalculateTotalPrice();

    public virtual string GetDescription()
    {
      return $"Trip to {Destination} ({DurationDays} days) - Starts: {StartDate.ToShortDateString()}";
    }
  }
}

