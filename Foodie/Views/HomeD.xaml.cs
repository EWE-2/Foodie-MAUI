using Microsoft.Maui.Platform;
using Syncfusion.Maui.Toolkit.NavigationDrawer;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Foodie.Views;

public partial class HomeD : ContentPage
{
    public HomeD(MealsViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        BindingContext = viewModel;
    }

    public MealsViewModel ViewModel { get; }


    List<string> supportedFilePaths;
    private async void DropGestureRecognizer_Drop(object sender, DropEventArgs e)
    {
        var s = e.Data;
#if WINDOWS
        var windowsEvents = e.PlatformArgs.DragEventArgs;
        var dragUI = windowsEvents.DragUIOverride;
        var items = await windowsEvents.DataView.GetStorageItemsAsync();
        supportedFilePaths = new List<string>();
        if (items.Count > 0)
        {
            foreach (var item in items)
            {
                if (item is Windows.Storage.StorageFile file)
                {
                    string fileExtension = file.FileType.ToLower();
                    if (fileExtension != ".jpg" && fileExtension != ".jpeg" &&
                        fileExtension != ".webp" && fileExtension != ".png")
                    {
                        dragUI.IsGlyphVisible = true;
                        return;
                    }
                    else
                    {
                        Debug.WriteLine($"File is {item.Path}");
                        supportedFilePaths.Add(item.Path.ToLower());
                    }
                }
            }
        }
#endif
        MealsPicture.ImageSource = supportedFilePaths[0];
        ViewModel.SelectedMeal.ImageUrl = supportedFilePaths[0];
    }

    private void ChangeMeal_Tapped(object sender, TappedEventArgs e)
    {
        var send = (View)sender;
        var meal = send.BindingContext as MealModelView;
        ViewModel.MealSteps = meal.Steps.ToStepString();


        ViewModel.MealIngredients = meal.Ingredients.ToIngredientString();
        ViewModel.SelectedMealImage = meal.ImageUrl;
        ViewModel.SelectedMeal = meal;

        TagsChipGroup.SelectedItem = null;
    }

    private void SaveButton_Clicked(object sender, EventArgs e)
    {
        TagsChipGroup.SelectedItem = null;
    }

    private void SfChipGroup_SelectionChanged(object sender, Syncfusion.Maui.Toolkit.Chips.SelectionChangedEventArgs e)
    {
        var add = e.AddedItem as TagModelView;
        var rem = e.RemovedItem as TagModelView;

        if (add is null)
        {
            ViewModel.SelectedMeal.Tags.Remove(rem);
        }
        else
        {
            ViewModel.SelectedMeal.Tags.Add(add);
        }
    }

    private void ResetButton_Clicked(object sender, EventArgs e)
    {
        MealsPicture.ImageSource = null;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ViewModel.CurrentPage = PageEnum.HomePage;
    }
}