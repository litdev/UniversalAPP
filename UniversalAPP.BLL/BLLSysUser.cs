using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DynamicLinq;
using System.Linq.Dynamic.Core;

namespace UniversalAPP.BLL
{
    public class BLLSysUser: BaseBLL
    {
        public BLLSysUser(EFCore.EFDBContext context)
        {
            this.db = context;
        }
        
    }
}
