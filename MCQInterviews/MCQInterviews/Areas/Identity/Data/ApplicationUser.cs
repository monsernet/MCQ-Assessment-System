using MCQInterviews.Models.Domain;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthSystem.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string FirstName { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string LastName { get; set; }

    [PersonalData]
    public DateTime RegistrationDate { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(200)")]
    public string? ProfileImage { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(250)")]
    public string? Description { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string? Country { get; set; }

    public int IsActive { get; set; } = 1;

    public DateTime? LastLoginDate { get; set; }

    // Navigation property to MCQ Test Results
    public ICollection<MCQTestResult> TestResults { get; set; }
    // Navigation property to UserLogins
    public ICollection<UserLogin> UserLogins { get; set; }
}

