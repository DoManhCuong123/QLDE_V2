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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace QLDE_V2
{
    /// <summary>
    /// Interaction logic for GiaoDienLam.xaml
    /// </summary>
    public partial class GiaoDienLam : UserControl
    {
        private DispatcherTimer examTimer;
        private int thoiGianConLai;

        public GiaoDienLam()
        {
            InitializeComponent();
        }

        public void BatDauThi(int maDe, int thoiGian)
        {
            // Load câu hỏi từ cơ sở dữ liệu theo mã đề (maDe)
            thoiGianConLai = thoiGian * 60; // thời gian tính bằng giây
            examTimer = new DispatcherTimer();
            examTimer.Interval = TimeSpan.FromSeconds(1);
            examTimer.Tick += DemNguocThoiGian;
            examTimer.Start();
        }
        private void DemNguocThoiGian(object sender, EventArgs e)
        {
            thoiGianConLai--;
            txtTime.Text = $"Thời gian còn lại: {thoiGianConLai / 60}:{thoiGianConLai % 60}";
            if (thoiGianConLai <= 0)
            {
                examTimer.Stop();
                MessageBox.Show("Hết thời gian làm bài!");
                // Kết thúc bài thi
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            examTimer.Stop();
            MessageBox.Show("Bài thi đã hoàn thành!");
            DisplayLamDe window = (DisplayLamDe)DisplayLamDe.GetWindow(this);
            window.Close();
        }
    }
}
