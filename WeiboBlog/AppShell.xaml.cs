namespace WeiboBlog;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
        DisplayAlert("HELP", MainPage.help, "OK");
    }
}
