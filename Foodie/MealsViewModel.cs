using Syncfusion.Maui.Toolkit.Carousel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Foodie;
public partial class MealsViewModel : ObservableObject
{
    public PageEnum CurrentPage = PageEnum.HomePage;
    [ObservableProperty]
    UserModelView currentUser;

    [ObservableProperty]
    ObservableCollection<MealModelView> allMeals =new();
    [ObservableProperty]
    ObservableCollection<TagModelView> allTags =new();
    
    [ObservableProperty]
    TagModelView selectedTag=new();
    [ObservableProperty]
    MealModelView selectedMeal=new();
    [ObservableProperty]
    string mealSteps =string.Empty;
    [ObservableProperty]
    string mealIngredients =string.Empty;
    [ObservableProperty]
    string mealVideoLinks =string.Empty;
    [ObservableProperty]
    string selectedMealImage =string.Empty;
    
    public IMealService MealService { get; }
    public MealsViewModel(IMealService mealService)
    {
        MealService = mealService;
        InitializeApp();
        MealService.RefreshComplete = (status) => InitializeApp();
        
    }

    private void InitializeApp()
    {
        GetAnyRandomFourMeals();
        if (MealService.AllMeals is null || MealService.AllTags is null)
        {
            return;
        }
        
        AllMeals = MealService.AllMeals.ToObservableCollection();
        AllTags = MealService.AllTags.ToObservableCollection();
        CurrentUser = MealService.CurrentUser;
        GetAnyRandomFourMeals();

    }

    List<string> allMealsNames = new();
    List<string> allTagsNames = new();
    public void GearUpForSearch()
    {
        allMealsNames = AllMeals.Select(n => n.Name).ToList();
        allTagsNames= AllTags.Select(n => n.TagName).ToList();
    }

    [ObservableProperty]
    ObservableCollection<MealModelView> anyRandomFourMeals ;
    [ObservableProperty]
    ObservableCollection<MealModelView> moreMeals;
    public void GetAnyRandomFourMeals()
    {
        if (MealService.AllMeals is null || MealService.AllTags is null)
        {
            return;
        }

        AnyRandomFourMeals ??= new();
        AnyRandomFourMeals = AllMeals
    .OrderBy(x => Guid.NewGuid()) // Randomly shuffles the items
    .Take(4) // Takes any 4 random items
    .ToObservableCollection();
        
    }
    partial void OnAllMealsChanged(ObservableCollection<MealModelView>? oldValue, ObservableCollection<MealModelView> newValue)
    {
        Debug.WriteLine("changed");
    }

    [RelayCommand]
    public async Task UpSertMeal(MealModelView? meal=null)
    {
        if (meal is null)
        {
            meal = SelectedMeal;
            var existingMealIndexx = AllMeals.IndexOf(AllMeals.FirstOrDefault(x => x.Id == meal.Id));
            if (existingMealIndexx != -1)
            {
                AllMeals[existingMealIndexx] = meal;
            }
        }
        meal.VideoLinks = MealVideoLinks.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToObservableCollection();
        ProcessMealSteps();

        await ParseAndSaveIngredients();
        MealService.UpdateMeal(meal);
        var existingMealIndex = AllMeals.IndexOf(AllMeals.FirstOrDefault(x => x.Id == meal.Id));
        if (existingMealIndex != -1)
        {
            AllMeals[existingMealIndex] = meal;
        }
        else
        {
            // Add the new item
            AllMeals.Add(meal);
        }

        Reset();
    }

    [RelayCommand]
    void Reset()
    {
        SelectedMeal = new();
        SelectedMealImage = string.Empty;
        MealSteps = string.Empty;
        MealVideoLinks = string.Empty;
        MealIngredients = string.Empty;
    }

    private void ProcessMealSteps()
    {
        if (!string.IsNullOrEmpty(MealSteps))
        {
            var lines = MealSteps.Split(new[] { '.', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            Console.WriteLine($"Total Lines: {lines.Length}");
            foreach (var line in lines)
            {
                Console.WriteLine($"Line: {line}");
            }
            var stepModels = lines.Select((line, index) => new StepModelView(
                stepNumber: (index + 1).ToString(),
                stepDescription: line.Trim()
            )).ToObservableCollection();

            SelectedMeal.Steps = stepModels;
        }
    }


    private async Task ParseAndSaveIngredients()
    {
        string pattern = @"^(\d+(\.\d+)?)?\s*(kg|g|ml|oz|lb|teaspoon|tablespoon|tbsp|tsp)?\s*(.*)$";
        Regex ingredientRegex = new Regex(pattern, RegexOptions.IgnoreCase);

        // Replace commas with '\n' and split by '\r' and '\n'.
        var lines = MealIngredients.Replace(",", "\n").Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        var ingredients = new ObservableCollection<IngredientModelView>();

        foreach (var line in lines)
        {
            var match = ingredientRegex.Match(line.Trim());

            if (match.Success)
            {
                string qty = match.Groups[1].Value;
                string unit = match.Groups[3].Value;
                string ingredientName = match.Groups[4].Value;
                

                var ingredient = new IngredientModelView(ingredientName, qty, unit);
                ingredients.Add(ingredient);
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", $"Invalid format for ingredient {line}\nIt should be of format QTY UNIT ITEM: Example '2 kg eggs'", "OK");
                return;
            }
        }
        SelectedMeal.Ingredients = ingredients;
    }




    [ObservableProperty]
    string currentTagName;  
    [RelayCommand]
    public void DeleteMeal()
    {
        MealService.DeleteMeal(SelectedMeal);
        AllMeals.Remove(SelectedMeal);
        SelectedMeal = new();
    }

    [RelayCommand]
    public void AddTag()
    {
        var tag = new TagModelView(CurrentTagName);
        AllTags.Add(tag);
        MealService.AddTag(tag);
        CurrentTagName = string.Empty;
    }
    [RelayCommand]
    public void DeleteTag()
    {
        var tag = new TagModelView(CurrentTagName);
        AllTags.Remove(tag);
        MealService.DeleteTag(tag);
        CurrentTagName = string.Empty;
    }
    [ObservableProperty]
    string newImageUrl = string.Empty;
    [RelayCommand]
    public async Task GetImage()
    {
        if (SelectedMeal is null)
        {
            return;
        }
        //SelectedMeal.ImageUrl = await Utils.GetMealImagePath(SelectedMeal, NewImageUrl);
        ////SelectedMealImage = NewImageUrl;
        ////SelectedMeal.ImageUrl = NewImageUrl;
        //await UpSertMeal(SelectedMeal);
    }
    [ObservableProperty]
    int favStatusIndex =1;
    [ObservableProperty]
    bool isLoadingJustForlooks;
    public void ViewSingleMeal()
    {
        if(CurrentUser.ListOfFavouriteMeals.Contains(SelectedMeal))
        {
            FavStatusIndex = 0;
        }
        if (CurrentPage == PageEnum.SingleMealPage)
        {
            return;
        }
        Shell.Current.GoToAsync(nameof(SingleMeal));
    }
    public async void AddToFavoriteMeal(MealModelView? meal)
    {
        if (meal is null )
        {
            meal = SelectedMeal;
        }

        CurrentUser.ListOfFavouriteMeals.Add(meal);
        meal.IsFavorite = true;
        await UpSertMeal(meal);
        UpdateUser();
    }
    public async void RemoveFromFavoriteMeal(MealModelView? meal)
    {
        if (meal is null)
        {
            meal = SelectedMeal;
        }

        CurrentUser.ListOfFavouriteMeals.Remove(meal);
        meal.IsFavorite = false;
        await UpSertMeal(meal);
        UpdateUser();
    }
    
    public void AddToBlackListed(MealModelView? meal)
    {
        if (meal is null)
        {
            meal = SelectedMeal;
        }

        CurrentUser.ListOfBlackListedMeals.Add(meal);
        
        
        UpdateUser();
    }
    public void RemoveFromBlackListed(MealModelView? meal)
    {
        if (meal is null)
        {
            meal = SelectedMeal;
        }

        CurrentUser.ListOfBlackListedMeals.Remove(meal);
    }

    public async void GoToFavoritesPage()
    {
        MealService.GetMeals();
        AllMeals = MealService.AllMeals.ToObservableCollection();
        await Shell.Current.GoToAsync(nameof(FavoriteMeals));
    }

    [RelayCommand]
    public void UpdateUser()
    {
        MealService.UpSertUser(CurrentUser);
    }

    [ObservableProperty]
    string searchText;
    bool GotResultInName;

    [RelayCommand]
    public void SearchMeal(string searText)
    {
        var result = MealService.AllMeals
    .Where(meal => !string.IsNullOrEmpty(meal.Name) && meal.Name.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0)
    .ToList();

        if (result.Count<1)
        {
            GotResultInName = false;
            AllMeals = MealService.AllMeals.ToObservableCollection();
            return; 
        }
        else
        {
            GotResultInName = true;
            AllMeals = result.ToObservableCollection();
        }
    }
}

public enum PageEnum
{
    HomePage,
    SearchPage,
    SingleMealPage
}