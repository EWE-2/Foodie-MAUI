using DevExpress.Maui.Editors;
using Syncfusion.Maui.Toolkit.Carousel;
using Syncfusion.Maui.Toolkit.Chips;

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
        //TagsChipGroup.SelectedItem = null;
    }

    private void FavsButton_Clicked(object sender, EventArgs e)
    {
        NavDraw.IsOpen = false;
        ViewModel.GoToFavoritesPage();
    }

    private void NavToSingleMeal_Clicked(object sender, EventArgs e)
    {
        NavDraw.IsOpen = false;
        ViewModel.GoToFavoritesPage();
    }

    private void IsFavToggleBtn_CheckedChanged(object sender, DevExpress.Maui.Core.ValueChangedEventArgs<bool> e)
    {
        if (e.NewValue)
        {
            if (!ViewModel.CurrentUser.ListOfFavouriteMeals.Contains(ViewModel.SelectedMeal))
            {
                ViewModel.CurrentUser.ListOfFavouriteMeals.Add(ViewModel.SelectedMeal);
                ViewModel.SelectedMeal.IsFavorite = true;
            }

        }
        else
        if (ViewModel.CurrentUser.ListOfFavouriteMeals.Contains(ViewModel.SelectedMeal))
        {
            ViewModel.CurrentUser.ListOfFavouriteMeals.Remove(ViewModel.SelectedMeal);
            ViewModel.SelectedMeal.IsFavorite = false;
        }
        ViewModel.UpdateUser();
    }

    private void IsbkmrkToggleBtn_CheckedChanged(object sender, DevExpress.Maui.Core.ValueChangedEventArgs<bool> e)
    {
        if (e.NewValue)
        {
            if (!ViewModel.CurrentUser.ListOfMeals.Contains(ViewModel.SelectedMeal))
            {
                ViewModel.CurrentUser.ListOfMeals.Add(ViewModel.SelectedMeal);
                ViewModel.SelectedMeal.IsBookMarked = true;
                IsFavToggleBtn.IsChecked = true;
            }

        }
        else
        if (ViewModel.CurrentUser.ListOfMeals.Contains(ViewModel.SelectedMeal))
        {
            ViewModel.CurrentUser.ListOfMeals.Remove(ViewModel.SelectedMeal);
            ViewModel.SelectedMeal.IsFavorite = false;
            IsFavToggleBtn.IsChecked = false;
        }
        ViewModel.UpdateUser();

    }

    private void HomePageCV_TapConfirmed(object sender, DevExpress.Maui.CollectionView.CollectionViewGestureEventArgs e)
    {
        var meal = e.Item as MealModelView;
        ViewModel.SelectedMeal = meal;

        ViewModel.MealSteps = meal.Steps.ToStepString();


        ViewModel.MealIngredients = meal.Ingredients.ToIngredientString();
        ViewModel.SelectedMealImage = meal.ImageUrl;
        ViewModel.SelectedMeal = meal;
        ViewModel.ViewSingleMeal();
    }

    private void HomePageCV_LongPress(object sender, DevExpress.Maui.CollectionView.CollectionViewGestureEventArgs e)
    {
        var item = e.Item as MealModelView;
        ViewModel.SelectedMeal = item;

        HomePageBtmSheet.Show();
    }

    List<MealModelView>? filteredMeals = new();

    private void IsFavToggleBtn_TapPressed(object sender, DevExpress.Maui.Core.DXTapEventArgs e)
    {

    }

    private void HomePaGeMealSearchBox_TextChanged(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(HomePaGeMealSearchBox.Text))
        {
            if (HomePaGeMealSearchBox.Text.Length >= 1)
            {
                HomePageCV.FilterString = $"Contains([Name], '{HomePaGeMealSearchBox.Text}')";
                filteredMeals?.Clear();

                // Apply the filter to the DisplayedSongs collection
                filteredMeals = ViewModel.AllMeals
                    .Where(item => item.Name.Contains(HomePaGeMealSearchBox.Text, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            }
            else
            {
                HomePageCV.FilterString = string.Empty;
            }
        }

    }

    private void HomePaGeMealSearchBox_ClearIconClicked(object sender, HandledEventArgs e)
    {
        HomePageCV.FilterString = string.Empty;
        filteredMeals?.Clear();

    }
}