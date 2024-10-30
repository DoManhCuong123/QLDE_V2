using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLDE_V2.Class
{
    public class MucDoCauHoi
    {
        [Key,AutoIncrement]
        public int MucDoCauHoiId {  get; set; }
        public string? NoiDungMD {  get; set; }
        public List<CauHoi> CauHoiS { get; set; } = new List<CauHoi>();

    }
}
