using System;

namespace CW_AvaloniaProject.Models
{
  public class DomesticTour : TravelPackage
  {
    private bool _includePrivateTransport;
    private decimal _transportSurcharge;

    public bool IncludePrivateTransport
    {
      get => _includePrivateTransport;
      set => _includePrivateTransport = value;
    }

    public decimal TransportSurcharge
    {
      get => _transportSurcharge;
      set => _transportSurcharge = value;
    }

    // Constructor
    public DomesticTour(string dest, DateTime start, int days, decimal price, bool privateTransport)
        : base(dest, start, days, price)
    {
      IncludePrivateTransport = privateTransport;
      TransportSurcharge = privateTransport ? 150.0m : 0m;
    }

    protected override decimal CalculateTotalPrice()
    {
      return BasePrice + TransportSurcharge;
    }

    protected override string GetDescription()
    {
      return $"[Domestic] {base.GetDescription()} | Transport: {(IncludePrivateTransport ? "Private Bus" : "Self-Travel")}";
    }
  }
}