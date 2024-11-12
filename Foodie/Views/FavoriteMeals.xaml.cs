namespace Foodie.Views;

public partial class FavoriteMeals : ContentPage
{
	public FavoriteMeals(MealsViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        BindingContext = viewModel;
    }

    public MealsViewModel ViewModel { get; }

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
}