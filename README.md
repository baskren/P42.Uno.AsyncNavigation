# P42.Uno.AsyncNavigation
Asynchronous navigation for pre-instantiated UWP / Uno page content

## Purpose
Even though it's not recommended, there are times when you want to pre-load as much of a page as you can 
*before* you start a navigation event.  Perhaps the page is a bit bigger than you would like and it's being 
rendered on a very slow device.  Don't laugh ... it happens in the real world.

When this happens, it would be great if a **loading ...** animation could be shown to your users.  But, how
do you bound the beginning and end of the page loading cycle?  Turns out it's pretty complicated and subject 
to variation between operating systems.

To make this simpler, **AsyncNavigation** was created.

## Usage

### PushAsync(Page page)
After referencing, in your project, `P42.Uno.AsyncNavigation` as a project or a NuGet package, you will be able 
to navigate as follows:

```csharp
        async void OnOpenPage1ButtonClick(object sender, RoutedEventArgs e)
        {
            await StartProgressAnimation();  // your code to display a progress indicator
            var page = new Page1();
            await P42.Uno.AsyncNavigation.Navigation.PushAsync(page);
            await StopProgressAnimation();  // your code to hide the progress indicator shown above
        }
```        

### PopAsync()

Correspondingly, you can asynchronously pop pages (that have been asynchronously pushed).

```csharp
        async void OnBackButtonPressed(object sender, RoutedEventArgs e)
        {
            await StartProgressAnimation();  // your code to display a progress indicator
            await P42.Uno.AsyncNavigation.Navigation.PopAsync();
            await StopProgressAnimation();  // your code to hide the progress indicator shown above
        }
```

