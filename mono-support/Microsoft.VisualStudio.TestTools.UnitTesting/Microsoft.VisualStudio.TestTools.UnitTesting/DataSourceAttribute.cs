// Type: Microsoft.VisualStudio.TestTools.UnitTesting.DataSourceAttribute
// Assembly: Microsoft.VisualStudio.QualityTools.UnitTestFramework, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F644D728-C1F1-48EF-B944-A5D0A59DED82
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\ReferenceAssemblies\v4.0\Microsoft.VisualStudio.QualityTools.UnitTestFramework.dll

using System;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class DataSourceAttribute : Attribute
    {
        public static readonly string DefaultProviderName = "System.Data.OleDb";
        public static readonly DataAccessMethod DefaultDataAccessMethod = DataAccessMethod.Random;
        private string m_invariantProviderName = DataSourceAttribute.DefaultProviderName;
        private string m_connectionString;
        private string m_tableName;
        private DataAccessMethod m_accessMethod;
        private string m_dataSourceSettingName;

        public string ProviderInvariantName
        {
            get
            {
                return this.m_invariantProviderName;
            }
        }

        public string ConnectionString
        {
            get
            {
                return this.m_connectionString;
            }
        }

        public string TableName
        {
            get
            {
                return this.m_tableName;
            }
        }

        public DataAccessMethod DataAccessMethod
        {
            get
            {
                return this.m_accessMethod;
            }
        }

        public string DataSourceSettingName
        {
            get
            {
                return this.m_dataSourceSettingName;
            }
        }

        static DataSourceAttribute()
        {
        }

        public DataSourceAttribute(string providerInvariantName, string connectionString, string tableName, DataAccessMethod dataAccessMethod)
        {
            this.m_invariantProviderName = providerInvariantName;
            this.m_connectionString = connectionString;
            this.m_tableName = tableName;
            this.m_accessMethod = dataAccessMethod;
        }

        public DataSourceAttribute(string connectionString, string tableName)
            : this(DataSourceAttribute.DefaultProviderName, connectionString, tableName, DataSourceAttribute.DefaultDataAccessMethod)
        {
        }

        public DataSourceAttribute(string dataSourceSettingName)
        {
            this.m_dataSourceSettingName = dataSourceSettingName;
        }
    }
}
