using Ion.Core;
using System.Windows;

namespace Ion.Tools.Rename;

public partial class App() : AppTool()
{
#pragma warning disable IDE0060
    [STAThread]
    public static void Main(params string[] arguments)
    {
        new App().Do(i => { i.InitializeComponent(); i.Run(); });
    }
#pragma warning restore
}