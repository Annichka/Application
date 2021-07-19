using System;

namespace SampleApp.Core.Entities.Base
{
	public interface IEntityBase
	{
		Guid Id { get; set; }
		bool IsNew();
	}
}
