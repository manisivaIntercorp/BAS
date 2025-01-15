using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public class NationalityModel
    {
        public int Id { get; set; }
        public string? Nationality { get; set; }
        public string? NationalityCode { get; set; }
        public string? Active { get; set; }
    }
}
