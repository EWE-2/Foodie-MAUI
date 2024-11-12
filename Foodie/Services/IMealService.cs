

using System.Diagnostics;
using System.Text.Json;

namespace Foodie.Services;
public interface IMealService
{
    UserModelView CurrentUser { get; set; }
    IList<MealModelView> AllMeals { get; }
    IList<TagModelView> AllTags
    {
        get;
    }
    Action<bool> RefreshComplete { get; set; }
    void UpSertUser(UserModelView userView);
    void UpdateMeal(MealModelView meal);
    void DeleteMeal(MealModelView meal);
    void GetMeals();
    void AddTag(TagModelView tag);
    void UpdateTag(TagModelView tag);
    void DeleteTag(TagModelView tag);
    void GetTags();

    Task ExportToJSONAsync();
}


public class  MealService : IMealService
{
    Realm db;
    public IList<MealModelView> AllMeals { get; internal set; }
    public IList<TagModelView> AllTags { get; internal set; }
    public UserModelView CurrentUser { get; set; }
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

            GetUserSetup();

            await SyncData(); //commented for tests TODO:UNCOMMENT FOR RELEASE
            return;
            var Meals = db.All<MealModel>().ToList();
            List<MealModelView>? meals = realmMeals.Select(m => new MealModelView(m)).ToList();
            AllMeals = meals;
            
            GetTagsSetUp();
            RefreshComplete?.Invoke(true);
            
            
            return;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    private void GetTagsSetUp()
    {
        var tagsdb = db.All<TagModel>().ToList();
        var tags = tagsdb.Select(t => new TagModelView(t)).ToList();
        AllTags = tags;
    }

    private void GetUserSetup()
    {
        var user = db.All<UserModel>();
        if (user.Any())
        {
            var currentUser = db.All<UserModel>().FirstOrDefault();
            CurrentUser = new UserModelView(currentUser);
        }
        else
        {
            var newUser = new UserModel();
            CurrentUser = new UserModelView(newUser);
            db.Write(() =>
            {
                db.Add(newUser);
            });
        }
    }

    public Action<bool> RefreshComplete { get; set; } = (status) => {
        Console.WriteLine($"Refresh completed with status: {status}");
    };

    public async Task SyncData()
    {
        // Define URLs for the JSON files (replace these with actual URLs)
        string mealsUrl = @"https://github.com/YBTopaz8/Foodie-MAUI/tree/master/db/Meals.json"; 
        string tagsUrl = @"https://github.com/YBTopaz8/Foodie-MAUI/tree/master/db/Tags.json";   

        // Define the path where the downloaded files will be saved temporarily
        string mealsFilePath = Path.Combine(FileSystem.AppDataDirectory, "Meals.json");
        string tagsFilePath = Path.Combine(FileSystem.AppDataDirectory, "Tags.json");
        try
        {

            await DownloadFileAsync(mealsUrl, mealsFilePath);

            // Download the Tag JSON file
            await DownloadFileAsync(tagsUrl, tagsFilePath);
            await ImportFromJSONAsync();

            RefreshComplete?.Invoke(true);
            return;
            
        }
        catch (Exception ex)
        {
            //normally ex should be no files found, which is ok for dev, not not ok for user

            var officialDb = Realm.GetInstance(DataBaseService.GetRealm());

            var officialMeals = officialDb.All<MealModel>().ToList();

            var officialTags = officialDb.All<TagModel>().ToList();
            AllMeals = officialMeals.Select(m => new MealModelView(m)).ToList();
            AllTags = officialTags.Select(m => new TagModelView(m)).ToList();
            RefreshComplete?.Invoke(true);
            return;
        }

    }

    // Helper method to download files
    private async Task DownloadFileAsync(string url, string filePath)
    {
        using HttpClient client = new HttpClient();
        using var response = await client.GetAsync(url);
        //response.EnsureSuccessStatusCode(); // Ensure successful response


        var directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var stream = await response.Content.ReadAsStreamAsync();
        using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await stream.CopyToAsync(fileStream);
    }

    // Helper method to deserialize JSON data into a generic type
    private async Task<T> DeserializeJsonAsync<T>(string filePath)
    {
        string jsonData = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<T>(jsonData);
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

    public void UpSertUser(UserModelView userView)
    {
        try
        {
            UserModel? dbUser=null;
            bool isNewMeal = false;
            if(userView.Id == ObjectId.Empty)
            {
                isNewMeal = true;
                dbUser = new UserModel(userView);
                userView.Id = dbUser.Id;
            }
            
            db = Realm.GetInstance(DataBaseService.GetRealm());
            db.Write(() =>
            {
                if (isNewMeal)
                {
                    db.Add<UserModel>(dbUser);
                    return;
                }

                var userToUpdate = db.All<UserModel>().FirstOrDefault(x => x.Id == userView.Id);
                userToUpdate = new UserModel(userView, userView.Id);
                db.Add(userToUpdate, update: true);
            });

            GetMeals();
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
    public async Task ExportToJSONAsync()
    {
        
        var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments ) + @"\FFoodie";
        if (!Directory.Exists(documentsPath))
        {
            Directory.CreateDirectory(documentsPath);
        }
        string mealsFilePath = Path.Combine(documentsPath, "Meals.json");
        string tagsFilePath = Path.Combine(documentsPath, "Tags.json");

        var mealsJson = JsonSerializer.Serialize(AllMeals);
        var tagsJson = JsonSerializer.Serialize(AllTags);

        await File.WriteAllTextAsync(mealsFilePath, mealsJson);
        await File.WriteAllTextAsync(tagsFilePath, tagsJson);

        Debug.WriteLine("Export complete!");
    }

    
    public async Task ImportFromJSONAsync()
    {
        
        var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\FFoodie";

        string mealsFilePath = Path.Combine(documentsPath, "Meals.json");
        string tagsFilePath = Path.Combine(documentsPath, "Tags.json");
        if (!File.Exists(mealsFilePath) || !File.Exists(tagsFilePath))
        {
            return;
        }
        var mealsJson = await File.ReadAllTextAsync(mealsFilePath);
        var tagsJson = await File.ReadAllTextAsync(tagsFilePath);

        AllMeals = JsonSerializer.Deserialize<List<MealModelView>>(mealsJson);
        AllTags = JsonSerializer.Deserialize<List<TagModelView>>(tagsJson);
        if (AllMeals.Count < 1)
        {
            return;
        }
        db.Write(() =>
        {
            foreach (var meal in AllMeals)
            {
                Debug.WriteLine(meal.ImageUrl);
                var existingMeal = db.Find<MealModel>(meal.Id);
                if (existingMeal is null)
                {
                    var newMeal = new MealModel(meal);
                    db.Add(newMeal);
                }
            }

            db.RemoveAll<TagModel>();
            foreach (var tag in AllTags)
            {
                var newTag = new TagModel(tag);
                db.Add(newTag);
            }
        });

    }


}

public static class Utils
{
    public async static Task<string> GetMealImagePath(MealModelView meal, string imageUrl)
    {
        var mealImgPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\FFoodie\Image\";
       
        if (!Directory.Exists(mealImgPath))
        {
            Directory.CreateDirectory(mealImgPath);
        }

        // Get the file name from the URL (e.g., image.jpg)
        var fileName = Path.GetFileName(new Uri(imageUrl).LocalPath);
        var filePath = Path.Combine(mealImgPath, fileName);

        using HttpClient client = new HttpClient();
        try
        {
            var response = await client.GetAsync(imageUrl);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error downloading image: {response.StatusCode}");
            }

            var imageBytes = await response.Content.ReadAsByteArrayAsync();
            if (imageBytes.Length > 0)
            {
                await File.WriteAllBytesAsync(filePath, imageBytes);
            }
            else
            {
                Console.WriteLine("Image download failed: No data received.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to download and load image: {ex.Message}");
        }

        meal.ImageUrl = filePath;
        return filePath;
    }
}