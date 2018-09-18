using System;
using System.Collections;
using System.Collections.Generic;

namespace EmployeeBenefits.Domain
{
	public class Employee: EntityBase
	{
		public virtual string EmployeeNumber { get; set; }
		public virtual string FirstName { get; set; }
		public virtual string LastName { get; set; }
		public virtual string EmailAddress { get; set; }
		public virtual string Password { get; set; }
		public virtual DateTime DateOfBirth { get; set; }
		public virtual DateTime JoiningDate { get; set; }
		public virtual bool IsAdmin { get; set; }
			   
		public virtual Address ResidentialAddress { get; set; }
		public virtual ICollection<Benefit> Benefits { get; set; }
		public virtual ICollection<Leave> Leaves { get; set; }
	}
}