using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodie.Models;
public class MealModel: RealmObject
{
    [PrimaryKey]
    public ObjectId Id { get; set; }=ObjectId.GenerateNewId();
    public string Name { get; set; }
    public IList<IngredientModel> Ingredients { get; }
    public IList<StepModel> Steps { get; }
    public string ImageUrl { get; set; }
    public IList<string> MoreImagesUrl { get; }
    public string Description { get; set; }
    public string Duration { get; set; }
    public int Difficulty { get; set; }= 2;
    public IList<string> Tags { get; }
    public IList<string> VideoLinks { get; }
    public string OriginalArticleUrl { get; set; }
    public string SimilarArticlesUrl { get; set; }
    public MealModel(MealModelView source)
    {
       
        Id = ObjectId.GenerateNewId();
        Name = source.Name;
        ImageUrl = source.ImageUrl;
        Description = source.Description;
        Duration = source.Duration;
        Difficulty = source.Difficulty;
        Ingredients = new List<IngredientModel>(source.Ingredients.Select(i => new IngredientModel(i)));
        Steps = new List<StepModel>(source.Steps.Select(s => new StepModel(s)));
        Tags = new List<string>(source.Tags.Select(s => s.TagName));
        VideoLinks = new List<string>(source.VideoLinks);
        OriginalArticleUrl = source.OriginalArticleUrl;
        SimilarArticlesUrl = source.SimilarArticlesUrl;
    }
    public MealModel(MealModel source)
    {
       
        Id = source.Id;
        Name = source.Name;
        ImageUrl = source.ImageUrl;
        Description = source.Description;
        Duration = source.Duration;
        Difficulty = source.Difficulty;
        Ingredients = new List<IngredientModel>(source.Ingredients.Select(i => new IngredientModel(i)));
        Steps = new List<StepModel>(source.Steps.Select(s => new StepModel(s)));
        Tags = new List<string>(source.Tags.Select(s => s));
        VideoLinks = new List<string>(source.VideoLinks);
        OriginalArticleUrl = source.OriginalArticleUrl;
        SimilarArticlesUrl = source.SimilarArticlesUrl;
    }
    public MealModel(MealModelView source, ObjectId id)
    {
        Id = id;
        Name = source.Name;
        ImageUrl = source.ImageUrl;
        Description = source.Description;
        Duration = source.Duration;
        Difficulty = source.Difficulty;

        Ingredients = new List<IngredientModel>(source.Ingredients.Select(i => new IngredientModel(i)));
        Steps = new List<StepModel>(source.Steps.Select(s => new StepModel(s)));
        Tags = new List<string>(source.Tags.Select(s => s.TagName));
        VideoLinks = new List<string>(source.VideoLinks);
        OriginalArticleUrl = source.OriginalArticleUrl;
        SimilarArticlesUrl = source.SimilarArticlesUrl;
    }
    public MealModel()
    {
        
    }
}
public partial class MealModelView : ObservableObject
{
    [PrimaryKey]
    public ObjectId Id { get; set; } 

    [ObservableProperty]
    string name;
    [ObservableProperty]
    string originalArticleUrl;
    [ObservableProperty]
    string similarArticlesUrl;

    [ObservableProperty]
    string imageUrl = string.Empty;
    
    [ObservableProperty]
    ObservableCollection<string> modeImagesUrl;

    [ObservableProperty]
    string description;

    [ObservableProperty]
    string duration;

    [ObservableProperty]
    int difficulty = 2;

    [ObservableProperty]
    ObservableCollection<IngredientModelView> ingredients = new ObservableCollection<IngredientModelView>();

    [ObservableProperty]
    ObservableCollection<StepModelView> steps = new ObservableCollection<StepModelView>();

    [ObservableProperty]
    ObservableCollection<TagModelView> tags = new ObservableCollection<TagModelView>();

    [ObservableProperty]
    ObservableCollection<string> videoLinks = new();

    [ObservableProperty]
    bool isFavorite;
    [ObservableProperty]
    bool isBookMarked;

    // Constructor for copying properties from an existing MealModel
    public MealModelView(MealModel source)
    {
        Id = source.Id;
        Name = source.Name;
        ImageUrl = source.ImageUrl;
        
        Description = source.Description;
        Duration = source.Duration;
        Difficulty = source.Difficulty;

        Ingredients = new ObservableCollection<IngredientModelView>(source.Ingredients.Select(i => new IngredientModelView(i)));
        Steps = new ObservableCollection<StepModelView>(source.Steps.Select(s => new StepModelView(s)));
        Tags = new ObservableCollection<TagModelView>(source.Tags.Select(t => new TagModelView(t)));
        VideoLinks = new ObservableCollection<string>(source.VideoLinks);
        OriginalArticleUrl = source.OriginalArticleUrl;
        SimilarArticlesUrl = source.SimilarArticlesUrl;
    }

    // Default Constructor
    public MealModelView() { }

    // Override Equals to compare based on ObjectId
    public override bool Equals(object? obj)
    {
        if (obj is MealModelView other)
        {
            return this.Id == other.Id;
        }
        return false;
    }

    // Override GetHashCode to use ObjectId's hash code
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    
}
public class IngredientModel: EmbeddedObject
{

    public string Name { get; set; }
    public string Quantity { get; set; }
    public string Unit { get; set; }
    public string ImageUrl { get; set; }
    public IngredientModel(IngredientModelView source)
    {
        //Id = ObjectId.GenerateNewId();
        Name = source.Name;
        Quantity = source.Quantity;
        ImageUrl = source.ImageUrl;
    }
    public IngredientModel(IngredientModel source)
    {
        //Id = ObjectId.GenerateNewId();
        Name = source.Name;
        Quantity = source.Quantity;
        ImageUrl = source.ImageUrl;
    }

    public IngredientModel()
    {
        
    }
}

public partial class TagModelView: ObservableObject
{
    public ObjectId Id;
    [ObservableProperty]
    string tagName;
    public TagModelView(TagModel tag)
    {
        Id = tag.Id;
        TagName = tag.TagName;
    }
    public TagModelView(string tagName)
    {
        TagName = tagName;
    }
    public TagModelView()
    {
        
    }
}
public class TagModel: RealmObject
{
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
    
    public string TagName { get; set; }
    public TagModel()
    {
        
    }
    public TagModel(TagModelView tag)
    {
        Id = ObjectId.GenerateNewId();
        TagName = tag.TagName;
    }
    public TagModel(TagModel tag)
    {
        Id = tag.Id;
        TagName = tag.TagName;
    }
}
public class StepModel: EmbeddedObject
{
    public string StepNumber { get; set; }
    public string StepDescription { get; set; }
    public string StepImage { get; set; }
    public string StepDuration { get; set; }
    
    public StepModel(StepModelView source)
    {
        StepNumber = source.StepNumber;
        StepDescription = source.StepDescription;
        StepImage = source.StepImage;
        StepDuration = source.StepDuration;
    }
    public StepModel(StepModel source)
    {
        StepNumber = source.StepNumber;
        StepDescription = source.StepDescription;
        StepImage = source.StepImage;
        StepDuration = source.StepDuration;
    }
    public StepModel()
    {
        
    }
    
}

public partial class StepModelView : ObservableObject
{
    [ObservableProperty]
    string stepNumber;

    [ObservableProperty]
    string stepDescription;

    [ObservableProperty]
    string stepImage;

    [ObservableProperty]
    string stepDuration;

    // Constructor for copying properties from an existing StepModel
    public StepModelView(StepModel source)
    {
        StepNumber = source.StepNumber;
        StepDescription = source.StepDescription;
        StepImage = source.StepImage;
        StepDuration = source.StepDuration;
    }

    // Default Constructor
    public StepModelView() { }
    public StepModelView(string stepNumber, string stepDescription)
    {
        StepNumber = stepNumber;
        StepDescription = stepDescription;
    }

    
}

public partial class IngredientModelView : ObservableObject
{
    [PrimaryKey]
    public ObjectId Id { get; set; }

    [ObservableProperty]
    string name;

    [ObservableProperty]
    string quantity;
    [ObservableProperty]
    string unit ;

    [ObservableProperty]
    string imageUrl ;

    // Constructor for copying properties from an existing IngredientModel
    public IngredientModelView(IngredientModel source)
    {
        //Id = source.Id;
        Name = source.Name;
        Quantity = source.Quantity;
        ImageUrl = source.ImageUrl;
    }

    // Default Constructor
    public IngredientModelView() { }

    public IngredientModelView(string? name, string qty, string? unit)
    {
        Name = name;
        Quantity = qty;
        Unit = unit;
    }
}
public static class StepModelViewExtensions
{
    
    public static List<string> ToStepStringList(this IEnumerable<StepModelView> steps)
    {
        return steps.Select(step => $"{step.StepDuration} {step.StepDescription}".Trim()).ToList();
    }
    
    public static string ToStepString(this IEnumerable<StepModelView> steps)
    {

        // Join all ingredient strings with a new line separator
        return string.Join(Environment.NewLine, steps.Select(step =>
            $"{step.StepDuration} {step.StepDescription}".Trim()));
    }

    
    public static string ToStepString(this StepModelView step)
    {
        return $"{step.StepDuration} {step.StepDescription}".Trim();
    }
}
public static class IngredientModelViewExtensions
{
    
    public static List<string> ToIngredientStringList(this IEnumerable<IngredientModelView> ingredients)
    {
        return ingredients.Select(ingredient => $"{ingredient.Quantity} {ingredient.Unit} {ingredient.Name}".Trim()).ToList();
    }

    
    public static string ToIngredientString(this IEnumerable<IngredientModelView> ingredients)
    {
        
        // Join all ingredient strings with a new line separator
        return string.Join(Environment.NewLine, ingredients.Select(ingredient =>
            $"{ingredient.Quantity} {ingredient.Unit} {ingredient.Name}".Trim()));
    }
        
    public static string ToIngredientString(this IngredientModelView ingredient)
    {
        return $"{ingredient.Quantity} {ingredient.Unit} {ingredient.Name}".Trim();
    }
}
