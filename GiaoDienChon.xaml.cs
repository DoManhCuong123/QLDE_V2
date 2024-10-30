using QLDE_V2.Class;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QLDE_V2
{
    /// <summary>
    /// Interaction logic for GiaoDienChon.xaml
    /// </summary>
    public partial class GiaoDienChon : UserControl
    {
        QLDE_V2Db context = new QLDE_V2Db();
        string connectionString = "Data Source = test67.sqlite";
        public event Action<int, int> BatDauThi;

        public GiaoDienChon()
        {
            InitializeComponent();
            LoadMaDeToComboBox();
        }


        private void LoadMaDeToComboBox()
        {
            var maDeList = new List<KhoDe>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT MaDe FROM KhoDetb";
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var made = new KhoDe
                            {
                                MaDe = reader.GetInt32(0),
                            };
                            maDeList.Add(made);
                        }
                    }
                }
            }
            // Gán danh sách mã đề vào ComboBox
            comboBoxMaDe.ItemsSource = maDeList;
            comboBoxMaDe.DisplayMemberPath = "MaDe"; 
            comboBoxMaDe.SelectedValuePath = "MaDe";      
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int selectedMaDe = (int)comboBoxMaDe.SelectedValue;
            int selectedTime = int.Parse(txtTime.Text);
            BatDauThi?.Invoke(selectedMaDe, selectedTime);
        }
    }
}
