# [ALPHA] P42.Uno.AsyncNavigation
"Xamarin.Forms like" Asynchronous navigation for pre-instantiated UWP / Uno page content

**NOTE:** This project is in preliminary development and supports only the most basic navigation functionality.  If there are features you would like to see, please submit an issue.

## Purpose
The Xamarin.Forms NavigationPage is the basis for page navigation in Xamarin.Forms.  There are two parts to it that are very convenient:

- Its asynchronous nature
- The abilty to pre-instantiate page objects *before* navigation

Both of these characteristics make it easy to sequence app logic based upon where things are in the navigation proesses.  For example, having the two above features makes it trivial to provide busy indicators while viewmodels are views being generated.

In UWP, page navigation is a bit more [abstract](https://docs.microsoft.com/en-us/windows/uwp/design/basics/navigate-between-two-pages) - with the Application's Frame handling the page instantiation and presentation out of scope.  In other words, pages are instantiated and presented at once by calling the app's Frame's `Navigate` method, using the new page's type as an argument.  The downside to this approach is you don't have a handle on the new page object before navigation starts - and thus it's really hard to know when the page is done instantiating, loading, and rendering.

But there are times when you want to pre-load as much of a page as you can *before* you start a navigation event.  Perhaps the page is a bit bigger than you would like and it's being rendered on a very slow device - like a page with a list view with complex cells in Uno Wasm app?  Or maybe you're porting a Xamarin.Forms application and you really would like a navigation model that won't require a big tear up?

To make this simpler, **AsyncNavigation** was created.

## Usage

### Initialization
First you will need to reference, in your project, `P42.Uno.AsyncNavigation` as a project or a NuGet package.

Next, similar to `NavigationPage` in Xamarin.Forms, in order to use **AsyncNavigation**, the root page of your application needs to be a `P42.Uno.AsyncNavigation.NavigationPage`.  In UWP / Uno apps, this means opening up `App.xsml.cs` and replacing the following default code ...

```csharp
if (e.PrelaunchActivated == false)
{
    if (rootFrame.Content == null)
    {
        // When the navigation stack isn't restored navigate to the first page,
        // configuring the new page by passing required information as a navigation
        // parameter
        rootFrame.Navigate(typeof(MainPage), e.Arguments);
        }
    }
}
```

... with the below:

```csharp
if (e.PrelaunchActivated == false)
{
    if (rootFrame.Content == null)
    {
        // When the navigation stack isn't restored navigate to the first page,
        // configuring the new page by passing required information as a navigation
        // parameter
        rootFrame.Navigate(typeof(P42.Uno.AsyncNavigation.NavigationPage), typeof(MainPage));
        }
    }
}
```

This sets the root of your project to a `P42.Uno.AsyncNavigation.NavigationPage`, creates an instance of `MainPage`, and places that instance of `MainPage` onto the navigation stack.

### PushAsync()

Now you'll be able to navigate as follows:

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

## Animation Options

Both `PushAsync` and `PopAsync` support an optional argument of the type `PageAnimationOptions`.  This type has the following properties:

- `AnimationDirection`, the direction of the page animation.  
  - `None`
  - `LeftToRight`
  - `RightToLeft`
  - `TopToBottom`
  - `BottomToTop`

  **NOTE:** The default for entrance (`PushAsync`) is `RightToLeft`.  The default for exit (`PopAsync`) is the opposite of what ever the entrance was for that page.
- `ShouldFade`, should the page fade in/out during entrance / exit?  Default: `true`.
- `Duration`, what is the `TimeSpan` of the animation?  Default: `TimeSpan.FromMilliseconds(600)`.
- `EasingFunction`, what is the easing function to be used?  Default: `CubicEasing`.

## Navigation Bar

To make transitioning from Xamarin.Forms easier, here are a few extensions methods you might find helpful.

### HasNavigationBar
Indicates the display of the page's navigation bar, positioned at the top of the page.

- `public static bool GetHasNavigationBar(this Page page)`: Returns a value that indicates whether the page has a navigation bar. Default: `true`.
- `public static void SetHasNavigationBar(this Page page, bool value)`: Sets a value that indicates whether or not this NavigationPage Page has a navigation bar.

### HasBackButton
Indicates the display of the page's Back Button, positioned on the left of the page's navigation bar.

- `public static bool GetHasBackButton(this Page page)`: Returns a value that indicates whether page has a back button. Default: `true`.
- `public static void SetHasBackButton(this Page page, bool value)`:  Sets a value that indicates whether page has a back button.

### BackButtonTitle
The title text for the page's back button, positioned to the right of the Back icon (which is on the left side of the page's navigation bar).

- `public static string GetBackButtonTitle(this Page page)`: Returns the title of the back button for the specified page. Default: `default(string)`.
- `public static void SetBackButtonTitle(this Page page, string value)`: Sets the title of the back button for the specified page.

### Title
The page's title element, positioned at the center of the page's navigation bar.

- `public static object GetTitle(this Page page)`: Returns the page's title element (could be a `string` or a `FrameworkElement`).  Default: `default(object)`.
- `public static void SetTitle(this Page page, object value)`: Sets the page's title element (could be a `string` or a `FrameworkElement`). 

### Icon
The page's navigation bar IconElement, positioned to the left of the Title.

- `public static IconElement GetIcon(this Page page)`: Sets the page's IconElement.  Default: `default(IconElement)`.
- `public static void SetIcon(this Page page, IconElement value)`: Sets the page's IconElement.

### IconColor
The color of the page's icon.

- `public static Color GetIconColor(this Page page)`: Returns the page's title icon's color.  Default: default system text/icon color.
- `public static void SetIconColor(this Page page, Color value)`: Set the page's title icon's color.
