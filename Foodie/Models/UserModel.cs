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

}
