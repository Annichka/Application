using SampleApp.Core.Interfaces;
using System;

namespace SampleApp.Core.Filter.Address
{
	public class AddressFilter : IEntityFilter
	{
		public string NameEquals { get; set; }
		public Guid? CreatedById { get; set; }
		public Guid? IdNotEquals {get;set;}
	}
}
