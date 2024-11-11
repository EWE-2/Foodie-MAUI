using Syncfusion.Maui.Toolkit.Carousel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Foodie;
public partial class MealsViewModel : ObservableObject
{

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
    }

    private void InitializeApp()
    {
        MealService.GetMeals();
        MealService.GetTags();

        if (MealService.AllMeals is null || MealService.AllTags is null)
        {
            return;
        }
        
        AllMeals = MealService.AllMeals.ToObservableCollection();
        AllTags = MealService.AllTags.ToObservableCollection();
    }

    [RelayCommand]
    public async Task UpSertMeal()
    {
        SelectedMeal.VideoLinks = mealVideoLinks.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToObservableCollection();
        ProcessMealSteps();

        await ParseAndSaveIngredients();
        MealService.UpdateMeal(SelectedMeal);
        var existingMealIndex = AllMeals.IndexOf(AllMeals.FirstOrDefault(x => x.Id == SelectedMeal.Id));
        if (existingMealIndex != -1)
        {
            // Replace the existing item with the updated one
            AllMeals[existingMealIndex] = SelectedMeal;
        }
        else
        {
            // Add the new item
            AllMeals.Add(SelectedMeal);
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
}
