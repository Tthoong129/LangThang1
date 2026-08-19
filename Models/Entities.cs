using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniMap.Models
{
    public class User
    {
        [Key]
        public long Id { get; set; }
        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;
        [Required, MaxLength(150)]
        public string Email { get; set; } = string.Empty;
        [Required, MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;
        [MaxLength(20)]
        public string? Phone { get; set; }
        [MaxLength(500)]
        public string? AvatarUrl { get; set; }
        [MaxLength(100)]
        public string? GoogleId { get; set; }
        [Required, MaxLength(30)]
        public string Role { get; set; } = "user"; // 'user','category_admin','system_admin'
        [Required, MaxLength(20)]
        public string Status { get; set; } = "active"; // 'active','locked'
        [MaxLength(500)]
        public string? Bio { get; set; }
        [MaxLength(500)]
        public string? CoverUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<AdminCategoryAssignment> CategoryAssignments { get; set; } = new List<AdminCategoryAssignment>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<VisitLog> VisitLogs { get; set; } = new List<VisitLog>();
        public ICollection<AccessHistory> AccessHistories { get; set; } = new List<AccessHistory>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }

    public class Region
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        [Required, MaxLength(20)]
        public string Status { get; set; } = "active"; // 'active','hidden'

        public ICollection<Province> Provinces { get; set; } = new List<Province>();
    }

    public class Province
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public int RegionId { get; set; }
        public Region? Region { get; set; }
        [Required, MaxLength(20)]
        public string Status { get; set; } = "active";

        public ICollection<Place> Places { get; set; } = new List<Place>();
        public ICollection<FoodProvince> FoodProvinces { get; set; } = new List<FoodProvince>();
    }

    public class PlaceType
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty; // 'Ăn uống', 'Du lịch', 'Lưu trú', 'Vui chơi'
        [Required, MaxLength(20)]
        public string Status { get; set; } = "active";

        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }

    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public int PlaceTypeId { get; set; }
        public PlaceType? PlaceType { get; set; }
        [Required, MaxLength(20)]
        public string Status { get; set; } = "active";

        public ICollection<Place> Places { get; set; } = new List<Place>();
        public ICollection<AdminCategoryAssignment> AdminAssignments { get; set; } = new List<AdminCategoryAssignment>();
    }

    public class AdminCategoryAssignment
    {
        [Key]
        public long Id { get; set; }
        public long UserId { get; set; }
        public User? User { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }

    public class Place
    {
        [Key]
        public long Id { get; set; }
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required, MaxLength(255)]
        public string Address { get; set; } = string.Empty;
        [MaxLength(20)]
        public string? Phone { get; set; }
        [MaxLength(500)]
        public string? Website { get; set; }
        [Column(TypeName = "decimal(12,0)")]
        public decimal? MinPrice { get; set; }
        [Column(TypeName = "decimal(12,0)")]
        public decimal? MaxPrice { get; set; }
        [MaxLength(255)]
        public string? OpeningHours { get; set; }
        [Column(TypeName = "decimal(10,7)")]
        public decimal? Latitude { get; set; }
        [Column(TypeName = "decimal(10,7)")]
        public decimal? Longitude { get; set; }

        public int ProvinceId { get; set; }
        public Province? Province { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Required, MaxLength(30)]
        public string Source { get; set; } = "admin_created"; // 'user_proposed','admin_created'
        public long? ProposedBy { get; set; }
        public User? Proposer { get; set; }

        [Required, MaxLength(20)]
        public string Status { get; set; } = "approved"; // 'pending','approved','rejected','hidden'
        [MaxLength(500)]
        public string? RejectReason { get; set; }
        public long? ApprovedBy { get; set; }
        public User? Approver { get; set; }

        public DateTime? ProposedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }

        [Column(TypeName = "decimal(2,1)")]
        public decimal AvgRating { get; set; } = 0;
        public int ReviewCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<PlaceMedia> Media { get; set; } = new List<PlaceMedia>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<FoodPlace> FoodPlaces { get; set; } = new List<FoodPlace>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<VisitLog> VisitLogs { get; set; } = new List<VisitLog>();
        public ICollection<AccessHistory> AccessHistories { get; set; } = new List<AccessHistory>();
    }

    public class PlaceMedia
    {
        [Key]
        public long Id { get; set; }
        public long PlaceId { get; set; }
        public Place? Place { get; set; }
        public long? UploadedBy { get; set; }
        public User? Uploader { get; set; }
        [Required, MaxLength(10)]
        public string MediaType { get; set; } = "image"; // 'image','video'
        [Required, MaxLength(500)]
        public string Url { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class PlaceEditProposal
    {
        [Key]
        public long Id { get; set; }
        public long PlaceId { get; set; }
        public Place? Place { get; set; }
        public long ProposedBy { get; set; }
        public User? Proposer { get; set; }
        [Required]
        public string ProposedData { get; set; } = string.Empty; // JSON string
        [Required, MaxLength(20)]
        public string Status { get; set; } = "pending"; // 'pending','approved','rejected'
        [MaxLength(500)]
        public string? RejectReason { get; set; }
        public long? ReviewedBy { get; set; }
        public User? Reviewer { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }
    }

    public class Food
    {
        [Key]
        public long Id { get; set; }
        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        [MaxLength(500)]
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<FoodProvince> FoodProvinces { get; set; } = new List<FoodProvince>();
        public ICollection<FoodPlace> FoodPlaces { get; set; } = new List<FoodPlace>();
    }

    public class FoodProvince
    {
        public long FoodId { get; set; }
        public Food? Food { get; set; }
        public int ProvinceId { get; set; }
        public Province? Province { get; set; }
    }

    public class FoodPlace
    {
        public long FoodId { get; set; }
        public Food? Food { get; set; }
        public long PlaceId { get; set; }
        public Place? Place { get; set; }
    }

    public class Review
    {
        [Key]
        public long Id { get; set; }
        public long PlaceId { get; set; }
        public Place? Place { get; set; }
        public long UserId { get; set; }
        public User? User { get; set; }
        public byte Rating { get; set; } // 1-5
        public string? Content { get; set; }
        [MaxLength(500)]
        public string? VideoUrl { get; set; }
        public DateTime? ExperienceDate { get; set; }
        [Required, MaxLength(20)]
        public string Status { get; set; } = "visible"; // 'visible','hidden'
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ReviewMedia> Media { get; set; } = new List<ReviewMedia>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }

    public class ReviewMedia
    {
        [Key]
        public long Id { get; set; }
        public long ReviewId { get; set; }
        public Review? Review { get; set; }
        [Required, MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Comment
    {
        [Key]
        public long Id { get; set; }
        public long ReviewId { get; set; }
        public Review? Review { get; set; }
        public long UserId { get; set; }
        public User? User { get; set; }
        [Required]
        public string Content { get; set; } = string.Empty;
        [Required, MaxLength(20)]
        public string Status { get; set; } = "visible"; // 'visible','hidden'
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Favorite
    {
        public long UserId { get; set; }
        public User? User { get; set; }
        public long PlaceId { get; set; }
        public Place? Place { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class VisitLog
    {
        [Key]
        public long Id { get; set; }
        public long UserId { get; set; }
        public User? User { get; set; }
        public long PlaceId { get; set; }
        public Place? Place { get; set; }
        public DateOnly VisitedDate { get; set; }
        [Required, MaxLength(20)]
        public string Privacy { get; set; } = "public"; // 'public','private'
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class AccessHistory
    {
        [Key]
        public long Id { get; set; }
        public long UserId { get; set; }
        public User? User { get; set; }
        public long PlaceId { get; set; }
        public Place? Place { get; set; }
        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
    }

    public class ReportReason
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(150)]
        public string Content { get; set; } = string.Empty;
        [Required, MaxLength(20)]
        public string Status { get; set; } = "active";
    }

    public class Report
    {
        [Key]
        public long Id { get; set; }
        public long ReporterId { get; set; }
        public User? Reporter { get; set; }
        [Required, MaxLength(20)]
        public string TargetType { get; set; } = string.Empty; // 'place','review','comment'
        public long TargetId { get; set; }
        public int ReasonId { get; set; }
        public ReportReason? Reason { get; set; }
        public string? Description { get; set; }
        [Required, MaxLength(20)]
        public string Status { get; set; } = "pending"; // 'pending','resolved'
        [MaxLength(30)]
        public string? Result { get; set; } // 'violation_confirmed','dismissed'
        public long? HandledBy { get; set; }
        public User? Handler { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public DateTime? HandledAt { get; set; }
    }

    public class Appeal
    {
        [Key]
        public long Id { get; set; }
        public long UserId { get; set; }
        public User? User { get; set; }
        [Required, MaxLength(30)]
        public string TargetType { get; set; } = string.Empty; // 'place','review','comment','place_edit_proposal'
        public long TargetId { get; set; }
        [Required]
        public string Reason { get; set; } = string.Empty;
        [Required, MaxLength(40)]
        public string Status { get; set; } = "pending"; // 'pending','handled_by_category_admin','escalated_to_system_admin','resolved'
        
        public long? CategoryAdminId { get; set; }
        public User? CategoryAdmin { get; set; }
        public string? CategoryAdminResult { get; set; }
        public DateTime? CategoryAdminAt { get; set; }

        public long? SystemAdminId { get; set; }
        public User? SystemAdmin { get; set; }
        public string? FinalResult { get; set; }
        public DateTime? SystemAdminAt { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }

    public class Notification
    {
        [Key]
        public long Id { get; set; }
        public long UserId { get; set; }
        public User? User { get; set; }
        [Required, MaxLength(500)]
        public string Content { get; set; } = string.Empty;
        [Required, MaxLength(40)]
        public string Type { get; set; } = string.Empty; // 'place_approved','place_rejected','place_edit_handled','review_report_handled','comment_created','appeal_resolved'
        [MaxLength(30)]
        public string? TargetType { get; set; }
        public long? TargetId { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class SystemConfig
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string ConfigKey { get; set; } = string.Empty;
        [Required, MaxLength(255)]
        public string ConfigValue { get; set; } = string.Empty;
        [MaxLength(255)]
        public string? Description { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public long? UpdatedBy { get; set; }
        public User? Updater { get; set; }
    }

    public class AuditLog
    {
        [Key]
        public long Id { get; set; }
        public long UserId { get; set; }
        public User? User { get; set; }
        [Required, MaxLength(100)]
        public string Action { get; set; } = string.Empty;
        [MaxLength(50)]
        public string? TargetType { get; set; }
        public long? TargetId { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
