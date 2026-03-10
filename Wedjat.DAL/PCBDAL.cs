using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wedjat.Model;
using Wedjat.Model.Entity;

namespace Wedjat.DAL
{
    public class PCBDAL : BaseDAL<PCB>
    {
        public PCBDAL() : base(AppDbContext.Sqlite)
        {

        }
    }
}
