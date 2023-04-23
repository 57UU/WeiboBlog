namespace WeiboBlog;
public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

    //default
    private void Button_Clicked(object sender, EventArgs e)
    {
        entry.Text = "https://blog.sina.cn/dpool/blog/alanlan#type=-1";
    }

    private void Button_Clicked_1(object sender, EventArgs e)
    {
        var web=new WebLoader(entry.Text);
        Navigation.PushAsync(web);
    }
}

