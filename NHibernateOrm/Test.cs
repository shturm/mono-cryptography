using NUnit.Framework;
using System;
using System.Linq;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using FluentNHibernate.Automapping;
using System.Reflection;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using NHibernateOrm.Entities;
using NHibernate;
using System.IO;

namespace NHibernateOrm
{
	[TestFixture ()]
	public class Test
	{
		public string connectionString { get; set; } = "Server=localhost;Database=northwind;Uid=uniuser;Pwd=unipass;";
		public string DbFileName { get; set; } = "northwind.db";

		[Test ()]
		public void ConnectToMySQL_Automapped ()
		{

			var dbConf = MySQLConfiguration.Standard.ConnectionString (connectionString).ShowSql ();
			Action<MappingConfiguration> mappings = SetAutoMappingsMappings;

			var fnhConfiguration = Fluently.Configure ()
										   .Database (dbConf)
										   .Mappings (mappings)
											.ExposeConfiguration (PrintSchema);

			ISessionFactory factory;
			try {
				factory = fnhConfiguration.BuildSessionFactory ();
			} catch (Exception ex) {
				throw ex.InnerMostException ();
			}
			var session = factory.OpenSession ();
			Assert.AreEqual (1, session.CreateSQLQuery ("select 1").List () [0]);
		}

		[Test]

		public void ConnectoToSqlite_Automapped ()
		{
			//var dbConf = MySQLConfiguration.Standard.ConnectionString (connectionString).ShowSql ();
			var dbConf = SQLiteConfiguration.Standard.UsingFile (DbFileName).ShowSql ();
			Action<MappingConfiguration> mappings = SetAutoMappingsMappings;

			var fnhConfiguration = Fluently.Configure ()
										   .Database (dbConf)
										   .Mappings (mappings)
											.ExposeConfiguration (PrintSchema)
			                               .ExposeConfiguration (CreateSchema);

			ISessionFactory factory;
			try {
				factory = fnhConfiguration.BuildSessionFactory ();
			} catch (Exception ex) {
				throw ex.InnerMostException ();
			}
			var session = factory.OpenSession ();
			Assert.AreEqual (1, session.CreateSQLQuery ("select 1").List () [0]);
		}

		void SetAutoMappingsMappings (MappingConfiguration m)
		{
			var autopersistModel = AutoMap.Assembly (Assembly.GetExecutingAssembly (), new MyAutomappingConfiguration());

			autopersistModel.Conventions.Add <CascadeConvention>();

			m.AutoMappings.Add (autopersistModel);
		}

		void PrintSchema (Configuration conf)
		{
			var export = new SchemaExport (conf);
			export.Create (useStdOut: true, execute: false);
		}

		void CreateSchema (Configuration conf)
		{
			if (File.Exists (DbFileName)) File.Delete (DbFileName);
			var export = new SchemaExport (conf);
			export.Create (useStdOut: false, execute: true);
		}

	}

	class CascadeConvention : IReferenceConvention, IHasManyConvention, IHasManyToManyConvention
	{
		public void Apply (IManyToOneInstance instance)
		{
			instance.Cascade.All ();
		}

		public void Apply (IOneToManyCollectionInstance instance)
		{
			instance.Cascade.All ();
		}

		public void Apply (IManyToManyCollectionInstance instance)
		{
			instance.Cascade.All ();
		}

	}

	class MyAutomappingConfiguration : DefaultAutomappingConfiguration
	{
		public override bool ShouldMap (Type type)
		{
			// specify the criteria that types must meet in order to be mapped
			// any type for which this method returns false will not be mapped.
			return type.Namespace == "NHibernateOrm.Entities";
		}

		public override bool IsComponent (Type type)
		{
			// override this method to specify which types should be treated as components
			// if you have a large list of types, you should consider maintaining a list of them
			// somewhere or using some form of conventional and/or attribute design
			return type == typeof (Location);
		}
	}


}