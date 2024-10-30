using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLDE_V2.Class
{
    public class CauHoi
    {
        [Key, AutoIncrement]
        public int CauHoiId { get; set; }
        public string? NoiDungCH { get; set; }

        public int? BaiIdBai { get; set; } // Foreign key property
        public int? ChuongIdChuong { get; set; } // Foreign key property
        public int? KieuCauHoiIdKCH {  get; set; } // Foreign key property
        public int? MucDoCauHoiIdMDCH {  get; set; } // Foreign key property

        public  Chuong? Chuong { get; set; }
        public KieuCauHoi? KieuCauHoi { get; set; }
        public MucDoCauHoi? MucDoCauHoi { get; set; }
        public  Bai? Bai { get; set; }
        public List<CauHoiDe> CauHoiDeS { get; set; } = new List<CauHoiDe>();
    }
}
