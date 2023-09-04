using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace WeiboBlog;

public partial class WebLoader : ContentPage
{
	string addr;
	public WebLoader(string address)
	{
		addr = address;
		InitializeComponent();
        this.Loaded += WebLoader_Loaded;
	}

    private async void WebLoader_Loaded(object sender, EventArgs e)
    {
		webview.Source = addr;
        webview.Navigated += Webview_Loaded;
        await Task.Delay(1000);
        webview.HeightRequest = Height*2/3;
    }
    bool isScrolling = true;
    private async  void Webview_Loaded(object sender, EventArgs e)
    {
        while (isScrolling)
        {
            scrollBottom();
            await Task.Delay(300);
        }
    }
    string prev = "";
    int count = 0;
    async void scrollBottom()
	{
        string getHeight = "document.documentElement.scrollTop";

        string js = "window.scrollTo(0, document.body.scrollHeight);";
        var height = await webview.EvaluateJavaScriptAsync(getHeight);
        await webview.EvaluateJavaScriptAsync(js);
        if (prev == height)
        {
            count++;
        }
        else
        {
            count = 0;
        }
        prev = height;
        if (count == 10)
        {
            Button_Clicked(null,null);
        }
        

    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        isScrolling = false;
        string js = @"
            function getLinks(){
                str='';
                a=document.querySelectorAll('li');
                a.forEach(function(item) {
                   // try{
                        b=item.childNodes[0];
                        str+=b.getAttribute('data-link');
                        str+='*';
               // }catch(e){}
                
                });
               return str;
            }
            getLinks()
            
        ";
        var result=await webview.EvaluateJavaScriptAsync(js);
        await Task.Delay(100);
        
        while (result == null ) {

            result = await webview.EvaluateJavaScriptAsync("getLinks()");
            await Task.Delay(100);
        }
        if (result == "null")
        {
            await DisplayAlert("Error", "maybe this platform is not support", "OK");
        }


        foreach (var item in result.Split("*"))
        {
            if (item == "null" ||item=="") {
                continue;
            }
            links.Add(item);
        }
        //await DisplayAlert("links",result,"OK");
        getPassage();

    }
    
    List<string> links = new List<string>();
    async void getPassage() {
        label.IsVisible = true;
        all = links.Count;
        webview.IsVisible = false;
        button.IsVisible = false;
        List<WebView> list = new List<WebView>();
        for(int num=0;num<5;num++) { 
            WebView webview2 = new WebView();
            list.Add(webview2);
            layout.Add(webview2);
            await Task.Delay(120);//这个不加就会报错，我也不知道为什么
            webview2.Navigated += async (s,o)=> {
                WebView web = (WebView)s;
                string js = @"
                function getData(){
                    content=document.getElementsByClassName(""content b-txt1"")[0].innerHTML;
                    time=document.getElementsByClassName(""time"")[0].innerHTML;
                    title=document.getElementsByClassName(""new-head"")[0].childNodes[1].innerHTML;
                    return time+""*#$%""+title+""*#$%""+content;
                }
                getData();";
                string result = await web.EvaluateJavaScriptAsync(js);
                var result2 = result.Split("*#$%");
                string time, title, content;
                time= result2[0];
                title= result2[1];
                content= result2[2];

                PassageData data = new PassageData() {
                    date = DateTime.Parse(time),
                    html = Regex.Unescape( content),
                    title = title,
                };
                passages.Add(data);
                increase();
                if (links.Count != 0)
                {
                    web.Source = getLast();
                }
                if (passages.Count == all)
                {
                    //done
                    foreach(var i in list)
                    {
                        i.IsVisible = false;
                    }
                    handle();
                }
            };

        }
        //await Task.Delay();
        foreach (var i in list)
        {
            i.HeightRequest = Height / 6;
           
            if (links.Count != 0)
            {
                i.Source = getLast();
            }
            //await i.EvaluateJavaScriptAsync("");
        }
    }
    // 设置剪切板的数据
    private async void SetClipboard(string text)
    {
        await Clipboard.Default.SetTextAsync(text);
    }
    string html2;
    void handle()
    {
        passages.Sort();
        StringBuilder stringBuilder = new StringBuilder();
        //add print button
        stringBuilder.Append("<button onclick=\"" +
            "window.print()" +
            "\" id=\"printBtn\">PRINT</button>" +
            "<script>window.print()</script>");
        foreach(var i in passages)
        {
            
            stringBuilder.Append($"<h1>{i.title}</h1><br><h3>{i.date.ToString()}<h3><div>{i.html}</div><br><br>");

        }
        webview.IsVisible = true;
        webview.Navigated += Webview_Navigated;
        html2= stringBuilder.ToString();
        var body = html2;
        html2 = $"<head><title>BEIBO DOWNLOADER</title><meta charset=\"utf-8\"></head><body><script>document.body.style.width=\"100vw\"</script>{html2}</body>";
        string head;
        
        webview.Navigated += async (a, b) =>
        {
            string head=await webview.EvaluateJavaScriptAsync("document.head");
            //html2 = $"{head}<script>document.body.style.width=\"100vw\"</script><body>{body}</body>";
            string js = $"document.body.innerHTML=String.raw`<body>{body}</body>`;";
            var re=await webview.EvaluateJavaScriptAsync(js);

            await Task.Delay(1000);
            await webview.EvaluateJavaScriptAsync(js);

        };
        webview.Source = "https://blog.sina.cn/";

        printBtn.IsVisible = true;
        copy.IsVisible = true;
        save.IsVisible = true;
        thread = new Thread(() =>
        {
            try
            {
startServer();
            }catch (Exception ex)
            {

            }
            
        });
        thread.Start();
        //this.Disappearing += (s, o) => { thread.Join(); };
        

    }



    private async void Webview_Navigated(object sender, WebNavigatedEventArgs e)
    {
        try
        {
            Uri uri = new Uri("http://127.0.0.1:8080");
            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception ex)
        {
            // An unexpected error occurred. No browser may be installed on the device.
            await DisplayAlert("Error", ex.ToString(), "OK");
        }
    }

    string getLast()
    {
        int c=links.Count-1;
        string obj = links[c];
        links.RemoveAt(c);
        return obj;
    }
    List<PassageData> passages = new List<PassageData>();
    int numDone = 0;
    int all;
    void increase() { 
    numDone++;
        label.Text = $"{numDone}/{all} Done";
    }

    private void Button_Clicked_1(object sender, EventArgs e)
    {
        Webview_Navigated(this,null);
    }

    private void Button_Clicked_2(object sender, EventArgs e)
    {
        //copy source code
        SetClipboard(html2);
    }
    Thread thread;
    void startServer()
    {
        var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
        listener.Bind(localEndPoint);
        listener.Listen(10);

        Console.WriteLine("Server started. Listening on http://127.0.0.1:8080");

        while (true)
        {
            Console.WriteLine("Waiting for a connection...");
            var handler = listener.Accept();
            Console.WriteLine($"Connection from {handler.RemoteEndPoint}");

            var buffer = new byte[1024];
            var bytesReceived = handler.Receive(buffer);
            var request = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
            Console.WriteLine($"Request received:\n{request}");

            var response = $"HTTP/1.1 200 OK\r\nContent-Type: text/html\r\n\r\n {html2}";
            var responseBytes = Encoding.UTF8.GetBytes(response);
            handler.Send(responseBytes);
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
    }

    private void Button_Clicked_3(object sender, EventArgs e)
    {
        html2 = "OK";
        new Thread(startServer).Start();
    }

    private async  void save_Clicked(object sender, EventArgs e)
    {
        string fn = "source_code.txt";
        string file = Path.Combine(FileSystem.CacheDirectory, fn);

        File.WriteAllText(file, html2);

        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "分享源代码文件",
            File = new ShareFile(file)
        });

    }
}
class PassageData:IComparable<PassageData> 

{
    public DateTime date;
    public string title;
    public string html { set { _html = value.Replace("<wbr>", " ").Replace("http","https"); }
        get { return _html; } }
    string _html;

    public int CompareTo(PassageData other)
    {
        return date.CompareTo(other.date);
    }
    
}
