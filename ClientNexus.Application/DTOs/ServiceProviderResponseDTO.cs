using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.DTOs
{
    public class ServiceProviderResponseDTO
    {
       
        public int? Id { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public float Rate { get; set; }
      
        public string Description { get; set; }
      
        public string MainImage { get; set; }
      
        public string ImageIDUrl { get; set; }
     
        public string ImageNationalIDUrl { get; set; }
      
        public int YearsOfExperience { get; set; }
       
        public string State { get; set; }
       
        public string City { get; set; }
  
        public List<string> SpecializationName { get; set; }
        
        public int Office_consultation_price { get; set; }
      
        public int Telephone_consultation_price { get; set; } 
    }
}
