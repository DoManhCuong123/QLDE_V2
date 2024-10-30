using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;


namespace QLDE_V2.Class
{
    public class Chuong
    {
        [Key, AutoIncrement]
        public int IdChuong { get; set; }
        public string? ChuongName { get; set; }

        public List<Bai> BaiS { get; set; } = new List<Bai>();
        public List<CauHoi> CauHoiS { get; set; } = new List<CauHoi>();
    }
}
