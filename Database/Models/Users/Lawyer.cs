
namespace Database.Models.Users
{
    public class Lawyer : ServiceProvider
    {
         public int? YearsOfExperience { get; set; }
        
        public ICollection<LawyerLicence>? Licences { get; set; }
        public ICollection<LawyerSpecialization>? Specializations { get; set; }
    }
}