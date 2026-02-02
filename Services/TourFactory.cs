using System;
using CW_AvaloniaProject.Models;

namespace CW_AvaloniaProject.Services
{
  // 2. Factory Class to handle Object Creation safely
  public static class TourFactory
  {
    /// <summary>
    /// Tries to create an International Tour from raw string inputs.
    /// This simulates reading from GUI TextBoxes.
    /// </summary>
    public static InternationalTour CreateInternational(
        string destInput,
        string dateInput,
        string durationInput,
        string priceInput,
        string visaInput,
        string insuranceInput)
    {
      try
      {
        // -- VALIDATION & PARSING --

        // 1. Validate Destination
        if (string.IsNullOrWhiteSpace(destInput))
          throw new InvalidTourDataException("Destination cannot be empty.", nameof(destInput));

        // 2. Validate Date
        if (!DateTime.TryParse(dateInput, out DateTime startDate))
          throw new InvalidTourDataException("Invalid date format. Use YYYY-MM-DD.", nameof(dateInput));

        if (startDate < DateTime.Now.Date)
          throw new InvalidTourDataException("Tour cannot start in the past.", nameof(dateInput));

        // 3. Validate numeric fields (Handling Parsing Errors)
        int duration = ParseInt(durationInput, "Duration");
        decimal basePrice = ParseDecimal(priceInput, "Price");
        decimal visaCost = ParseDecimal(visaInput, "Visa Cost");
        decimal insurance = ParseDecimal(insuranceInput, "Insurance");

        // -- CREATION --
        // The Constructor itself might throw ArgumentException (defined in Step 1)
        return new InternationalTour(destInput, startDate, duration, basePrice, visaCost, insurance);
      }
      catch (FormatException ex)
      {
        // Catching system exceptions and wrapping them in our logic
        throw new InvalidTourDataException($"Data format error: {ex.Message}", "General");
      }
      catch (ArgumentException ex)
      {
        // Catching validation errors from the Model classes (Step 1)
        throw new InvalidTourDataException(ex.Message, "Model Validation");
      }
      catch (Exception ex)
      {
        // Catch-all for unexpected errors
        // Log error here in a real app
        throw new Exception($"Critical Error creating tour: {ex.Message}", ex);
      }
    }

    // Helper method to consolidate parsing logic
    private static int ParseInt(string input, string fieldName)
    {
      if (!int.TryParse(input, out int result))
        throw new InvalidTourDataException($"{fieldName} must be a valid whole number.", fieldName);
      return result;
    }

    private static decimal ParseDecimal(string input, string fieldName)
    {
      if (!decimal.TryParse(input, out decimal result))
        throw new InvalidTourDataException($"{fieldName} must be a valid number (e.g., 100.50).", fieldName);
      return result;
    }
  }
}