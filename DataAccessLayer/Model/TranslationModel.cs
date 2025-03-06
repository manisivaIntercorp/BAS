using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public class TranslationModel
    {
        //public int? Id { get; set; }
        public string? Key { get; set; }
        public string? LanguageCode { get; set; }
        public string? Value { get; set; }
        public string? Scope { get; set; }
        public string? NeutralValue { get; set; }
    }

    public class Translation
    {
        public string? ResourceName { get; set; }
        public string? TranslationKey { get; set; }
        public string? LanguageCode { get; set; }
        public string? TranslationValue { get; set; }
    }
}
