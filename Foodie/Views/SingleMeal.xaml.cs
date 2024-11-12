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
        FavsChipGroup.SelectedItem = UnFavChip;
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
        Launcher.OpenAsync(new Uri(ViewModel.SelectedMeal.OriginalArticleUrl));
    }

    private void FavsChipGroup_SelectionChanged(object sender, Syncfusion.Maui.Toolkit.Chips.SelectionChangedEventArgs e)
    {
        bool isAdding;
        if (e.AddedItem is not null)
        {
            var addedChip = e.AddedItem as SfChip;
            if (addedChip.FontSize == 14)//14 for fav 13 for non fav
            {
                if (!ViewModel.CurrentUser.ListOfFavouriteMeals.Contains(ViewModel.SelectedMeal))
                {
                    ViewModel.CurrentUser.ListOfFavouriteMeals.Add(ViewModel.SelectedMeal);
                }

            }
            else if (addedChip.FontSize == 13)
            {
                var removedItem = addedChip.BindingContext as MealModelView;
                if (ViewModel.CurrentUser.ListOfFavouriteMeals.Contains(ViewModel.SelectedMeal))
                {
                    ViewModel.CurrentUser.ListOfFavouriteMeals.Remove(ViewModel.SelectedMeal);
                }

            }
        }
    }

}