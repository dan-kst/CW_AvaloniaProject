using System;
using System.Collections.Generic;
using System.Linq;
using CW_AvaloniaProject.Models;

namespace CW_AvaloniaProject.Managers
{
  public class TourManager
  {
    // In-memory storage
    private List<TravelPackage> _tours = new List<TravelPackage>();

    // Basic CRUD (Create/Read)
    public void AddTour(TravelPackage tour)
    {
      if (tour == null) throw new ArgumentNullException(nameof(tour));
      _tours.Add(tour);
    }

    public List<TravelPackage> GetAllTours()
    {
      return _tours;
    }

    // --- LINQ REQUIREMENT IMPLEMENTATION ---

    // 1. FILTERING (Where)
    // Finds tours cheaper than a specific budget
    public List<TravelPackage> FilterByMaxPrice(decimal maxBudget)
    {
      // LINQ query syntax
      // Uses polymorphism: CalculateTotalPrice() calls the specific child implementation
      return _tours
          .Where(t => t.CalculateTotalPrice() <= maxBudget)
          .ToList();
    }

    // 2. SEARCHING (Where + String methods)
    // Case-insensitive search for destination
    public List<TravelPackage> SearchByDestination(string keyword)
    {
      if (string.IsNullOrWhiteSpace(keyword)) return new List<TravelPackage>();

      return _tours
          .Where(t => t.Destination.Contains(keyword, StringComparison.OrdinalIgnoreCase))
          .ToList();
    }

    // 3. SORTING (OrderBy / OrderByDescending)
    // Sorts tours by Start Date
    public List<TravelPackage> GetToursSortedByDate(bool ascending)
    {
      if (ascending)
      {
        return _tours.OrderBy(t => t.StartDate).ToList();
      }
      else
      {
        return _tours.OrderByDescending(t => t.StartDate).ToList();
      }
    }

    // 4. TYPE FILTERING (OfType)
    // Specific LINQ operator to get ONLY International tours from the mixed list
    public List<InternationalTour> GetOnlyInternationalTours()
    {
      return _tours.OfType<InternationalTour>().ToList();
    }

    // 5. AGGREGATION (Sum, Average, Min, Max)
    // Calculates total revenue if all tours are sold
    public decimal CalculatePotentialRevenue()
    {
      if (!_tours.Any()) return 0;
      return _tours.Sum(t => t.CalculateTotalPrice());
    }

    // 6. PROJECTION (Select) - Advanced LINQ
    // Returns a list of simple strings describing the tours (useful for simple UI lists)
    public List<string> GetTourSummaries()
    {
      return _tours
          .Select(t => $"{t.Destination} - {t.CalculateTotalPrice():C}")
          .ToList();
    }
  }
}