using System.Linq;
using EmployeeBenefits.Domain;
using NUnit.Framework;

namespace EmployeeBenefits.UnitTests
{
	[TestFixture]
	public class EmployeeTests
	{
		public void EmployeeEntitledToPaidLeaves()
		{
			//Arrange
			var employee = new Employee();

			//Act
			employee.Leaves.Add(new Leave{ Type = LeaveType.Paid, AvailableEntitlement = 15 });

			//Assert
			var paidLeave = employee.Leaves.FirstOrDefault(f => f.Type == LeaveType.Paid);
			Assert.That(paidLeave, Is.Not.Null);
			Assert.That(paidLeave.AvailableEntitlement, Is.EqualTo(15));
		}
	}
}