using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using CW_AvaloniaProject.Models;

namespace CW_AvaloniaProject.Data
{
  public class DatabaseService
  {
    private readonly string _connectionString;

    public DatabaseService()
    {
      // Creates a file named "tours.db" in the application folder
      string dbPath = Path.Combine(AppContext.BaseDirectory, "tours.db");
      _connectionString = $"Data Source={dbPath}";
      InitializeDatabase();
    }

    // 1. Initialize DB (Create Table if it doesn't exist)
    private void InitializeDatabase()
    {
      using (var connection = new SqliteConnection(_connectionString))
      {
        connection.Open();

        // SQL Command to create a single table for all tour types
        // We use nullable columns (REAL NULL, INTEGER NULL) for fields specific to child classes
        string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Tours (
                        Id TEXT PRIMARY KEY,
                        TourType TEXT NOT NULL,
                        Destination TEXT NOT NULL,
                        StartDate TEXT NOT NULL,
                        DurationDays INTEGER NOT NULL,
                        BasePrice REAL NOT NULL,
                        VisaCost REAL NULL,
                        FlightInsurance REAL NULL,
                        IncludePrivateTransport INTEGER NULL,
                        TransportSurcharge REAL NULL
                    );";

        using (var command = new SqliteCommand(createTableQuery, connection))
        {
          command.ExecuteNonQuery();
        }
      }
    }

    // 2. Save (INSERT)
    // Requirement 6: Use SQL queries to store information
    public void SaveTour(TravelPackage tour)
    {
      using (var connection = new SqliteConnection(_connectionString))
      {
        connection.Open();

        string insertQuery = @"
                    INSERT INTO Tours (
                        Id, TourType, Destination, StartDate, DurationDays, BasePrice, 
                        VisaCost, FlightInsurance, IncludePrivateTransport, TransportSurcharge
                    ) VALUES (
                        @Id, @TourType, @Destination, @StartDate, @DurationDays, @BasePrice,
                        @VisaCost, @FlightInsurance, @IncludePrivateTransport, @TransportSurcharge
                    );";

        using (var command = new SqliteCommand(insertQuery, connection))
        {
          // Common fields
          command.Parameters.AddWithValue("@Id", tour.Id.ToString());
          command.Parameters.AddWithValue("@Destination", tour.Destination);
          command.Parameters.AddWithValue("@StartDate", tour.StartDate.ToString("yyyy-MM-dd"));
          command.Parameters.AddWithValue("@DurationDays", tour.DurationDays);
          command.Parameters.AddWithValue("@BasePrice", tour.BasePrice);

          // Polymorphic check to save specific fields
          if (tour is InternationalTour inter)
          {
            command.Parameters.AddWithValue("@TourType", "International");
            command.Parameters.AddWithValue("@VisaCost", inter.VisaCost);
            command.Parameters.AddWithValue("@FlightInsurance", inter.FlightInsurance);
            command.Parameters.AddWithValue("@IncludePrivateTransport", DBNull.Value);
            command.Parameters.AddWithValue("@TransportSurcharge", DBNull.Value);
          }
          else if (tour is DomesticTour dom)
          {
            command.Parameters.AddWithValue("@TourType", "Domestic");
            command.Parameters.AddWithValue("@VisaCost", DBNull.Value);
            command.Parameters.AddWithValue("@FlightInsurance", DBNull.Value);
            // SQLite uses 1 for true, 0 for false
            command.Parameters.AddWithValue("@IncludePrivateTransport", dom.IncludePrivateTransport ? 1 : 0);
            command.Parameters.AddWithValue("@TransportSurcharge", dom.TransportSurcharge);
          }

          command.ExecuteNonQuery();
        }
      }
    }

    // 3. Load (SELECT)
    public List<TravelPackage> LoadTours()
    {
      var tours = new List<TravelPackage>();

      using (var connection = new SqliteConnection(_connectionString))
      {
        connection.Open();

        string selectQuery = "SELECT * FROM Tours";

        using (var command = new SqliteCommand(selectQuery, connection))
        using (var reader = command.ExecuteReader())
        {
          while (reader.Read())
          {
            // Read common data
            string type = reader.GetString(reader.GetOrdinal("TourType"));
            string dest = reader.GetString(reader.GetOrdinal("Destination"));
            DateTime start = DateTime.Parse(reader.GetString(reader.GetOrdinal("StartDate")));
            int days = reader.GetInt32(reader.GetOrdinal("DurationDays"));
            decimal price = reader.GetDecimal(reader.GetOrdinal("BasePrice"));

            // IMPORTANT: Need to restore the ID to match the original object
            Guid id = Guid.Parse(reader.GetString(reader.GetOrdinal("Id")));

            TravelPackage tour = null;

            // Reconstruct specific objects based on 'TourType'
            if (type == "International")
            {
              decimal visa = reader.GetDecimal(reader.GetOrdinal("VisaCost"));
              decimal insurance = reader.GetDecimal(reader.GetOrdinal("FlightInsurance"));

              tour = new InternationalTour(dest, start, days, price, visa, insurance);
            }
            else if (type == "Domestic")
            {
              bool hasTransport = reader.GetInt32(reader.GetOrdinal("IncludePrivateTransport")) == 1;
              tour = new DomesticTour(dest, start, days, price, hasTransport);
            }

            if (tour != null)
            {
              typeof(TravelPackage).GetProperty("Id").SetValue(tour, id);

              tours.Add(tour);
            }
          }
        }
      }

      return tours;
    }
  }
}