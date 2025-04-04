using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO_QLBS
{
    public class connect
    {
        public string ConnectionString { get; set; }

        public connect()
        {
            ConnectionString = Properties.Settings.Default.qlbsConnectionString;
        }
    }
}
