using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using CW_AvaloniaProject.Models;

namespace CW_AvaloniaProject.Managers
{
  public class TourManager
  {
    // In-memory storage
    private ObservableCollection<TravelPackage> _tours = new ObservableCollection<TravelPackage>();

    // Basic CRUD (Create/Read)
    public void AddTour(TravelPackage tour)
    {
      if (tour == null) throw new ArgumentNullException(nameof(tour));
      _tours.Add(tour);
    }

    public ObservableCollection<TravelPackage> GetAllTours()
    {
      return _tours;
    }

    // --- LINQ REQUIREMENT IMPLEMENTATION ---

    // 1. FILTERING (Where)
    // Finds tours cheaper than a specific budget
    public IEnumerable<TravelPackage> GetCheapTours(decimal maxPrice)
    {
      // Note: CalculateTotalPrice() is called polymorphically here
      return _tours.Where(t => t.BasePrice <= maxPrice);
    }

    // 2. SEARCHING (Where + String methods)
    // Case-insensitive search for destination
    public IEnumerable<TravelPackage> SearchByDestination(string keyword)
    {
      if (string.IsNullOrWhiteSpace(keyword)) return new ObservableCollection<TravelPackage>();

      return _tours
          .Where(t => t.Destination.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }

    // 3. SORTING (OrderBy / OrderByDescending)
    // Sorts tours by Start Date
    public IEnumerable<TravelPackage> GetToursSortedByDate(bool ascending)
    {
      if (ascending)
      {
        return _tours.OrderBy(t => t.StartDate);
      }
      else
      {
        return _tours.OrderByDescending(t => t.StartDate);
      }
    }

    // 4. TYPE FILTERING (OfType)
    // Specific LINQ operator to get ONLY International tours from the mixed IEnumerable
    public IEnumerable<InternationalTour> GetOnlyInternationalTours()
    {
      return _tours.OfType<InternationalTour>();
    }

    // 5. AGGREGATION (Sum, Average, Min, Max)
    // Calculates total revenue if all tours are sold
    public decimal CalculatePotentialRevenue()
    {
      if (!_tours.Any()) return 0;
      return _tours.Sum(t => t.CalculateTotalPrice());
    }

    // 6. PROJECTION (Select) - Advanced LINQ
    // Returns a IEnumerable of simple strings describing the tours (useful for simple UI IEnumerables)
    public IEnumerable<string> GetTourSummaries()
    {
      return _tours
          .Select(t => $"{t.Destination} - {t.CalculateTotalPrice():C}");
    }
  }
}