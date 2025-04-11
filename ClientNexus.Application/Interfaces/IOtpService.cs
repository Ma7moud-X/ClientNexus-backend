﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.Interfaces
{
  
    public interface IOtpService
    {
        Task<string> GenerateOtpAsync(string email);
        Task<bool> ValidateOtpAsync(string email, string otp);
        Task SendOtpAsync(string email, string otp);
    }

}
