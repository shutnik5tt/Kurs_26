using System.Windows;
using BookshopApp.Data;

namespace BookshopApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            using var db = new AppDbContext();
            db.Database.EnsureCreated();
        }
    }
}
