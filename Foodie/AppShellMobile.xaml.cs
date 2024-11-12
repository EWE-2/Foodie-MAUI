namespace Foodie;

public partial class AppShellMobile : Shell
{
	public AppShellMobile()
	{
		InitializeComponent();
        Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
        Routing.RegisterRoute(nameof(SearchPage), typeof(SearchPage));
        Routing.RegisterRoute(nameof(SingleMeal), typeof(SingleMeal));
        Routing.RegisterRoute(nameof(FavoriteMeals), typeof(FavoriteMeals));
    }
}