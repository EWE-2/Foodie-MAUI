using DevExpress.Maui.Editors;
using Syncfusion.Maui.Toolkit.Chips;

namespace Foodie.Views;

public partial class SingleMeal : ContentPage
{
	public SingleMeal(MealsViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        BindingContext = viewModel;
        //viewW.Source = new Uri("https://www.youtube.com/watch?v=nYboXPsTRts&t=1711s");
    }

    public MealsViewModel ViewModel { get; }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel.GetImage();
        ViewModel.CurrentPage = PageEnum.SingleMealPage;

        switch (ViewModel.SelectedMeal.IsFavorite)
        {
            case true:
                IsFavToggleChip.SelectedIndex = 0;
                break;
            case false:
                IsFavToggleChip.SelectedIndex = 1;
                break;
            default:
                break;
        }
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

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (!string.IsNullOrEmpty(ViewModel.SelectedMeal.OriginalArticleUrl))
        {
            Launcher.OpenAsync(new Uri(ViewModel.SelectedMeal.OriginalArticleUrl));
        }
    }

    private void UILayoutToggled_SelectionChanged(object sender, EventArgs e)
    {
        var s = sender as ChoiceChipGroup;
        switch (s.SelectedIndex)
        {
            case 0:
                if (!ViewModel.CurrentUser.ListOfFavouriteMeals.Contains(ViewModel.SelectedMeal))
                {
                    ViewModel.CurrentUser.ListOfFavouriteMeals.Add(ViewModel.SelectedMeal);
                    ViewModel.SelectedMeal.IsFavorite = true;
                }
                break;
            case 1:
                if (ViewModel.CurrentUser.ListOfFavouriteMeals.Contains(ViewModel.SelectedMeal))
                {
                    ViewModel.CurrentUser.ListOfFavouriteMeals.Remove(ViewModel.SelectedMeal);
                    ViewModel.SelectedMeal.IsFavorite = false;
                }
                break;
            default:
                break;

        }
        ViewModel.UpdateUser();
    }

    private async void StepsExpanderBtn_Clicked(object sender, EventArgs e)
    {
        StepsExpander.Commands.ToggleExpandState.Execute(null);
        if (StepsExpander.IsExpanded)
        {
            double currentY = SingleMealScrollView.ScrollY;
            await SingleMealScrollView.ScrollToAsync(0, currentY + 100, true);
        }

    }
    private async void IngredientExpanderBtn_Clicked(object sender, EventArgs e)
    {
        IngredientsExpander.Commands.ToggleExpandState.Execute(null);
        if (IngredientsExpander.IsExpanded)
        {
            double currentY = SingleMealScrollView.ScrollY;
            await SingleMealScrollView.ScrollToAsync(0, currentY + 100, true);
        }
        //double currentY = SingleMealScrollView.ScrollY;
        //await SingleMealScrollView.ScrollToAsync(0, currentY + 50, true);

    }
}