using System;
namespace NHibernateOrm
{
	static class ExceptionExtensions
	{
		public static Exception InnerMostException (this Exception ex, bool print = true)
		{
			while (ex.InnerException != null) 
			{
				if (print) {
					Console.WriteLine (ex.Message);
				}
				ex = ex.InnerException;
			}
				
			return ex;
		}

	}
}

