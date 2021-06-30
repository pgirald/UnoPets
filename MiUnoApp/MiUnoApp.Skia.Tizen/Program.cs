using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace MiUnoApp.Skia.Tizen
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new TizenHost(() => new MiUnoApp.App(), args);
            host.Run();
        }
    }
}
