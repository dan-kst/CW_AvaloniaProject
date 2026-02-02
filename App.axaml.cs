using Avalonia;
using Avalonia.Markup.Xaml;

namespace CW_AvaloniaProject;

public partial class App : Application
{
  public override void Initialize()
  {
    AvaloniaXamlLoader.Load(this);
  }
}