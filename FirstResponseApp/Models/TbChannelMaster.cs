using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FirstResponseApp.Models
{
    public class TbChannelMaster
    {

        [Key]

        public long Id { get; set; }
        public string ChannelName { get; set; }

        public bool isActive { get; set; }
    }
}
