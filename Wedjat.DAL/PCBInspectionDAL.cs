using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wedjat.Model;
using Wedjat.Model.Entity;

namespace Wedjat.DAL
{
    public class PCBInspectionDAL : BaseDAL<PCBInspection>
    {
        public PCBInspectionDAL() : base(AppDbContext.Sqlite)
        {

        }
    }
}
