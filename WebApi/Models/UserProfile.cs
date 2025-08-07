namespace WebApi.Models
{
	public class UserProfile
	{
		public string Id { get; set; } = string.Empty; // In Identity, Id is string
		public string Name { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
	}
}
