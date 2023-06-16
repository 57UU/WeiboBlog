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

    private void Button_Clicked_2(object sender, EventArgs e)
    {
        //how to use
        string help="""
            ①把博客主页的链接复制到文本框，点击start
            ②当主页全部加载完全时点击AllLoaded
            ③等待内容爬取（最上方有进度显示）
            ④点击External Browser
            ⑤点击网页上左上角的PRINT，保存为PDF即可
            """;

        DisplayAlert("HELP", help, "OK");
    }
}

