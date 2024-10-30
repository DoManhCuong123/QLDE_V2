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

namespace QLDE_V2
{
    /// <summary>
    /// Interaction logic for DisplayLamDe.xaml
    /// </summary>
    public partial class DisplayLamDe : Window
    {
        public DisplayLamDe()
        {
            InitializeComponent();
            Giadienchondis();
        }

        public void Giadienchondis()
        {
            var chonDisplay = new GiaoDienChon();
            chonDisplay.BatDauThi += BatDauThi;
            mainContent.Content = chonDisplay;
        }

        private void BatDauThi(int maDe, int thoiGian)
        {
            var examControl = new GiaoDienLam();
            mainContent.Content = examControl;
            examControl.BatDauThi(maDe, thoiGian);
        }
    }
}
