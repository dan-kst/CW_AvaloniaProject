using System;
using System.Collections.ObjectModel;
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
  }
}