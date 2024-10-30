using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Windows;

namespace QLDE_V2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            DatabaseFacade fecade = new DatabaseFacade(new QLDE_V2Db());
            fecade.EnsureCreated();

        }
    }       
}
