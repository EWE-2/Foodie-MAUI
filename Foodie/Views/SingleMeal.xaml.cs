namespace Foodie.Views;

public partial class SingleMeal : ContentPage
{
	public SingleMeal(MealsViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        BindingContext = viewModel;
        //viewW.Source = new Uri("https://www.youtube.com/watch?v=nYboXPsTRts&t=1711s");
    }

    public MealsViewModel ViewModel { get; }


}