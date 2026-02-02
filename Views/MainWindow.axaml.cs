using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using CW_AvaloniaProject.Models;
using CW_AvaloniaProject.Services;
using CW_AvaloniaProject.Managers;
using System.Collections.Generic;

namespace CW_AvaloniaProject
{
  public partial class MainWindow : Window
  {

    private readonly TourManager _manager;

    public MainWindow()
    {
      InitializeComponent();
      _manager = new TourManager();

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

    // --- REQUIREMENT 4: EXCEPTION HANDLING ---
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
          // Manual validation for Domestic (simulating Factory logic here for variety)
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

    // --- REQUIREMENT 5: LINQ OPERATIONS ---

    public void OnShowAllClick(object sender, RoutedEventArgs e) { }

    public void OnSortDateClick(object sender, RoutedEventArgs e) { }

    public void OnFilterCheapClick(object sender, RoutedEventArgs e) { }

    public void OnFilterIntlClick(object sender, RoutedEventArgs e) { }

    public void OnRevenueClick(object sender, RoutedEventArgs e) { }

    // --- REQUIREMENT 6: DATABASE (SQL) ---

    public void OnSaveDbClick(object sender, RoutedEventArgs e) { }

    public void OnLoadDbClick(object sender, RoutedEventArgs e) { }

    // --- HELPER FOR UI BINDING ---
    private void UpdateListUI()
    {
      TourList.ItemsSource = _manager.GetAllTours();
    }
  }
}