using System.Data;

namespace DataAccessLayer.Implementation
{

    public class EmailTemplate
    {
        

        public long RefID1
        {
            get;
            set;
        }
        
        public int RefID2
        {
            get;
            set;
        }
        
        public String Template
        {
            get;
            set;
        }
        
        public String EmployeeID
        {
            get;
            set;
        }

        
        public String Subject
        {
            get;
            set;
        }

        
        public String DisplayName
        {
            get;
            set;
        }

        
        public String UserName
        {
            get;
            set;
        }

        
        public String Password
        {
            get;
            set;
        }

        
        public String URL
        {
            get;
            set;
        }

        
        public String ExpiredDateTime
        {
            get;
            set;        
        }

        
        public String Active
        {
            get;
            set;
        }

        
        public String TemplateCode
        {
            get;
            set;
        }
        
        public Int64 UserID
        {
            get;
            set;
        }

        
        public Int64 ContactNo
        {
            get;set;
        }
        
        public string Month
        {
            get;
            set;
        }
        
        public string Year
        {
            get;
            set;
        }

        
        public string Status
        {
            get;
            set;
        }
    public string EmailID
        {
            get;
            set;
        }
    }
    public class GetEmailTemplate
    {
        public string? EmailID { get; set; }
        public string? Template { get; set; }
        public string? Subject { get; set; }
        public string? MobileContent { get; set; }
    }
}