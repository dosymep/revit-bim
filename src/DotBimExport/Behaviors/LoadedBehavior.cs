using System.Windows;
using System.Windows.Input;

using Microsoft.Xaml.Behaviors;

namespace DotBimExport.Behaviors;

/// <summary>
/// Executes a command when the window raises <see cref="FrameworkElement.Loaded"/>.
/// </summary>
public sealed class LoadedBehavior : Behavior<Window> {
    /// <summary>
    /// Identifies the <see cref="Command"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
        nameof(Command),
        typeof(ICommand),
        typeof(LoadedBehavior));

    /// <summary>
    /// Gets or sets the command invoked when the window is loaded.
    /// </summary>
    public ICommand? Command {
        get => (ICommand?) GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttached() {
        base.OnAttached();
        AssociatedObject.Loaded += OnLoaded;
    }

    /// <inheritdoc />
    protected override void OnDetaching() {
        AssociatedObject.Loaded -= OnLoaded;
        base.OnDetaching();
    }

    private void OnLoaded(object sender, RoutedEventArgs e) {
        if(Command is not { } command || !command.CanExecute(null)) {
            return;
        }

        command.Execute(null);
    }
}
