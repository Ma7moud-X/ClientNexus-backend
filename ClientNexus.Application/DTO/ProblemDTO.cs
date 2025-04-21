using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ClientNexus.Domain.Enums;

namespace ClientNexus.Application.DTO
{

    public class ProblemDto
    {
        public int Id { get; set; }
        [Required]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 1000 characters")]
        public required string Description { get; set; }
        public string? AdminComment { get; set; }
        [EnumDataType(typeof(ProblemStatus))]
        public ProblemStatus Status { get; set; } = ProblemStatus.New;
        [EnumDataType(typeof(ReporterType))]
        [Required]        
        public ReporterType ReportedBy { get; set; }
        [Required]
        public int ClientId { get; set; }
        [Required]        
        public int ServiceProviderId { get; set; }
        public int? SolvingAdminId { get; set; }
        [Required]
        public int ServiceId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ClosedAt { get; set; }
    }
    public class CreateProblemDto
    {
        [Required]
        public int ClientId { get; set; }
        [Required]        
        public int ServiceProviderId { get; set; }
        [Required]
        public int ServiceId { get; set; }

        [EnumDataType(typeof(ReporterType))]
        [Required]        
        public ReporterType ReportedBy { get; set; }
        [Required]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 1000 characters")]
        public required string Description { get; set; }
    }
    public class UpdateProblemDto
    {
        [Required]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 1000 characters")]
        public required string Description { get; set; }
    }
    public class ProblemListItemDto
    {
        public int Id { get; set; }
        [Required]
        public required string Description { get; set; }
        public string? AdminComment { get; set; }
        [EnumDataType(typeof(ProblemStatus))]   
        public ProblemStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
    }
    public class ProblemAdminDto
    {
        public int Id { get; set; }
        [Required]
        public required string Description { get; set; }
        public string? AdminComment { get; set; }

        [EnumDataType(typeof(ProblemStatus))]
        public ProblemStatus Status { get; set; }
        [EnumDataType(typeof(ReporterType))]
        [Required]
        public ReporterType ReportedBy { get; set; }
        
        // Client info
        [Required]
        public int ClientId { get; set; }
        [Required]        
        public required string ClientName { get; set; }
        
        // Service Provider info
        [Required]        
        public int ServiceProviderId { get; set; }
        [Required]        
        public required string ServiceProviderName { get; set; }
        
        // Service info
        [Required]        
        public int ServiceId { get; set; }
        [Required]        
        public required string ServiceName { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
    }
    public class UpdateProblemStatusDto
    {
        [EnumDataType(typeof(ProblemStatus))]
        public ProblemStatus Status { get; set; }
    }
    public class AdminCommentDto
    {
        [Required]        
        public int SolvingAdminId { get; set; }
        [Required]
        [StringLength(1000, MinimumLength = 5, ErrorMessage = "AdminComment must be between 5 and 1000 characters")]
        public required string AdminComment { get; set; }
    }

}