using ClientNexus.Application.Interfaces;
using System;
using System.Net.Mail;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using ClientNexus.Domain.Interfaces;

namespace ClientNexus.Application.Services
{
    public class OtpService : IOtpService
    {
        private readonly ICache _cache; // RedisCache
        private readonly SmtpClient _smtpClient;

        // Constructor injection of Redis cache and SMTP client
        public OtpService(ICache cache)
        {
            _cache = cache;
            _smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("client.nexus.2025@gmail.com", "Fady@123"),
                EnableSsl = true
            };
        }

        // Generate OTP and store it in Redis
        public async Task<string> GenerateOtpAsync(string email)
        {
            var otp = new Random().Next(100000, 999999).ToString(); // Generate OTP

            // Store OTP in Redis with an expiration time of 5 minutes
            var expiration = TimeSpan.FromMinutes(5);
            await _cache.SetStringAsync(email, otp, expiration); // Using RedisCache

            return otp;
        }

        // Send OTP via email
        public async Task SendOtpAsync(string email, string otp)
        {
            var mailMessage = new MailMessage("client.nexus.2025@gmail.com", email)
            {
                Subject = "Password Reset OTP",
                Body = $"Your OTP for password reset is {otp}."
            };

            await _smtpClient.SendMailAsync(mailMessage); // Send OTP via Gmail SMTP
        }

        // Validate OTP by comparing with Redis-stored value
        public async Task<bool> ValidateOtpAsync(string email, string otp)
        {
            var cachedOtp = await _cache.GetStringAsync(email); // Retrieve OTP from Redis
            return cachedOtp != null && cachedOtp == otp; // Validate OTP
        }
    }
}
