using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
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
        public virtual string? Position { get; set; }

        [Required]
		[StringLength(FirstNameMaxLength)]
		public string FirstName { get; set; } 

		[Required]
		[StringLength(LastNameMaxLength)]
		public string LastName { get; set; } 

        [StringLength(CompanyNameMaxLength)]
        public string? CompanyName { get; set; }

        [Required]
        [StringLength(UserNameMaxLength)]
        public override string UserName { get; set; }

        [Required]
        [StringLength(EmailMaxLength)]
        public override string Email { get; set; }

        [Required]
        public override string PasswordHash { get; set; }

        [Phone]
        [StringLength(UserPhoneNumberMaxLength)]
        public override string? PhoneNumber { get; set; }


		[ForeignKey(nameof(UserImage))]
		public Guid? UserImageId { get; set; } 
		public virtual UserImage? UserImage { get; set; }

		public string FullName => $"{FirstName} {LastName}"; // FullName combines FirstName and LastName

		public Guid RoleId { get; set; }
		public virtual Role Role { get; set; }

		public abstract string GetRoleName(); // to be implemented by child classes
		public virtual ICollection<IdentityUserRole<Guid>> UserRoles { get; set; } = new HashSet<IdentityUserRole<Guid>>();

		public ICollection<ChatMessage> SentMessages { get; set; }
		public ICollection<ChatMessage> ReceivedMessages { get; set; }
	}
}