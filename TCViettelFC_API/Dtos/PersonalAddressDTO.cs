namespace TCViettelFC_API.Dtos
{
	public class PersonalAddressDTO
	{
		public int AddressId { get; set; }
		public string? CityName { get; set; }
		public string? City { get; set; }
		public string? DistrictName { get; set; }
		public string? District { get; set; }
		public string? WardName { get; set; }
		public string? Ward { get; set; }
		public string? DetailedAddress { get; set; }
		public int? Status { get; set; }
	}

	public class PersonalAddressCreateDto
	{
		public int? CustomerId { get; set; }
		public string? CityName { get; set; }
		public string? City { get; set; }
		public string? DistrictName { get; set; }
		public string? District { get; set; }
		public string? WardName { get; set; }
		public string? Ward { get; set; }
		public string? DetailedAddress { get; set; }
		public int? Status { get; set; }
	}
}
