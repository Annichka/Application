using System;
using System.Collections.Generic;

namespace SampleApp.Core.Models
{
	public class GenericResponse<T>
	{
		public GenericResponse()
		{
			Errors = new List<string>();
			Models = new List<T>();
			Succeded = true;
		}

		public bool Succeded { get; set; }

		public ICollection<T> Models { get; set; }

		public List<string> Errors { get; set; }

		public void AddError(string error)
		{
			if (!Errors.Contains(error))
			{
				Errors.Add(error);
			}

			Succeded = false;
		}

		public override string ToString()
		{
			string errors = string.Empty;

			if (Errors != null)
			{
				errors = string.Join(Environment.NewLine, Errors);
			}

			return errors;
		}
	}

}
