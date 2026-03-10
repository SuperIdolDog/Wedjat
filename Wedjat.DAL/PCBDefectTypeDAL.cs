using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wedjat.Model;
using Wedjat.Model.Entity;

namespace Wedjat.DAL
{
    public class PCBDefectTypeDAL : BaseDAL<PCBDefectType>
    {
        public PCBDefectTypeDAL() : base(AppDbContext.Sqlite)
        {

        }
    }
}
