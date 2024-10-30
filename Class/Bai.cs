using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLDE_V2.Class
{
    public class Bai
    {
        [Key, AutoIncrement]
        public int IdBai { get; set; }
        public string? BaiName { get; set; }

        public int ChuongIdChuong { get; set; } // Foreign key property
        public  Chuong Chuong { get; set; }

        public List<CauHoi> CauHoiS { get; set; } = new List<CauHoi>();
    }
}
