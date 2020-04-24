
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace YZFAdmin.Service
{
    public class ConnectionConfig
    {
        public string ConnectionString { get; set; }
        public DbStoreType DbType { get; set; }
    }

    public enum DbStoreType
    {
        MySql = 0,
        SqlServer = 1,
        Sqlite = 2,
        Oracle = 3
    }

    public class DapperFactoryOptions
    {
        public IList<Action<ConnectionConfig>> DapperActions { get; } = new List<Action<ConnectionConfig>>();
    }

    public interface IDapperFactory
    {
        DapperClient CreateClient(string name);
    }
    public class DefaultDapperFactory : IDapperFactory
    {
        private readonly IServiceProvider _services;
        private readonly IOptionsMonitor<DapperFactoryOptions> _optionsMonitor;

        public DefaultDapperFactory(IServiceProvider services, IOptionsMonitor<DapperFactoryOptions> optionsMonitor)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
        }

        public DapperClient CreateClient(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var client = new DapperClient(new ConnectionConfig { });

            var option = _optionsMonitor.Get(name).DapperActions.FirstOrDefault();
            if (option != null)
                option(client.CurrentConnectionConfig);
            else
                throw new ArgumentNullException(nameof(option));

            return client;
        }

    }

    public class DapperClient
    {
        public ConnectionConfig CurrentConnectionConfig { get; set; }

        public DapperClient(IOptionsMonitor<ConnectionConfig> config)
        {
            CurrentConnectionConfig = config.CurrentValue;
        }

        public DapperClient(ConnectionConfig config) { CurrentConnectionConfig = config; }

        IDbConnection _connection = null;
        public IDbConnection Connection
        {
            get
            {
                switch (CurrentConnectionConfig.DbType)
                {
                    //case DbStoreType.MySql:
                    //    _connection = new MySql.Data.MySqlClient.MySqlConnection(CurrentConnectionConfig.ConnectionString);
                    //    break;
                    //case DbStoreType.Sqlite:
                    //    _connection = new SQLiteConnection(CurrentConnectionConfig.ConnectionString);
                    //    break;
                    case DbStoreType.SqlServer:
                        _connection = new System.Data.SqlClient.SqlConnection(CurrentConnectionConfig.ConnectionString);
                        break;
                    case DbStoreType.Oracle:
                        //_connection = new Oracle.ManagedDataAccess.Client.OracleConnection(CurrentConnectionConfig.ConnectionString);
                        break;
                    default:
                        throw new Exception("未指定数据库类型！");
                }
                return _connection;
            }
        }

        /// <summary>
        /// 执行SQL返回集合
        /// </summary>
        /// <param name="strSql">sql语句</param>
        /// <returns></returns>
        public virtual List<T> Query<T>(string strSql)
        {
            using (IDbConnection conn = Connection)
            {
                return conn.Query<T>(strSql, null).ToList();
            }
        }

        /// <summary>
        /// 执行SQL返回集合
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="obj">参数model</param>
        /// <returns></returns>
        public virtual List<T> Query<T>(string strSql, object param)
        {
            using (IDbConnection conn = Connection)
            {
                return conn.Query<T>(strSql, param).ToList();
            }
        }

        /// <summary>
        /// 执行SQL返回一个对象
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <returns></returns>
        public virtual T QueryFirst<T>(string strSql)
        {
            using (IDbConnection conn = Connection)
            {
                return conn.Query<T>(strSql).FirstOrDefault<T>();
            }
        }

        /// <summary>
        /// 执行SQL返回一个对象
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <returns></returns>
        public virtual async Task<T> QueryFirstAsync<T>(string strSql)
        {
            using (IDbConnection conn = Connection)
            {
                var res = await conn.QueryAsync<T>(strSql);
                return res.FirstOrDefault<T>();
            }
        }

        /// <summary>
        /// 执行SQL返回一个对象
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="obj">参数model</param>
        /// <returns></returns>
        public virtual T QueryFirst<T>(string strSql, object param)
        {
            using (IDbConnection conn = Connection)
            {
                return conn.Query<T>(strSql, param).FirstOrDefault<T>();
            }
        }

        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="param">参数</param>
        /// <returns>0成功，-1执行失败</returns>
        public virtual int Execute(string strSql, object param)
        {
            using (IDbConnection conn = Connection)
            {
                try
                {
                    return conn.Execute(strSql, param) > 0 ? 0 : -1;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="strProcedure">过程名</param>
        /// <returns></returns>
        public virtual int ExecuteStoredProcedure(string strProcedure)
        {
            using (IDbConnection conn = Connection)
            {
                try
                {
                    return conn.Execute(strProcedure, null, null, null, CommandType.StoredProcedure) == 0 ? 0 : -1;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="strProcedure">过程名</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public virtual int ExecuteStoredProcedure(string strProcedure, object param)
        {
            using (IDbConnection conn = Connection)
            {
                try
                {
                    return conn.Execute(strProcedure, param, null, null, CommandType.StoredProcedure) == 0 ? 0 : -1;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


    }


    public static class DapperFactoryCollectionExtensions
    {
        public static IServiceCollection AddDapper(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddLogging();
            services.AddOptions();

            services.AddSingleton<DefaultDapperFactory>();
            services.TryAddSingleton<IDapperFactory>(serviceProvider => serviceProvider.GetRequiredService<DefaultDapperFactory>());

            return services;
        }

        public static IDapperFactoryBuilder AddDapper(this IServiceCollection services, string name, Action<ConnectionConfig> configureClient)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (configureClient == null)
                throw new ArgumentNullException(nameof(configureClient));

            AddDapper(services);

            var builder = new DefaultDapperFactoryBuilder(services, name);
            builder.ConfigureDapper(configureClient);
            return builder;
        }

        public static IDapperFactoryBuilder ConfigureDapper(this IDapperFactoryBuilder builder, Action<ConnectionConfig> configureClient)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (configureClient == null)
                throw new ArgumentNullException(nameof(configureClient));

            builder.Services.Configure<DapperFactoryOptions>(builder.Name, options => options.DapperActions.Add(configureClient));

            return builder;
        }

    }

    public interface IDapperFactoryBuilder
    {
        string Name { get; }

        IServiceCollection Services { get; }
    }

    internal class DefaultDapperFactoryBuilder : IDapperFactoryBuilder
    {
        public DefaultDapperFactoryBuilder(IServiceCollection services, string name)
        {
            Services = services;
            Name = name;
        }

        public string Name { get; }

        public IServiceCollection Services { get; }
    }


}
