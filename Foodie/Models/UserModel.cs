using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodie.Models;
public class UserModel :RealmObject
{
    [PrimaryKey]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
    public string Name { get; set; } = "User_" + ObjectId.GenerateNewId().ToString();
    public DateTimeOffset DateCreated { get; set; } 
    public IList<MealModel> ListOfMeals { get; }
    public IList<MealModel> ListOfFavouriteMeals { get; }
    public IList<MealModel> ListOfBlackListedMeals { get; }
    public UserModel(UserModelView model)
    {
        DateCreated = model.DateCreated;
        ListOfMeals = new List<MealModel>(model.ListOfMeals.Select(m => new MealModel(m)));
        ListOfFavouriteMeals = new List<MealModel>(model.ListOfFavouriteMeals.Select(m => new MealModel(m)));
        ListOfBlackListedMeals = new List<MealModel>(model.ListOfBlackListedMeals.Select(m => new MealModel(m)));

    }
    public UserModel(UserModel model)
    {
        Id=model.Id;
        DateCreated = model.DateCreated;
        ListOfMeals = new List<MealModel>(model.ListOfMeals.Select(m => new MealModel(m)));
        ListOfFavouriteMeals = new List<MealModel>(model.ListOfFavouriteMeals.Select(m => new MealModel(m)));

        ListOfBlackListedMeals = new List<MealModel>(model.ListOfBlackListedMeals.Select(m => new MealModel(m)));

    }
    public UserModel()
    {
        
    }
    public UserModel(UserModelView model, ObjectId id)
    {
        Id = id;
        DateCreated = model.DateCreated;
        ListOfMeals = new List<MealModel>(model.ListOfMeals.Select(m => new MealModel(m)));
        ListOfFavouriteMeals = new List<MealModel>(model.ListOfFavouriteMeals.Select(m => new MealModel(m)));

        ListOfBlackListedMeals = new List<MealModel>(model.ListOfBlackListedMeals.Select(m => new MealModel(m)));

    }
}

public partial class UserModelView :ObservableObject
{
    public ObjectId Id;
    [ObservableProperty]
    string name = "User_" + ObjectId.GenerateNewId().ToString();

    [ObservableProperty]
    DateTimeOffset dateCreated;

    [ObservableProperty]
    ObservableCollection<MealModelView> listOfMeals;

    [ObservableProperty]
    ObservableCollection<MealModelView> listOfFavouriteMeals;

    [ObservableProperty]
    ObservableCollection<MealModelView> listOfBlackListedMeals ;

    public UserModelView(UserModel model)
    {
        Id = model.Id;
        DateCreated = model.DateCreated;
        ListOfMeals = new ObservableCollection<MealModelView>(model.ListOfMeals.Select(m => new MealModelView(m)));
        ListOfFavouriteMeals = new ObservableCollection<MealModelView>(model.ListOfFavouriteMeals.Select(m => new MealModelView(m)));
        
        ListOfBlackListedMeals = new ObservableCollection<MealModelView>(model.ListOfBlackListedMeals.Select(m=>new MealModelView(m)));
        
    }
    public UserModelView()
    {
        
    }

}
