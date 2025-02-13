
namespace Database.Models.Users
{
    public class Lawyer : ServiceProvider
    {
         public int? YearsOfExperience { get; set; }
        
        public List<LawyerLicence> Licences { get; set; } = new();
        public List<LawyerSpecialization> Specializations { get; set; } = new();
    }
}