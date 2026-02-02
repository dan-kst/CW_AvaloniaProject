using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using CW_AvaloniaProject.Models;
using CW_AvaloniaProject.Services;
using CW_AvaloniaProject.Managers;
using System.Linq;
using System.Collections.Generic;

namespace CW_AvaloniaProject
{
  public partial class MainWindow : Window
  {

    private readonly TourManager _manager;
    private readonly DatabaseService _dbService;

    public MainWindow()
    {
      InitializeComponent();
      _manager = new TourManager();
      _dbService = new DatabaseService();

      // Handle UI switching for tour types
      ComboType.SelectionChanged += (s, e) =>
      {
        bool isInternational = ComboType.SelectedIndex == 0;
        if (PanelInternational != null && PanelDomestic != null)
        {
          PanelInternational.IsVisible = isInternational;
          PanelDomestic.IsVisible = !isInternational;
        }
      };

      UpdateListUI();
    }

    public void OnAddClick(object sender, RoutedEventArgs e)
    {
      StatusText.Text = "";
      StatusText.Foreground = Avalonia.Media.Brushes.Red;

      try
      {
        TravelPackage newTour;

        if (ComboType.SelectedIndex == 0) // International
        {
          // Use the Factory from Step 2
          newTour = TourFactory.CreateInternational(
              TxtDest.Text ?? "",
              TxtDate.Text ?? "",
              TxtDuration.Text ?? "",
              TxtPrice.Text ?? "",
              TxtVisa.Text ?? "",
              TxtInsurance.Text ?? ""
          );
        }
        else // Domestic
        {
          string dest = TxtDest.Text ?? "";
          if (string.IsNullOrWhiteSpace(dest)) throw new ArgumentException("Destination required");
          if (!decimal.TryParse(TxtPrice.Text, out decimal price)) throw new ArgumentException("Invalid Price");
          if (!DateTime.TryParse(TxtDate.Text, out DateTime date)) throw new ArgumentException("Invalid Date");
          if (!int.TryParse(TxtDuration.Text, out int days)) throw new ArgumentException("Invalid Duration");

          newTour = new DomesticTour(dest, date, days, price, ChkTransport.IsChecked ?? false);
        }

        _manager.AddTour(newTour);
        StatusText.Foreground = Avalonia.Media.Brushes.Green;
        StatusText.Text = "Success! Tour added.";
        UpdateListUI();
      }
      catch (InvalidTourDataException ex)
      {
        // Handling our custom exception
        StatusText.Text = $"Validation Error ({ex.InvalidField}): {ex.Message}";
      }
      catch (Exception ex)
      {
        // Handling general errors
        StatusText.Text = $"System Error: {ex.Message}";
      }
    }

    // --- LINQ HANDLERS ---

    public void OnShowAllClick(object sender, RoutedEventArgs e)
    {
      UpdateListUI();
      DbStatus.Text = "Showing all tours.";
    }

    public void OnSearchClick(object sender, RoutedEventArgs e)
    {
      TourList.ItemsSource = _manager.SearchByDestination(TxtSearch.Text ?? "");
    }

    public void OnSortDateClick(object sender, RoutedEventArgs e)
    {
      TourList.ItemsSource = _manager.GetToursSortedByDate(true);
      DbStatus.Text = "Sorted by start date.";
    }

    public void OnFilterCheapClick(object sender, RoutedEventArgs e)
    {
      TourList.ItemsSource = _manager.GetCheapTours(1000);
      DbStatus.Text = "Showing tours under $1000 (Base Price).";
    }

    public void OnFilterIntlClick(object sender, RoutedEventArgs e)
    {
      TourList.ItemsSource = _manager.GetOnlyInternationalTours();
      DbStatus.Text = "Filtered: International Tours only.";
    }
    public void OnRevenueClick(object sender, RoutedEventArgs e)
    {
      decimal total = _manager.CalculatePotentialRevenue();
      DbStatus.Text = $"Total Potential Revenue: {total:C}";
    }

    // --- REQUIREMENT 6: DATABASE (SQL) ---

    public void OnSaveDbClick(object sender, RoutedEventArgs e)
    {
      try
      {
        var tours = _manager.GetAllTours();
        // To avoid duplicate primary key errors in this simple demo, 
        // we clear the table and re-save current session data.
        _dbService.ClearDatabase();

        foreach (var t in tours)
        {
          _dbService.SaveTour(t);
        }
        DbStatus.Text = $"Successfully saved {tours.Count} tours to SQL.";
      }
      catch (Exception ex)
      {
        DbStatus.Text = "Save Failed: " + ex.Message;
      }
    }

    public void OnLoadDbClick(object sender, RoutedEventArgs e)
    {
      try
      {
        var loadedTours = _dbService.LoadTours();

        foreach (var t in loadedTours)
        {
          // Only add if not already in memory to prevent UI duplicates
          if (!_manager.GetAllTours().Any(x => x.Id == t.Id))
          {
            _manager.AddTour(t);
          }
        }

        UpdateListUI();
        DbStatus.Text = $"Loaded {loadedTours.Count} tours from Database.";
      }
      catch (Exception ex)
      {
        DbStatus.Text = "Load Failed: " + ex.Message;
      }
    }

    // --- HELPER FOR UI BINDING ---
    private void UpdateListUI()
    {
      TourList.ItemsSource = _manager.GetAllTours();
    }
  }
}