using System;

namespace CW_AvaloniaProject.Models
{
  public class InternationalTour : TravelPackage
  {
    private decimal _visaCost;
    private decimal _flightInsurance;

    public decimal VisaCost
    {
      get => _visaCost;
      set => _visaCost = value;
    }

    public decimal FlightInsurance
    {
      get => _flightInsurance;
      set => _flightInsurance = value;
    }

    // Constructor
    public InternationalTour(string dest, DateTime start, int days, decimal price, decimal visaCost, decimal insurance)
        : base(dest, start, days, price)
    {
      VisaCost = visaCost;
      FlightInsurance = insurance;
    }

    public override decimal CalculateTotalPrice()
    {
      return BasePrice + VisaCost + FlightInsurance;
    }

    public override string GetDescription()
    {
      return $"[International] {base.GetDescription()} | Visa Required: {(VisaCost > 0 ? "Yes" : "No")}";
    }
  }
}