using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLDE_V2.Class
{
    public class CauHoiDe
    {
        [Key, AutoIncrement]
        public int IdDeCauHoi {  get; set; }

        public int DeIdDe {  get; set; }
        public int CauHoiIdCauHoi { get; set; }

        public CauHoi CauHoi { get; set; }
        public KhoDe KhoDe { get; set; }

    }
}
