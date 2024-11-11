

namespace Foodie.Services;
public interface IMealService
{
    IList<MealModelView> AllMeals { get; }
    IList<TagModelView> AllTags
    {
        get;
    }
    void UpSertMeal(MealModelView meal);
    void UpdateMeal(MealModelView meal);
    void DeleteMeal(MealModelView meal);
    void GetMeals();
    void AddTag(TagModelView tag);
    void UpdateTag(TagModelView tag);
    void DeleteTag(TagModelView tag);
    void GetTags();
}


public class  MealService : IMealService
{
    Realm db;
    public IList<MealModelView> AllMeals { get; internal set; }
    public IList<TagModelView> AllTags { get; internal set; }
    public MealService()
    {
        GetMeals();
    }

    public async void GetMeals()
    {
        try
        {
            db = Realm.GetInstance(DataBaseService.GetRealm());

            AllMeals?.Clear();
            var realmMeals = db.All<MealModel>().ToList();

            if (realmMeals.Count < 1)
            {
                await SyncData();

            }
            var meals = realmMeals.Select(m => new MealModelView(m)).ToList();
            AllMeals = meals;
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    private async Task SyncData()
    {
        var officialDb = Realm.GetInstance(await DataBaseService.GetOfficialRealm());

        var officialMeals = officialDb.All<MealModel>().ToList();
        var officialTags = officialDb.All<TagModel>().ToList();
        AllMeals = officialMeals.Select(m => new MealModelView(m)).ToList();
        // Run the write operation in a background thread

        db.Write(() =>
        {
            foreach (var meal in officialMeals)
            {
                var newMeal = new MealModel(meal);
                db.Add(newMeal);
            }
        });
        db.Write(() =>
        {
            foreach (var tag in officialTags)
            {
                var newTag = new TagModel(tag);
                db.Add(newTag);
            }
        });

        string databaseFileName = "FoodieDB.realm";
        string targetPath = Path.Combine(FileSystem.AppDataDirectory, databaseFileName);
        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }
    }

    public void UpSertMeal(MealModelView meal)
    {
        try
        {
            var dbMeal = new MealModel(meal);
            db = Realm.GetInstance(DataBaseService.GetRealm());
            db.Write(() =>
            {
                db.Add(dbMeal);
            });
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    public void DeleteMeal(MealModelView meal)
    {
        try
        {
            db = Realm.GetInstance(DataBaseService.GetRealm());
            db.Write(() =>
            {
                var mealToUpdate = db.All<MealModel>().FirstOrDefault(x => x.Id == meal.Id);
                
                db.Remove(mealToUpdate);
            });
        }
        catch (Exception ex)
        {

            throw;
        }
    }
    public void UpdateMeal(MealModelView meal)
    {
        try
        {
            MealModel? dbMeal=null;
            bool isNewMeal = false;
            if(meal.Id == ObjectId.Empty)
            {
                isNewMeal = true;
                dbMeal = new MealModel(meal);
                meal.Id = dbMeal.Id;
            }
            
            db = Realm.GetInstance(DataBaseService.GetRealm());
            db.Write(() =>
            {
                if (isNewMeal)
                {
                    db.Add<MealModel>(dbMeal);
                    return;
                }

                var mealToUpdate = db.All<MealModel>().FirstOrDefault(x => x.Id == meal.Id);
                mealToUpdate = new(meal, meal.Id);
                db.Add(mealToUpdate, update: true);
            });

            GetMeals();
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    public void AddTag(TagModelView tag)
    {
        try
        {
            var dbMeal = new TagModel(tag);
            db = Realm.GetInstance(DataBaseService.GetRealm());
            db.Write(() =>
            {
                db.Add(dbMeal);
            });
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    public void UpdateTag(TagModelView tag)
    {
        try
        {
            bool isNewMeal = false;
            if (tag.Id != ObjectId.Empty)
            {
                isNewMeal = true;
            }
            var dbMeal = new TagModel(tag);
            db = Realm.GetInstance(DataBaseService.GetRealm());
            db.Write(() =>
            {
                if (isNewMeal)
                {
                    db.Add<TagModel>(dbMeal);
                    return;
                }

                var tagToUpdate = db.All<TagModel>().FirstOrDefault(x => x.Id == tag.Id);
                db.Add(tagToUpdate, update: true);
            });

            GetTags();
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    public void DeleteTag(TagModelView tag)
    {
        try
        {
            db = Realm.GetInstance(DataBaseService.GetRealm());
            db.Write(() =>
            {
                var tagToUpdate = db.All<TagModel>().FirstOrDefault(x => x.Id == tag.Id);

                db.Remove(tagToUpdate);
            });
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    public void GetTags()
    {
        try
        {
            db = Realm.GetInstance(DataBaseService.GetRealm());

            AllTags?.Clear();
            var realmMeals = db.All<TagModel>().ToList();
            var tags = realmMeals.Select(t => new TagModelView(t)).ToList();
            AllTags = tags;
        }
        catch (Exception ex)
        {

            throw;
        }
    }
}