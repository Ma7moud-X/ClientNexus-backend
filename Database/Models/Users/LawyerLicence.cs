
using Database.Models.Users;

namespace Database.Models
{
    public class LawyerLicence
    {
        public int Id { get; set; }
        public string LicenceNumber { get; set; }
        public string IssuingAuthority { get; set; }
        public DateOnly IssueDate { get; set; }
        public DateOnly ExpiryDate { get; set; }
        
        // Foreign key
        public int LawyerId { get; set; }
        public Lawyer Lawyer { get; set; }
    }
}