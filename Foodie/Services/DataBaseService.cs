using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace Foodie.IServices;
public static class DataBaseService
{
    public static RealmConfiguration GetRealm()
    {
        string dbPath = string.Empty;

#if ANDROID
        dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\FFoodie");
#elif WINDOWS
        dbPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\FFoodie";
#endif
        
        if (!Directory.Exists(dbPath))
        {
            Directory.CreateDirectory(dbPath);
        }

        string filePath = Path.Combine(dbPath, "FoodieDB.realm");
        //File.Delete(filePath);
        return new RealmConfiguration(filePath);
    }
    
    public async static Task<RealmConfiguration> GetOfficialRealm()
    {
        string databaseFileName = "FoodieDB.realm";
        string targetPath = Path.Combine(FileSystem.AppDataDirectory, databaseFileName);
        // Copy the file from the app package to the writable location
        if (File.Exists(targetPath))
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync(databaseFileName);
            using var fileStream = new FileStream(targetPath, FileMode.Create, FileAccess.Write);
            await stream.CopyToAsync(fileStream);
        }

        return new RealmConfiguration(targetPath);
    }
}
