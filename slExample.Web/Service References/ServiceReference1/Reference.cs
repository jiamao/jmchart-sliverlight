﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.235
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace slExample.Web.ServiceReference1 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceReference1.IDBHelper")]
    public interface IDBHelper {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDBHelper/ExecuteNonQuery", ReplyAction="http://tempuri.org/IDBHelper/ExecuteNonQueryResponse")]
        int ExecuteNonQuery(DoNet.Common.DbUtility.Proxy.DBInfo dbinfo, string cmdText);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDBHelper/ExecuteNonQueryWithParam", ReplyAction="http://tempuri.org/IDBHelper/ExecuteNonQueryWithParamResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataSet))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataTable[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataTable))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataColumn[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataColumn))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataRow[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataRow))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataItem[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataItem))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Proxy.DBInfo))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(string[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(object[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(System.Collections.Generic.Dictionary<string, object>))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(string[][]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(object[][]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.DbType))]
        int ExecuteNonQueryWithParam(DoNet.Common.DbUtility.Proxy.DBInfo dbinfo, string cmdText, string[] paraNames, object[] paraValues);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDBHelper/ExecuteNonQueryWithParams", ReplyAction="http://tempuri.org/IDBHelper/ExecuteNonQueryWithParamsResponse")]
        int ExecuteNonQueryWithParams(DoNet.Common.DbUtility.Proxy.DBInfo dbinfo, string cmdText, System.Collections.Generic.Dictionary<string, object> dbparams);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDBHelper/GetDataSet", ReplyAction="http://tempuri.org/IDBHelper/GetDataSetResponse")]
        DoNet.Common.DbUtility.Data.DataSet GetDataSet(DoNet.Common.DbUtility.Proxy.DBInfo dbinfo, string cmdText);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDBHelper/GetDataSetWithParam", ReplyAction="http://tempuri.org/IDBHelper/GetDataSetWithParamResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataSet))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataTable[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataTable))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataColumn[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataColumn))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataRow[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataRow))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataItem[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataItem))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Proxy.DBInfo))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(string[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(object[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(System.Collections.Generic.Dictionary<string, object>))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(string[][]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(object[][]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.DbType))]
        DoNet.Common.DbUtility.Data.DataSet GetDataSetWithParam(DoNet.Common.DbUtility.Proxy.DBInfo dbinfo, string cmdText, string[] paraNames, object[] paraValues);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDBHelper/ExecuteScalar", ReplyAction="http://tempuri.org/IDBHelper/ExecuteScalarResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataSet))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataTable[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataTable))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataColumn[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataColumn))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataRow[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataRow))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataItem[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataItem))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Proxy.DBInfo))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(string[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(object[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(System.Collections.Generic.Dictionary<string, object>))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(string[][]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(object[][]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.DbType))]
        object ExecuteScalar(DoNet.Common.DbUtility.Proxy.DBInfo dbinfo, string cmdText);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDBHelper/ExecuteScalarWithParam", ReplyAction="http://tempuri.org/IDBHelper/ExecuteScalarWithParamResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataSet))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataTable[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataTable))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataColumn[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataColumn))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataRow[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataRow))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataItem[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Data.DataItem))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.Proxy.DBInfo))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(string[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(object[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(System.Collections.Generic.Dictionary<string, object>))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(string[][]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(object[][]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(DoNet.Common.DbUtility.DbType))]
        object ExecuteScalarWithParam(DoNet.Common.DbUtility.Proxy.DBInfo dbinfo, string cmdText, string[] paraNames, object[] paraValues);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDBHelper/ExecuteSql", ReplyAction="http://tempuri.org/IDBHelper/ExecuteSqlResponse")]
        int ExecuteSql(DoNet.Common.DbUtility.Proxy.DBInfo dbinfo, string[] sqls);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDBHelper/ExecuteSqlWithParam", ReplyAction="http://tempuri.org/IDBHelper/ExecuteSqlWithParamResponse")]
        int ExecuteSqlWithParam(DoNet.Common.DbUtility.Proxy.DBInfo dbinfo, string[] sqls, string[][] paraNames, object[][] paraValues);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IDBHelperChannel : slExample.Web.ServiceReference1.IDBHelper, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class DBHelperClient : System.ServiceModel.ClientBase<slExample.Web.ServiceReference1.IDBHelper>, slExample.Web.ServiceReference1.IDBHelper {
        
        public DBHelperClient() {
        }
        
        public DBHelperClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public DBHelperClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DBHelperClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DBHelperClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public int ExecuteNonQuery(DoNet.Common.DbUtility.Proxy.DBInfo dbinfo, string cmdText) {
            return base.Channel.ExecuteNonQuery(dbinfo, cmdText);
        }
        
        public int ExecuteNonQueryWithParam(DoNet.Common.DbUtility.Proxy.DBInfo dbinfo, string cmdText, string[] paraNames, object[] paraValues) {
            return base.Channel.ExecuteNonQueryWithParam(dbinfo, cmdText, paraNames, paraValues);
        }
        
        public int ExecuteNonQueryWithParams(DoNet.Common.DbUtility.Proxy.DBInfo dbinfo, string cmdText, System.Collections.Generic.Dictionary<string, object> dbparams) {
            return base.Channel.ExecuteNonQueryWithParams(dbinfo, cmdText, dbparams);
        }
        
        public DoNet.Common.DbUtility.Data.DataSet GetDataSet(DoNet.Common.DbUtility.Proxy.DBInfo dbinfo, string cmdText) {
            return base.Channel.GetDataSet(dbinfo, cmdText);
        }
        
        public DoNet.Common.DbUtility.Data.DataSet GetDataSetWithParam(DoNet.Common.DbUtility.Proxy.DBInfo dbinfo, string cmdText, string[] paraNames, object[] paraValues) {
            return base.Channel.GetDataSetWithParam(dbinfo, cmdText, paraNames, paraValues);
        }
        
        public object ExecuteScalar(DoNet.Common.DbUtility.Proxy.DBInfo dbinfo, string cmdText) {
            return base.Channel.ExecuteScalar(dbinfo, cmdText);
        }
        
        public object ExecuteScalarWithParam(DoNet.Common.DbUtility.Proxy.DBInfo dbinfo, string cmdText, string[] paraNames, object[] paraValues) {
            return base.Channel.ExecuteScalarWithParam(dbinfo, cmdText, paraNames, paraValues);
        }
        
        public int ExecuteSql(DoNet.Common.DbUtility.Proxy.DBInfo dbinfo, string[] sqls) {
            return base.Channel.ExecuteSql(dbinfo, sqls);
        }
        
        public int ExecuteSqlWithParam(DoNet.Common.DbUtility.Proxy.DBInfo dbinfo, string[] sqls, string[][] paraNames, object[][] paraValues) {
            return base.Channel.ExecuteSqlWithParam(dbinfo, sqls, paraNames, paraValues);
        }
    }
}
