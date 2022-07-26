using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstResponseApp.Models
{
    public class SmtpSetting
    {
        public long Id { get; set; }
        public string DefaultFromAddress { get; set; }
        public string DefaultFromDisplayName { get; set; }
        public string SmtpHost { get; set; }
        public string SmtpPort { get; set; }
        public string SmtpPassword { get; set; }
        public string SmtpEnableSsl { get; set; }
        public string SmtpUserName { get; set; }
    }
}
