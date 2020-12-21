# P42.Uno.AsyncNavigation
"Xamarin.Forms like" Asynchronous navigation for pre-instantiated UWP / Uno page content

## Purpose
The Xamarin.Forms NavigationPage is the basis for page navigation in Xamarin.Forms.  There are two parts to it that are very convenient:

- Its asynchronous nature
- The abilty to pre-instantiate page objects *before* navigation

Both of these characteristics make it easy to sequence app logic based upon where things are in the navigation proesses.  For example, having the two above features makes it trivial to provide busy indicators while viewmodels are views being generated.

So, in UWP, how do you capture the beginning and end of the page loading cycle?  Turns out it's pretty complicated and subject 
to variation between operating systems.  This is because, in UWP, navigation has been architected quite differently.  Pages are instantiated and presented at once by calling the app's Frame's `Navigate` method, using the new page's type as an argument.  The downside to this approach is you don't have a handle on the new page object before navigation starts - and thus it's really hard to know when the page is done instantiating, loading, and rendering.

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

