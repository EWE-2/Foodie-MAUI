using Syncfusion.Maui.Toolkit.Carousel;

namespace Foodie.Views;

public partial class HomePage : ContentPage
{
	public HomePage(MealsViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        BindingContext = viewModel;

        //viewW.Source = new Uri("https://www.youtube.com/watch?v=nYboXPsTRts&t=1711s");
    }

    public MealsViewModel ViewModel { get; }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ViewModel.CurrentPage = PageEnum.HomePage;
    }

    private void ChangeMeal_Tapped(object sender, TappedEventArgs e)
    {
        var send = (View)sender;
        var meal = send.BindingContext as MealModelView;
        ViewModel.MealSteps = meal.Steps.ToStepString();


        ViewModel.MealIngredients = meal.Ingredients.ToIngredientString();
        ViewModel.SelectedMealImage = meal.ImageUrl;
        ViewModel.SelectedMeal = meal;
        ViewModel.ViewSingleMeal();
        //TagsChipGroup.SelectedItem = null;
    }

    private void FavsButton_Clicked(object sender, EventArgs e)
    {
        NavDraw.IsOpen = false;
        ViewModel.GoToFavoritesPage();
    }
}