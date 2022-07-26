using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstResponseApp.Models
{
    public class EmailTemplates
    {
        public long Id { get; set; }
        public string EmailTitle { get; set; }
        public string EmailContent { get; set; }
    }
}
