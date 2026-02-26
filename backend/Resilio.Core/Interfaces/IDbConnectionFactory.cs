using System.Data;

namespace Resilio.Core.Interfaces;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}