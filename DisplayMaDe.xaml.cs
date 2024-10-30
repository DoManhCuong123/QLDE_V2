using DocumentFormat.OpenXml.Math;
using QLDE_V2.Class;
using System;
using System.Collections.Generic;
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
using Xceed.Words.NET;
using Microsoft.Win32;

namespace QLDE_V2
{
    /// <summary>
    /// Interaction logic for DisplayMaDe.xaml
    /// </summary>
    public partial class DisplayMaDe : Window
    {
        public long _maDe;
        public List<(long CauHoiId, string NoiDungCH)> _cauHoiList;
        public DisplayMaDe(long maDe, List<(long CauHoiId, string NoiDungCH)> cauHoiList)
        {

            InitializeComponent();
            _maDe = maDe;
            _cauHoiList = cauHoiList;
            MaDeTextBlock.Text = maDe.ToString();
            CauHoiListBox.ItemsSource = cauHoiList.Select(ch => new { NoiDungCH = ch.NoiDungCH }).ToList();
        }

        private void ExportToWord(string filePath)
        {
            var doc = DocX.Create(filePath);

            // Thêm tiêu đề
            var title = doc.InsertParagraph("Đề thi").FontSize(20).Bold();

            // Thêm mã đề
            doc.InsertParagraph($"Mã Đề: {_maDe}").FontSize(16).Bold();

            // Thêm danh sách câu hỏi
            foreach (var cauHoi in _cauHoiList)
            {
                doc.InsertParagraph(cauHoi.NoiDungCH).FontSize(14);
            }
            // Lưu file
            doc.Save();
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Word Document (*.docx)|*.docx", // Lọc chỉ cho phép lưu dưới định dạng .docx
                Title = "Lưu đề thi",
                FileName = "DeThi.docx" // Tên mặc định
            };

            // Nếu người dùng chọn đường dẫn và nhấn OK
            if (saveFileDialog.ShowDialog() == true)
            {
                // Gọi hàm xuất ra file Word với đường dẫn đã chọn
                string filePath = saveFileDialog.FileName;
                ExportToWord(filePath);
                MessageBox.Show($"Đã xuất đề thi ra file {filePath}");
            }
        }
    }
}
