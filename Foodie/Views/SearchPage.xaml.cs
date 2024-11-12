namespace Foodie.Views;

public partial class SearchPage : ContentPage
{
	public SearchPage(MealsViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        BindingContext = viewModel;
    }

    public MealsViewModel ViewModel { get; }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel.GetImage();
        ViewModel.CurrentPage = PageEnum.SearchPage;
    }
}