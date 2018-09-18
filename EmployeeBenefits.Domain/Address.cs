namespace EmployeeBenefits.Domain
{
	public class Address: EntityBase
	{
		public virtual string AddressLine1 { get; set; }
		public virtual string AddressLine2 { get; set; }
		public virtual string PostCode { get; set; }
		public virtual string City { get; set; }
		public virtual string Country { get; set; }
	}
}
