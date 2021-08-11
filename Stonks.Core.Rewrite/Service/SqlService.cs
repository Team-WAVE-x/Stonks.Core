using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using Stonks.Core.Rewrite.Class;
using System;
using System.Collections.Generic;
using System.Data;

namespace Stonks.Core.Rewrite.Service
{
    public class SqlService
    {
        private readonly Setting _setting;
        private readonly MySqlConnection _connection;

        public SqlService(IServiceProvider service)
        {
            _setting = service.GetRequiredService<Setting>();
            _connection = new MySqlConnection(_setting.Config.ConnectionString);
        }

        /// <summary>
        /// 길드 테이블에 새로운 유저 추가
        /// </summary>
        /// <param name="guildid">길드 아이디</param>
        /// <param name="userid">추가할 유저 아이디</param>
        public void AddNewUser(ulong guildId, ulong userId)
        {
            try
            {
                _connection.Open();

                using (MySqlCommand sqlCom = new MySqlCommand())
                {
                    sqlCom.Connection = _connection;
                    sqlCom.CommandText = $"INSERT INTO TABLE_{guildId} (USERID, MONEY) VALUES(@USERID, 0)";
                    sqlCom.Parameters.AddWithValue("@USERID", userId);
                    sqlCom.CommandType = CommandType.Text;
                    sqlCom.ExecuteNonQuery();
                }

                _connection.Close();
            }
            catch (MySqlException ex)
            {
                switch (ex.ErrorCode)
                {
                    case 1146: //ER_NO_SUCH_TABLE
                        AddNewGuild(guildId);
                        break;

                    case 1049: //ER_BAD_DB_ERROR
                        CreateDatabase();
                        break;

                    case 1050: //ER_TABLE_EXISTS_ERROR
                        throw new Exception("유저가 이미 존재합니다.");

                    default:
                        throw new Exception($"처리되지 못한 예외가 발생하였습니다. ({ex.ErrorCode})");
                }
            }
        }

        /// <summary>
        /// 길드를 데이터베이스에 새로 추가합니다.
        /// </summary>
        /// <param name="guildid">길드 아이디</param>
        public void AddNewGuild(ulong guildid)
        {
            try
            {
                _connection.Open();

                using (MySqlCommand sqlCom = new MySqlCommand())
                {
                    sqlCom.Connection = _connection;
                    sqlCom.CommandText = $"CREATE TABLE TABLE_{guildid} (_ID INT PRIMARY KEY AUTO_INCREMENT, USERID BIGINT NOT NULL, MONEY BIGINT NOT NULL) ENGINE = INNODB;";
                    sqlCom.CommandType = CommandType.Text;
                    sqlCom.ExecuteNonQuery();
                }

                _connection.Close();
            }
            catch (MySqlException ex)
            {
                switch (ex.ErrorCode)
                {
                    case 1050: //ER_TABLE_EXISTS_ERROR
                        throw new Exception("길드가 이미 존재합니다.");

                    default:
                        throw new Exception($"처리되지 못한 예외가 발생하였습니다. ({ex.ErrorCode})");
                }
            }
        }

        /// <summary>
        /// 데이터베이스를 생성합니다.
        /// </summary>
        public void CreateDatabase()
        {
            try
            {
                _connection.Open();

                using (MySqlCommand sqlCom = new MySqlCommand())
                {
                    sqlCom.Connection = _connection;
                    sqlCom.CommandText = $"CREATE DATABASE IF NOT EXISTS `STONKS_DB`;";
                    sqlCom.CommandType = CommandType.Text;
                    sqlCom.ExecuteNonQuery();
                }

                _connection.Close();
            }
            catch (MySqlException ex)
            {
                switch (ex.ErrorCode)
                {
                    case 1007: //ER_DB_CREATE_EXISTS
                        throw new Exception("데이터베이스가 이미 존재합니다.");

                    default:
                        throw new Exception($"처리되지 못한 예외가 발생하였습니다. ({ex.ErrorCode})");
                }
            }
        }

        /// <summary>
        /// 랭킹을 데이터베이스로 부터 가져옵니다.
        /// </summary>
        /// <param name="guildid">길드 아이디</param>
        /// <param name="limit">랭킹 유저의 수</param>
        /// <returns></returns>
        public List<User> GetRanking(ulong guildId, int limit)
        {
            List<User> users = new List<User>();

            try
            {
                _connection.Open();

                using (MySqlCommand sqlCom = new MySqlCommand())
                {
                    sqlCom.Connection = _connection;
                    sqlCom.CommandText = $"SELECT * FROM TABLE_{guildId} WHERE NOT MONEY=0 ORDER BY MONEY DESC LIMIT @LIMIT;";
                    sqlCom.Parameters.AddWithValue("@LIMIT", limit);
                    sqlCom.CommandType = CommandType.Text;

                    using (MySqlDataReader reader = sqlCom.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User(
                                id: Convert.ToUInt64(reader["_ID"]),
                                guildId: guildId,
                                userId: Convert.ToUInt64(reader["USERID"]),
                                coin: Convert.ToUInt64(reader["MONEY"])
                            ));
                        }
                    }
                }

                _connection.Close();
            }
            catch (MySqlException ex)
            {
                switch (ex.ErrorCode)
                {
                    case 1146: //ER_NO_SUCH_TABLE
                        AddNewGuild(guildId);
                        users = new List<User>();
                        break;

                    default:
                        throw new Exception($"처리되지 못한 예외가 발생하였습니다. ({ex.ErrorCode})");
                }
            }

            return users;
        }

        /// <summary>
        /// 유저를 데이터베이스에서 가져와 객체로 반환합니다.
        /// </summary>
        /// <param name="guildId">길드의 아이디</param>
        /// <param name="userId">유저의 아이디</param>
        /// <returns></returns>
        public User GetUser(ulong guildId, ulong userId)
        {
            User user = null;

            try
            {
                _connection.Open();

                using (MySqlCommand sqlCom = new MySqlCommand())
                {
                    sqlCom.Connection = _connection;
                    sqlCom.CommandText = $"SELECT * FROM TABLE_{guildId} WHERE USERID=@ID ORDER BY MONEY DESC LIMIT 1;";
                    sqlCom.Parameters.AddWithValue("@ID", userId);
                    sqlCom.CommandType = CommandType.Text;

                    using (MySqlDataReader reader = sqlCom.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user = new User(
                                id: Convert.ToUInt64(reader["_ID"]),
                                guildId: guildId,
                                userId: Convert.ToUInt64(reader["USERID"]),
                                coin: Convert.ToUInt64(reader["MONEY"])
                            );
                        }
                    }
                }

                _connection.Close();
            }
            catch (MySqlException ex)
            {
                switch (ex.ErrorCode)
                {
                    case 1146: //ER_NO_SUCH_TABLE
                        AddNewGuild(guildId);
                        break;

                    default:
                        throw new Exception($"처리되지 못한 예외가 발생하였습니다. ({ex.ErrorCode})");
                }
            }

            return user;
        }

        /// <summary>
        /// 유저에게 코인을 추가합니다.
        /// </summary>
        /// <param name="guildId">길드 아이디</param>
        /// <param name="userId">유저 아이디</param>
        /// <param name="coin">추가할 코인의 량</param>
        public void AddUserCoin(ulong guildId, ulong userId, int coin)
        {
            _connection.Open();

            try
            {
                using (MySqlCommand sqlCom = new MySqlCommand())
                {
                    sqlCom.Connection = _connection;
                    sqlCom.CommandText = $"UPDATE TABLE_{guildId} SET MONEY=MONEY+@MONEY WHERE USERID=@ID";
                    sqlCom.Parameters.AddWithValue("@MONEY", coin);
                    sqlCom.Parameters.AddWithValue("@ID", userId);
                    sqlCom.CommandType = CommandType.Text;
                    sqlCom.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                switch (ex.ErrorCode)
                {
                    case 1146: //ER_NO_SUCH_TABLE
                        AddNewGuild(guildId);
                        break;

                    default:
                        throw new Exception($"처리되지 못한 예외가 발생하였습니다. ({ex.ErrorCode})");
                }
            }

            _connection.Close();
        }

        /// <summary>
        /// 유저에게서 코인을 가져갑니다.
        /// </summary>
        /// <param name="guildId">길드 아이디</param>
        /// <param name="userId">유저 아이디</param>
        /// <param name="coin">추가할 코인의 량</param>
        public void SubUserCoin(ulong guildId, ulong userId, int coin)
        {
            _connection.Open();

            try
            {
                using (MySqlCommand sqlCom = new MySqlCommand())
                {
                    sqlCom.Connection = _connection;
                    sqlCom.CommandText = $"UPDATE TABLE_{guildId} SET MONEY=MONEY-@MONEY WHERE USERID=@ID";
                    sqlCom.Parameters.AddWithValue("@MONEY", coin);
                    sqlCom.Parameters.AddWithValue("@ID", userId);
                    sqlCom.CommandType = CommandType.Text;
                    sqlCom.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                switch (ex.ErrorCode)
                {
                    case 1146: //ER_NO_SUCH_TABLE
                        AddNewGuild(guildId);
                        break;

                    default:
                        throw new Exception($"처리되지 못한 예외가 발생하였습니다. ({ex.ErrorCode})");
                }
            }

            _connection.Close();
        }
    }
}