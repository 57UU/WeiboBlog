namespace WeiboBlog;
public partial class MainPage : ContentPage
{

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
    public const string help = """
            ①把博客主页的链接复制到文本框，点击"start"
            ②当主页全部加载完全时点击"AllLoaded"
            ③等待内容爬取（最上方有进度显示）
            ④以下步骤任选一个即可：
             -->直接截长图
             -->点击"External Browser"，保存为PDF即可(图片无法获取)
             -->点击"Copy Source Code"，然后在*电脑浏览器*上打开属于"新浪博客"任一界面，打开开发工具("F12")，点击"元素"("element")，对开头为"<html"的元素右键点击"编辑为HTML"，将剪切板中的文本复制进去替换原来的所有文本，最后打印为PDF即可(ctrl+P)
            """;
    private void Button_Clicked_2(object sender, EventArgs e)
    {
        //how to use


        DisplayAlert("HELP", help, "OK");
    }

    //about
    private async void Button_Clicked_3(object sender, EventArgs e)
    {
        try
        {
            Uri uri = new Uri("https://github.com/57UU/WeiboBlog");
            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception ex)
        {
            // An unexpected error occurred. No browser may be installed on the device.
            await DisplayAlert("Error", ex.ToString(), "OK");
        }
    }
}

