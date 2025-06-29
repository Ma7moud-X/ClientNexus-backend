﻿using ClientNexus.Domain.Enums;
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
       
        //public string State { get; set; }
       
        //public string City { get; set; }
        public List<AddressDTO> Addresses { get; set; } = new();


        public List<string> SpecializationName { get; set; }
        
        public int Office_consultation_price { get; set; }
      
        public int Telephone_consultation_price { get; set; } 

        public string main_Specialization { get; set; }
        public Gender Gender { get; set; }
        public DateOnly BirthDate { get; set; }
        public string phonenumber { get; set; }
        public bool IsApproved { get; set; }
        public bool IsFeatured { get; set; }
        public SubscriptionStatus SubscriptionStatus { get; set; }
        public SubscriptionType SubType { get; set; }
        public DateTime? SubscriptionExpiryDate { get; set; }


    }
}
