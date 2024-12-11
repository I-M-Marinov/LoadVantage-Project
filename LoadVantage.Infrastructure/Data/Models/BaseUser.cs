using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static LoadVantage.Common.ValidationConstants.UserValidations;

namespace LoadVantage.Infrastructure.Data.Models
{
	public abstract class BaseUser : IdentityUser<Guid>
	{
        public BaseUser(string companyName)
        {
            CompanyName = companyName; 
        }

        public BaseUser()
        {
        }

        [Required]
        [StringLength(PositionMaxLength)]
        [Comment("Position of the user")]
        public virtual string? Position { get; set; }

        [Required]
		[StringLength(FirstNameMaxLength)]
        [Comment("First name of the user")]
		public string FirstName { get; set; } 

		[Required]
		[StringLength(LastNameMaxLength)]
		[Comment("Last name of the user")]
		public string LastName { get; set; } 

        [StringLength(CompanyNameMaxLength)]
        [Comment("The name of the company employing the user")]
		public string? CompanyName { get; set; }

        [Required]
        [StringLength(UserNameMaxLength)]
        [Comment("Username of the user")]
		public override string UserName { get; set; }

        [Required]
        [StringLength(EmailMaxLength)]
        [Comment("Email of the user")]
		public override string Email { get; set; }

		[Comment("HashedPassword for the user")]
        public override string? PasswordHash { get; set; }

        [Phone]
        [StringLength(UserPhoneNumberMaxLength)]
        [Comment("Phone number for the user")]
		public override string? PhoneNumber { get; set; }


		[ForeignKey(nameof(UserImage))]
		public Guid? UserImageId { get; set; }
		[Comment("User image for the user")]
		public virtual UserImage? UserImage { get; set; }

		public string FullName => $"{FirstName} {LastName}"; // FullName combines FirstName and LastName

		public Guid RoleId { get; set; }
		[Comment("Role of the user")]
		public virtual Role Role { get; set; }

		public abstract string GetRoleName(); // to be implemented by child classes
		public virtual ICollection<IdentityUserRole<Guid>> UserRoles { get; set; } = new HashSet<IdentityUserRole<Guid>>();

		public ICollection<ChatMessage> SentMessages { get; set; }
		public ICollection<ChatMessage> ReceivedMessages { get; set; }
		[Comment("Signifies if the user's account is activated or deactivated")]
		public bool IsActive { get; set; } = true;

	}
}