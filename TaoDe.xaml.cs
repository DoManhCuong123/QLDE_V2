using DocumentFormat.OpenXml.Office2013.Excel;
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
using System.Windows.Shapes;

namespace QLDE_V2
{

    public partial class TaoDe : Window
    {
        QLDE_V2Db context = new QLDE_V2Db();
        string connectionString = "Data Source = test67.sqlite";
        public TaoDe()
        {
            InitializeComponent();
        }


        private void btTaoDe_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtNumber.Text, out int socau))
            {
                MessageBox.Show("Vui lòng nhập một số câu hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            int chuong = cbchuong.SelectedIndex;
            int KCH = cbkieucauhoi.SelectedIndex;
            int dokho = cbdokho.SelectedIndex;
            string tende = txtTenDe.Text;
            int made = int.Parse(txtMaDe.Text);
            GenerateRandomExamFilter(socau, chuong, KCH, dokho, tende, made);    
            Close();
        }


        public void GenerateExam(int numberOfQuestions, string name, int made)
        {
            var cauHoiList = new List<(long CauHoiId, string NoiDungCH)>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var getCauHoiQuery = "SELECT CauHoiId, NoiDungCH FROM CauHoitb";
                using (var command = new SQLiteCommand(getCauHoiQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            cauHoiList.Add((reader.GetInt64(0), reader.GetString(1)));
                        }
                    }
                }

                if (numberOfQuestions > cauHoiList.Count)
                {
                    MessageBox.Show("Số lượng câu hỏi vượt quá yêu câu hỏi có sẵn.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                // Tạo danh sách câu hỏi ngẫu nhiên
                var random = new Random();
                var randomCauHoiList = cauHoiList.OrderBy(x => random.Next()).Take(numberOfQuestions).ToList();

                // Tạo đề thi mới
                var insertDeQuery = "INSERT INTO KhoDetb (MaDe, NoiDungDe) VALUES (@MaDe, @NoiDungDe)";
                int maDeId = 0;
                using (var command = new SQLiteCommand(insertDeQuery, connection))
                {
                    command.Parameters.AddWithValue("@MaDe", made);
                    command.Parameters.AddWithValue("@NoiDungDe", name);
                    command.ExecuteNonQuery();
                }

                // Sau khi chèn, bạn muốn lấy lại MaDe
                var selectDeQuery = "SELECT MaDe FROM KhoDetb WHERE MaDe = @MaDe";
                using (var command = new SQLiteCommand(selectDeQuery, connection))
                {
                    command.Parameters.AddWithValue("@MaDe", made);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read()) // Kiểm tra nếu có kết quả
                        {
                            maDeId = reader.GetInt32(0); // Lấy giá trị của MaDe
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy mã đề vừa chèn.");
                        }
                    }
                }

                // Liên kết câu hỏi với đề thi
                foreach (var cauHoi in randomCauHoiList)
                {
                    var insertDeCauHoiQuery = "INSERT INTO CauHoiDetb (DeIdDe, CauHoiIdCauHoi) VALUES (@DeIdDe, @CauHoiIdCauHoi)";
                    using (var command = new SQLiteCommand(insertDeCauHoiQuery, connection))
                    {
                        command.Parameters.AddWithValue("@DeIdDe", maDeId);
                        command.Parameters.AddWithValue("@CauHoiIdCauHoi", cauHoi.CauHoiId);
                        command.ExecuteNonQuery();
                    }
                }
                var examWindow = new DisplayMaDe(maDeId, randomCauHoiList);
                examWindow.Show();
            }
        }


        public void GenerateRandomExamFilter(int numberOfQuestions, int chapter, int questiontype, int difficult, string name, int made)
        {

            if (chapter == -1 && questiontype == -1 && difficult == -1)
            {
                GenerateExam(numberOfQuestions, name, made);
            }
            else
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    var cauHoiList = new List<(long CauHoiId, string NoiDungCH)>();
                    var getCauHoiQuery = @"
                SELECT CauHoiId, NoiDungCH 
                FROM CauHoitb 
                WHERE ChuongIdChuong = @ChuongIdChuong";
                    if (difficult > 0)
                        getCauHoiQuery += " AND MucDoCauHoiIdMDCH = @MucDoCauHoiIdMDCH";

                    if (questiontype > 0)
                        getCauHoiQuery += " AND KieuCauHoiIdKCH = @KieuCauHoiIdKCH";

                    // Chuẩn bị lệnh truy vấn SQL
                    using (var command = new SQLiteCommand(getCauHoiQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ChuongIdChuong", chapter + 1);

                        if (difficult != 0)
                            command.Parameters.AddWithValue("@MucDoCauHoiIdMDCH", difficult + 1);

                        if (questiontype != 0)
                            command.Parameters.AddWithValue("@KieuCauHoiIdKCH", questiontype + 1);

                        // Thực thi truy vấn và đọc dữ liệu
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cauHoiList.Add((reader.GetInt64(0), reader.GetString(1)));
                            }
                        }
                    }
                    // Kiểm tra số lượng câu hỏi yêu cầu không vượt quá số lượng câu hỏi có sẵn
                    if (numberOfQuestions > cauHoiList.Count)
                    {
                        MessageBox.Show("Số lượng câu hỏi vượt quá số lượng câu hỏi có sẵn.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return; // Dừng hàm nếu không đủ câu hỏi
                    }

                    // Tạo danh sách câu hỏi ngẫu nhiên
                    var random = new Random();
                    var randomCauHoiList = cauHoiList.OrderBy(x => random.Next()).Take(numberOfQuestions).ToList();

                    // Tạo đề thi mới trong bảng KhoDetb
                    var insertDeQuery = "INSERT INTO KhoDetb (MaDe, NoiDungDe) VALUES (@MaDe, @NoiDungDe)";
                    int maDeId = 0;
                    using (var command = new SQLiteCommand(insertDeQuery, connection))
                    {
                        command.Parameters.AddWithValue("@MaDe", made);
                        command.Parameters.AddWithValue("@NoiDungDe", name);
                        command.ExecuteNonQuery();
                    }

                    // Sau khi chèn, bạn muốn lấy lại MaDe
                    var selectDeQuery = "SELECT MaDe FROM KhoDetb WHERE MaDe = @MaDe";
                    using (var command = new SQLiteCommand(selectDeQuery, connection))
                    {
                        command.Parameters.AddWithValue("@MaDe", made);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // Kiểm tra nếu có kết quả
                            {
                                maDeId = reader.GetInt32(0); // Lấy giá trị của MaDe
                            }
                            else
                            {
                                MessageBox.Show("Không tìm thấy mã đề vừa chèn.");
                            }
                        }
                    }

                    // Liên kết câu hỏi với đề thi trong bảng CauHoiDetb
                    foreach (var cauHoi in randomCauHoiList)
                    {
                        var insertDeCauHoiQuery = "INSERT INTO CauHoiDetb (DeIdDe, CauHoiIdCauHoi) VALUES (@DeIdDe, @CauHoiIdCauHoi)";
                        using (var command = new SQLiteCommand(insertDeCauHoiQuery, connection))
                        {
                            command.Parameters.AddWithValue("@DeIdDe", maDeId);
                            command.Parameters.AddWithValue("@CauHoiIdCauHoi", cauHoi.CauHoiId);
                            command.ExecuteNonQuery();
                        }
                    }
                    // Hiển thị đề thi vừa tạo
                    var examWindow = new DisplayMaDe(maDeId, randomCauHoiList);
                    examWindow.Show();
                } 
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            cbchuong.SelectedIndex = -1;
            cbdokho.SelectedIndex = -1;
            cbkieucauhoi.SelectedIndex = -1;
        }

        private void btTaoDeRand_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtNumber.Text, out int socau))
            {
                MessageBox.Show("Vui lòng nhập một số câu hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int made = int.Parse(txtMaDe.Text);
            GenerateExamRand(socau,made);
        }


        public class PhepToan
        {
            public string PhepTinh { get; set; }
        }


        public PhepToan TaoPhepTinhNgauNhien(int a, int b)
        {
            Random random = new Random();
            PhepToan cauHoi = new PhepToan();
            int phepToan = random.Next(1, 5); // Tạo ngẫu nhiên số từ 1 đến 4 để chọn phép tính

            switch (phepToan)
            {
                case 1: // Phép cộng
                    cauHoi.PhepTinh = $"{a} + {b} = ";
                    break;
                case 2: // Phép trừ
                    cauHoi.PhepTinh = $"{a} - {b} = ";
                    break;
                case 3: // Phép nhân
                    cauHoi.PhepTinh = $"{a} * {b} = ";
                    break;
                case 4: // Phép chia
                        // Kiểm tra chia cho 0 để tránh lỗi
                    if (b != 0)
                    {
                        cauHoi.PhepTinh = $"{a} : {b} = ";

                    }
                    else
                    {
                        // Nếu chia cho 0 thì mặc định phép cộng
                        cauHoi.PhepTinh = $"{a} + {b} = ";
                    }
                    break;
            }
            return cauHoi;
        }


        public void LuuPhepTinhVaoCSDL(PhepToan cauHoi, int maDe)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var insertCauHoiQuery = "INSERT INTO CauHoitb (BaiIdBai, NoiDungCH, ChuongIdChuong, KieuCauHoiIdKCH, MucDoCauHoiIdMDCH) VALUES (@BaiIdBai, @NoiDungCH, @ChuongIdChuong, @KieuCauHoiIdKCH, @MucDoCauHoiIdMDCH)";
                using (var insertCommand = new SQLiteCommand(insertCauHoiQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@BaiIdBai", 1);
                    insertCommand.Parameters.AddWithValue("@NoiDungCH", cauHoi);
                    insertCommand.Parameters.AddWithValue("@ChuongIdChuong", 1);
                    insertCommand.Parameters.AddWithValue("@KieuCauHoiIdKCH", 1);
                    insertCommand.Parameters.AddWithValue("@MucDoCauHoiIdMDCH", 1);
                    insertCommand.ExecuteNonQuery();
                }
            }
        }


        public void GenerateExamRand(int numberOfQuestions,int made)
        {
            Random random = new Random();
            var cauHoiList = new List<(long CauHoiId, string NoiDungCH)>();
            for (int i = 1; i <= numberOfQuestions; i++)
            {
                int a = random.Next(1, 101); 
                int b = random.Next(1, 101); 
                var cauHoi = TaoPhepTinhNgauNhien(a, b);
                LuuPhepTinhVaoCSDL(cauHoi, made);
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    long lastInsertedId = connection.LastInsertRowId;
                    string selectQuery = "SELECT CauHoiId, NoiDungCH FROM CauHoitb WHERE CauHoiId = @CauHoiId";
                    using (var command = new SQLiteCommand(selectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@CauHoiId", lastInsertedId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {                             
                                cauHoiList.Add((reader.GetInt32(0),reader.GetString(1)));
                            }
                        }
                    }
                }
            }
        }
    }
}
