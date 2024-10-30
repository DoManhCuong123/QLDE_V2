using DocumentFormat.OpenXml.InkML;
using Microsoft.Win32;
using OfficeOpenXml;
using QLDE_V2.Class;
using System.Data.SQLite;
using System.IO;
using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        QLDE_V2Db context = new QLDE_V2Db();
        string connectionString = "Data Source = test67.sqlite";
        public MainWindow()
        {
            InitializeComponent();
            var data = GetDataFromDatabase();
            trvQLDE.ItemsSource = data;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                ImportFromExcel(filePath);
            }
            var data = GetDataFromDatabase();
            trvQLDE.ItemsSource = data;
        }


        public void ImportFromExcel(string filePath)
        {
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    var checkKieuCauHoiQuery = "SELECT COUNT(*) FROM KieuCauHoitb";
                    using (var command = new SQLiteCommand(checkKieuCauHoiQuery, connection))
                    {
                        var count = (long)command.ExecuteScalar();
                        if (count == 0)
                        {
                            var insertKieuCauHoiQuery = @"
                    INSERT INTO KieuCauHoitb (NoiDungKC) VALUES 
                    ('BT thực hành trắc nghiệm'), 
                    ('BT lý thuyết'), 
                    ('BT tự luận')";
                            using (var insertCommand = new SQLiteCommand(insertKieuCauHoiQuery, connection))
                            {
                                insertCommand.ExecuteNonQuery();
                            }
                        }
                    }

                    // Kiểm tra và chèn giá trị mặc định cho MucDoCauHoitb
                    var checkMucDoCauHoiQuery = "SELECT COUNT(*) FROM MucDoCauHoitb";
                    using (var command = new SQLiteCommand(checkMucDoCauHoiQuery, connection))
                    {
                        var count = (long)command.ExecuteScalar();
                        if (count == 0)
                        {
                            var insertMucDoCauHoiQuery = @"
                    INSERT INTO MucDoCauHoitb (NoiDungMD) VALUES 
                    ('Dễ'), 
                    ('Trung bình'), 
                    ('Khó')";
                            using (var insertCommand = new SQLiteCommand(insertMucDoCauHoiQuery, connection))
                            {
                                insertCommand.ExecuteNonQuery();
                            }
                        }
                    }
                    foreach (var sheet in package.Workbook.Worksheets)
                    {
                        //import tên chương
                        var chuongName = sheet.Name;
                        var insertChuongQuery = "INSERT INTO Chuongtb (ChuongName) VALUES (@ChuongName)";
                        using (var command = new SQLiteCommand(insertChuongQuery, connection))
                        {
                            command.Parameters.AddWithValue("@ChuongName", chuongName);
                            command.ExecuteNonQuery();
                        }
                        //lấy IdChuong vừa import vào
                        var chuongIdQuery = "SELECT IdChuong FROM Chuongtb WHERE ChuongName = @ChuongName";
                        using (var command = new SQLiteCommand(chuongIdQuery, connection))
                        {
                            command.Parameters.AddWithValue("@ChuongName", chuongName);
                            var chuongId = (long)command.ExecuteScalar();
                            int rowCount = sheet.Dimension.Rows;
                            long? lastBaiId = null;
                            for (int row = 2; row <= rowCount; row++)
                            {
                                var baiName = sheet.Cells[row, 1].Text;
                                long baiId = 0;

                                //Import dữ liệu cột bảng BÀI trừ khoảng trống
                                if (!string.IsNullOrWhiteSpace(baiName))
                                {
                                    var insertBaiQuery = "INSERT INTO Baitb (BaiName, ChuongIdChuong) VALUES (@BaiName, @ChuongIdChuong)";
                                    using (var commandBai = new SQLiteCommand(insertBaiQuery, connection))
                                    {
                                        commandBai.Parameters.AddWithValue("@BaiName", baiName);
                                        commandBai.Parameters.AddWithValue("@ChuongIdChuong", chuongId);
                                        commandBai.ExecuteNonQuery();
                                    }

                                    //Lấy IdBai vừa import
                                    var baiIdQuery = "SELECT IdBai FROM Baitb WHERE BaiName = @BaiName AND ChuongIdChuong = @ChuongIdChuong";
                                    using (var commandBai = new SQLiteCommand(baiIdQuery, connection))
                                    {
                                        commandBai.Parameters.AddWithValue("@BaiName", baiName);
                                        commandBai.Parameters.AddWithValue("@ChuongIdChuong", chuongId);
                                        baiId = (long)commandBai.ExecuteScalar();
                                        lastBaiId = baiId; // Cập nhật biến tạm thời với giá trị baiId mới

                                    }
                                }

                                baiId = lastBaiId.Value;
                                string noiDungCH = sheet.Cells[row, 2].Value?.ToString();
                                string kieuCauHoi = sheet.Cells[row, 3].Value?.ToString().Trim();
                                string mucDoCauHoi = sheet.Cells[row, 4].Value?.ToString().Trim();

                                if (!string.IsNullOrWhiteSpace(noiDungCH))
                                {

                                    // Lấy KieuCauHoiId
                                    var kieuCauHoiIdQuery = "SELECT KieuCauHoiId FROM KieuCauHoitb WHERE NoiDungKC = @NoiDungKC";
                                    int kieuCauHoiId = 0;
                                    using (var commandKieu = new SQLiteCommand(kieuCauHoiIdQuery, connection))
                                    {
                                        commandKieu.Parameters.AddWithValue("@NoiDungKC", kieuCauHoi);
                                        var result = commandKieu.ExecuteScalar();
                                        if (result != null)
                                        {
                                            kieuCauHoiId = Convert.ToInt32(result);
                                        }
                                        else
                                        {
                                            throw new InvalidOperationException("KieuCauHoiId not found.");
                                        }
                                    }

                                    // Lấy MucDoCauHoiId
                                    var mucDoCauHoiIdQuery = "SELECT MucDoCauHoiId FROM MucDoCauHoitb WHERE NoiDungMD = @NoiDungMD";
                                    int mucDoCauHoiId = 0;
                                    using (var commandMucDo = new SQLiteCommand(mucDoCauHoiIdQuery, connection))
                                    {
                                        commandMucDo.Parameters.AddWithValue("@NoiDungMD", mucDoCauHoi);
                                        var result = commandMucDo.ExecuteScalar();
                                        if (result != null)
                                        {
                                            mucDoCauHoiId = Convert.ToInt32(result);
                                        }
                                        else
                                        {
                                            throw new InvalidOperationException("MucDoCauHoiId not found.");
                                        }
                                    }

                                    // Import dữ liệu cột bảng CÂU HỎI trừ khoảng trống
                                    var insertCauHoiQuery = "INSERT INTO CauHoitb (BaiIdBai, NoiDungCH, ChuongIdChuong, KieuCauHoiIdKCH, MucDoCauHoiIdMDCH) VALUES (@BaiIdBai, @NoiDungCH, @ChuongIdChuong, @KieuCauHoiIdKCH, @MucDoCauHoiIdMDCH)";
                                    using (var insertCommand = new SQLiteCommand(insertCauHoiQuery, connection))
                                    {
                                        insertCommand.Parameters.AddWithValue("@BaiIdBai", baiId);
                                        insertCommand.Parameters.AddWithValue("@NoiDungCH", noiDungCH);
                                        insertCommand.Parameters.AddWithValue("@ChuongIdChuong", chuongId);
                                        insertCommand.Parameters.AddWithValue("@KieuCauHoiIdKCH", kieuCauHoiId);
                                        insertCommand.Parameters.AddWithValue("@MucDoCauHoiIdMDCH", mucDoCauHoiId);
                                        insertCommand.ExecuteNonQuery();
                                    }
                                }

                            }
                        }
                    }
                }
            }
        }

        public List<Chuong> GetDataFromDatabase()
        {
            var chuongs = new List<Chuong>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var chuongQuery = "SELECT * FROM Chuongtb";
                using (var command = new SQLiteCommand(chuongQuery, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var chuong = new Chuong
                        {
                            IdChuong = reader.GetInt32(0),
                            ChuongName = reader.GetString(1)
                        };
                        chuongs.Add(chuong);
                    }
                }

                foreach (var chuong in chuongs)
                {
                    var baiQuery = "SELECT * FROM Baitb WHERE ChuongIdChuong = @ChuongIdChuong";
                    using (var command = new SQLiteCommand(baiQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ChuongIdChuong", chuong.IdChuong);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var bai = new Bai
                                {
                                    IdBai = reader.GetInt32(0),
                                    BaiName = reader.GetString(1),
                                    ChuongIdChuong = reader.GetInt32(2)
                                };
                                chuong.BaiS.Add(bai);
                            }
                        }
                    }

                    foreach (var bai in chuong.BaiS)
                    {
                        var cauHoiQuery = @"
                SELECT ch.CauHoiId, ch.NoiDungCH, ch.BaiIdBai, kch.NoiDungKc, mdch.NoiDungMD
                FROM CauHoitb ch
                JOIN KieuCauHoitb kch ON ch.KieuCauHoiIdKCH = kch.KieuCauHoiId
                JOIN MucDoCauHoitb mdch ON ch.MucDoCauHoiIdMDCH = mdch.MucDoCauHoiId
                WHERE ch.BaiIdBai = @BaiIdBai";
                        using (var command = new SQLiteCommand(cauHoiQuery, connection))
                        {
                            command.Parameters.AddWithValue("@BaiIdBai", bai.IdBai);
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var cauHoi = new CauHoi
                                    {
                                        CauHoiId = reader.GetInt32(0),
                                        NoiDungCH = reader.GetString(1),
                                        BaiIdBai = reader.GetInt32(2),
                                        KieuCauHoi = new KieuCauHoi { NoiDungKC = reader.GetString(3) },
                                        MucDoCauHoi = new MucDoCauHoi { NoiDungMD = reader.GetString(4) }
                                    };
                                    bai.CauHoiS.Add(cauHoi);
                                }
                            }
                        }
                    }
                }
            }
            return chuongs;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var TaodeWindow = new TaoDe();
            TaodeWindow.Show();    
        }


        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var LamDeWindow = new DisplayLamDe();
            LamDeWindow.Show();
        }


        private void btThem(object sender, RoutedEventArgs e)
        {


        }
    }
}