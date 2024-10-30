using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLDE_V2.Class
{
    public class KhoDe
    {
        [Key]
        public int MaDe {  get; set; }
        public string NoiDungDe { get; set; }
        public float? Diem {  get; set; }
        public List<CauHoiDe> CauHoiDeS { get; set; } = new List<CauHoiDe>();

    }
}
