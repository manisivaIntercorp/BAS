namespace DataAccessLayer.Model
{
    public class NationalityModel
    {
        public int Id { get; set; }
        public string? Nationality { get; set; }
        public string? NationalityCode { get; set; }
        public string? Active { get; set; }
        public int? CreatedBy { get; set; }
        
        public string? CreatedName { get; set; }
        
        public string? ModifiedName { get; set; }
       
        public DateTime? CreatedDateTime { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
       
        public DateTime? LastActiveDate { get; set; }
    }
    
}