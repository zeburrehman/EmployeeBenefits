using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using EmployeeBenefits.Domain;
using NHibernate;
using NUnit.Framework;

namespace EmployeeBenefits.UnitTests
{
	[TestFixture]
	public class EmployeeMappingsTests
	{
		private InMemoryDatabaseForXmlMappings database;
		private ISession session;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			var dir = Path.GetDirectoryName(typeof(EmployeeBenefits.Persistence.test).Assembly.Location);
			if (dir != null)
			{
				Environment.CurrentDirectory = dir;
				Directory.SetCurrentDirectory(dir);
			}
			else
				throw new Exception("Path.GetDirectoryName(typeof(TestingWithReferencedFiles).Assembly.Location) returned null");
		}

		[SetUp]
		public void Setup()
		{
			database = new InMemoryDatabaseForXmlMappings();
			session = database.Session;
		}
		[Test]
		public void MapsPrimitiveProperties()
		{
			object id = 0;
			using (var transaction = session.BeginTransaction())
			{
				id = session.Save(new Employee
				{
					EmployeeNumber = "5987123",
					FirstName = "Hillary",
					LastName = "Gamble",
					EmailAddress = "hillary.gamble@corporate.com",
					DateOfBirth = new DateTime(1980, 4, 23),
					JoiningDate = new DateTime(2010, 7, 12),
					IsAdmin = true,
					Password = "Password"
				});
				transaction.Commit();
			}
			session.Clear();
			using (var transaction = session.BeginTransaction())
			{
				var employee = session.Get<Employee>(id);
				Assert.That(employee.EmployeeNumber,
					Is.EqualTo("5987123"));
				Assert.That(employee.FirstName, Is.EqualTo("Hillary"));
				Assert.That(employee.LastName, Is.EqualTo("Gamble"));
				Assert.That(employee.EmailAddress,
					Is.EqualTo("hillary.gamble@corporate.com"));
				Assert.That(employee.DateOfBirth.Year, Is.EqualTo(1980));
				Assert.That(employee.DateOfBirth.Month, Is.EqualTo(4));
				Assert.That(employee.DateOfBirth.Day, Is.EqualTo(23));
				Assert.That(employee.JoiningDate.Year,
					Is.EqualTo(2010));
				Assert.That(employee.JoiningDate.Month, Is.EqualTo(7));
				Assert.That(employee.JoiningDate.Day, Is.EqualTo(12));
				Assert.That(employee.IsAdmin, Is.True);
				Assert.That(employee.Password, Is.EqualTo("Password"));
				transaction.Commit();
			}
		}

		[Test]
		public void MapBenefits()
		{
			object id = 0;
			using (var transaction = session.BeginTransaction())
			{
				id = session.Save(new Employee
				{
					EmployeeNumber = "134",
					Benefits = new HashSet<Benefit>
					{
						new SkillsEnhancementAllowance{ Entitlement = 1000, RemainingEntitlement = 250 },
						new SeasonTicketLoan{ Amount = 1416, MonthlyInstallment = 18, StartDate = new DateTime(2015, 4, 25), EndDate = new DateTime(2015, 3, 25)},
						new Leave{ AvailableEntitlement = 30, RemainingEntitlement = 15, Type = LeaveType.Paid }
					}
				});
				transaction.Commit();
			}
			session.Clear();

			using (var transaction = session.BeginTransaction())
			{
				var employee = session.Get<Employee>(id);
				Assert.That(employee.Benefits.Count, Is.EqualTo(3));

				var seasonTicketLoan = employee.Benefits.OfType<SeasonTicketLoan>().FirstOrDefault();
				Assert.That(seasonTicketLoan, Is.Not.Null);
				if (seasonTicketLoan != null)
					Assert.That(seasonTicketLoan.Employee.EmployeeNumber, Is.EqualTo("134"));
				var skillsEnhansmentAllowance = employee.Benefits.OfType<SkillsEnhancementAllowance>().FirstOrDefault();
				Assert.That(skillsEnhansmentAllowance, Is.Not.Null);
				if (skillsEnhansmentAllowance != null)
					Assert.That(skillsEnhansmentAllowance.Employee.EmployeeNumber, Is.EqualTo("134"));
				var leave = employee.Benefits.OfType<Leave>().FirstOrDefault();
				Assert.That(leave, Is.Not.Null);
				if (leave != null)
					Assert.That(leave.Employee.EmployeeNumber, Is.EqualTo("134"));

				transaction.Commit();
			}
		}

		[Test]
		public void MapResidentialAddress()
		{
			object id = 0;
			using (var transaction = session.BeginTransaction())
			{
				var residentailAddress = new Address
				{
					AddressLine1 = "Line 1",
					AddressLine2 = "Line 2",
					City = "Lahore",
					Country = "Pakistan",
					PostCode = "4200"
				};

				var employee = new Employee
				{
					EmployeeNumber = "123",
					ResidentialAddress = residentailAddress
				};
				residentailAddress.Employee = employee;
				id = session.Save(employee);
				transaction.Commit();
			}

			using (var transaction = session.BeginTransaction())
			{
				var employee = session.Get<Employee>(id);
				Assert.That(employee, Is.Not.Null);
				Assert.That(employee.ResidentialAddress.AddressLine1, Is.EqualTo("Line 1"));
				Assert.That(employee.ResidentialAddress.AddressLine2, Is.EqualTo("Line 2"));
				Assert.That(employee.ResidentialAddress.City, Is.EqualTo("Lahore"));
				Assert.That(employee.ResidentialAddress.Country, Is.EqualTo("Pakistan"));
				Assert.That(employee.ResidentialAddress.PostCode, Is.EqualTo("4200"));
				Assert.That(employee.ResidentialAddress.Employee.EmployeeNumber, Is.EqualTo("123"));
				transaction.Commit();
			}
		}

		[Test]
		public void MapCommunities()
		{
			object id = 0;
			using (var transaction = session.BeginTransaction())
			{
				var employee = new Employee
				{
					EmployeeNumber = "125",
					Communities = new List<Community>
					{
						new Community{ Name = "Community One"},
						new Community{ Name = "Community Two"}
					}
				};
				id = session.Save(employee);
				transaction.Commit();
			}

			using (var transaction = session.BeginTransaction())
			{
				var employee = session.Get<Employee>(id);
				Assert.That(employee, Is.Not.Null);
				Assert.That(employee.Communities.Count, Is.EqualTo(2));
				Assert.That(employee.Communities.First().Members.First().EmployeeNumber, Is.EqualTo("125"));
			}
		}

		[Test]
		public void MapsSkillsEnhancementAllowance()
		{
			object id = 0;
			using (var transaction = session.BeginTransaction())
			{
				id = session.Save(new SkillsEnhancementAllowance
				{
					Name = "Skill Enhacement Allowance",
					Description = "Allowance for employees so that their skill enhancement trainings are paid for",
					Entitlement = 1000,
					RemainingEntitlement = 250
				});
				transaction.Commit();
			}
			session.Clear();
			using (var transaction = session.BeginTransaction())
			{
				var benefit = session.Get<Benefit>(id);
				var skillsEnhancementAllowance = benefit as SkillsEnhancementAllowance;
				Assert.That(skillsEnhancementAllowance, Is.Not.Null);
				if (skillsEnhancementAllowance != null)
				{
					Assert.That(skillsEnhancementAllowance.Name, Is.EqualTo("Skill Enhacement Allowance"));
					Assert.That(skillsEnhancementAllowance.Description, Is.EqualTo("Allowance for employees so that their skill enhancement trainings are paid for"));
					Assert.That(skillsEnhancementAllowance.Entitlement, Is.EqualTo(1000));
					Assert.That(skillsEnhancementAllowance.RemainingEntitlement, Is.EqualTo(250));
				}
				transaction.Commit();
			}
		}
	}
}