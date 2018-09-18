namespace EmployeeBenefits.Domain
{
	public class Leave
	{
		public virtual LeaveType Type { get; set; }
		public virtual int AvailableEntitlement { get; set; }
		public virtual int RemainingEntitlement { get; set; }
	}
}