using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLDE_V2.Class
{
    public class KieuCauHoi
    {
        [Key,AutoIncrement]
        public int KieuCauHoiId {  get; set; }
        public string? NoiDungKC {  get; set; }
        public List<CauHoi> CauHoiS { get; set; } = new List<CauHoi>();
    }
}
