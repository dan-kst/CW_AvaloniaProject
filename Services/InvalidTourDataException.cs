using System;
using CW_AvaloniaProject.Models;

namespace CW_AvaloniaProject.Services
{
  // 1. Custom Exception Class (Good practice for domain-specific errors)
  public class InvalidTourDataException(string message, string fieldName) : Exception(message)
  {
    public string InvalidField { get; } = fieldName;
  }
}