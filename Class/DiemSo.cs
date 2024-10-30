using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLDE_V2.Class
{
    public class DiemSo
    {
        public int MaDiem {  get; set; }
        public double Diem {  get; set; }

        public virtual KhoDe KhoDe { get; set; }
    }
}
