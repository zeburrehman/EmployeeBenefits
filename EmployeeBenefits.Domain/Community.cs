using System.Collections.Generic;

namespace EmployeeBenefits.Domain
{
	public class Community: EntityBase
	{
		public virtual string Name { get; set; }
		public virtual string Description { get; set; }
		public virtual ICollection<Employee> Members { get; set; }
	}
}