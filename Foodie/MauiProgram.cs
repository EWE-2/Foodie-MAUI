

using UraniumUI;

namespace Foodie;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiCommunityToolkit(options =>
            {

                //options.SetShouldSuppressExceptionsInAnimations(true);
                //options.SetShouldSuppressExceptionsInBehaviors(true);
                //options.SetShouldSuppressExceptionsInConverters(true);            

            })
            .ConfigureSyncfusionToolkit()
            .UseMauiApp<App>()
            .UseUraniumUI()
            .UseUraniumUIMaterial()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddMaterialSymbolsFonts();
                fonts.AddFontAwesomeIconFonts();
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif
        builder.Services.AddSingleton<HomeD>();
        
        
        builder.Services.AddSingleton<HomePage>();
        builder.Services.AddSingleton<SearchPage>();
        builder.Services.AddSingleton<SingleMeal>();
        builder.Services.AddSingleton<FavoriteMeals>();
        
        
        builder.Services.AddSingleton<MealsViewModel>();


        builder.Services.AddSingleton<IMealService, MealService>();
        return builder.Build();
    }
}
