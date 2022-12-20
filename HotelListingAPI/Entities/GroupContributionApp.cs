using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListingAPI.Entities
{
    public class GroupContributionApp
    {
        
    }

      public class Userr // change User class spelling because of conflict 
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; }

        public UserType UserType { get; set; }
    }

    public class Group
    {
        [Key]
        public int GroupId { get; set; }

        [Required]
        [StringLength(50)]
        public string GroupName { get; set; }

        [Required]
        public int GroupAdminId { get; set; }

        public User GroupAdmin { get; set; }

        public ICollection<User> Members { get; set; }

        public string ContributionGuidelines { get; set; }
    }

    public class Contribution
    {
        [Key]
        public int ContributionId { get; set; }

        [Required]
        public int UserId { get; set; }

        public User User { get; set; }

        [Required]
        public int GroupId { get; set; }

        public Group Group { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }

   public class Compliance
{
    [Key]
    public int ComplianceId { get; set; }

    [Required]
    public int UserId { get; set; }

    public User User { get; set; }

    [Required]
    public int GroupId { get; set; }

    public Group Group { get; set; }

    public ComplianceStatus Status { get; set; }

    public string Notes { get; set; }

    public decimal RequiredContributionAmount { get; set; }

    public decimal TotalContributionAmount { get; set; }

    public decimal CompliancePercentage
    {
        get
        {
            return TotalContributionAmount / RequiredContributionAmount;
        }
    }
}

    public enum UserType
    {
        Admin,
        Regular,
        Guest
    }

    public enum ComplianceStatus
    {
        Compliant,
        NonCompliant
    }
}