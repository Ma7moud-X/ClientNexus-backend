

using Database.Models.Users;

namespace Database.Models
{
    public class LawyerSpecialization
    {
        public int Id { get; set; }
        public string Name { get; set; }
                
        // Foreign key
        public int LawyerId { get; set; }
        public Lawyer Lawyer { get; set; }
    }
}