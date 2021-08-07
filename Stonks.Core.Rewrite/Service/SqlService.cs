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
                    sqlCom.CommandText = $"INSERT INTO TABLE_{guildId} (USERID) VALUES(@USERID)";
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
                    sqlCom.CommandText = $"CREATE TABLE TABLE_{guildid} (_ID INT PRIMARY KEY AUTO_INCREMENT, USERID BIGINT NOT NULL, MONEY BIGINT NOT NULL, ROUND INT NOT NULL) ENGINE = INNODB;";
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
                                coin: Convert.ToUInt64(reader["COIN"]),
                                round: Convert.ToInt32(reader["ROUND"])
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
        /// 끝말잇기 데이터베이스에서 랜덤한 단어를 하나 불러와 반환합니다.
        /// </summary>
        /// <returns>랜덤한 단어</returns>
        public string GetRandomWord()
        {
            string result = string.Empty;

            try
            {
                _connection.Open();

                using (MySqlCommand sqlCom = new MySqlCommand())
                {
                    sqlCom.Connection = _connection;
                    sqlCom.CommandText = $"SELECT * FROM DICTIONARY AS R1 JOIN (SELECT(RAND() * (SELECT MAX(ID) FROM DICTIONARY)) AS ID) AS R2 WHERE R1.ID >= R2.ID ORDER BY R1.ID ASC LIMIT 1;";
                    sqlCom.CommandType = CommandType.Text;

                    using (MySqlDataReader reader = sqlCom.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result = Convert.ToString(reader["WORD"]);
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
                        throw new Exception($"단어 사전 테이블을 찾을 수 없습니다.");

                    default:
                        throw new Exception($"처리되지 못한 예외가 발생하였습니다. ({ex.ErrorCode})");
                }
            }

            return result;
        }

        /// <summary>
        /// 해당 글자로 시작하는 단어의 리스트를 가져옵니다.
        /// </summary>
        /// <param name="startWith">시작하는 글자</param>
        /// <returns>해당 글자로 시작하는 단어의 리스트</returns>
        public List<string> GetStartWords(string startWith)
        {
            List<string> result = new List<string>();

            try
            {
                _connection.Open();

                using (MySqlCommand sqlCom = new MySqlCommand())
                {
                    sqlCom.Connection = _connection;
                    sqlCom.CommandText = $"SELECT * FROM DICTIONARY WHERE WORD LIKE @WORD;";
                    sqlCom.Parameters.AddWithValue("@WORD", $"{startWith}%");
                    sqlCom.CommandType = CommandType.Text;

                    using (MySqlDataReader reader = sqlCom.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(Convert.ToString(reader["WORD"]));
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
                        throw new Exception($"단어 사전 테이블을 찾을 수 없습니다.");

                    default:
                        throw new Exception($"처리되지 못한 예외가 발생하였습니다. ({ex.ErrorCode})");
                }
            }

            return result;
        }

        /// <summary>
        /// 해당 단어가 존재하는지 확인합니다.
        /// </summary>
        /// <param name="word">확인할 단어</param>
        /// <returns>단어가 존재하는지 여부</returns>
        public bool IsWordExist(string word)
        {
            bool result = false;

            try
            {
                _connection.Open();

                using (MySqlCommand sqlCom = new MySqlCommand())
                {
                    sqlCom.Connection = _connection;
                    sqlCom.CommandText = $"SELECT WORD FROM DICTIONARY WHERE WORD=@WORD LIMIT 1;";
                    sqlCom.Parameters.AddWithValue("@WORD", word);
                    sqlCom.CommandType = CommandType.Text;

                    using (MySqlDataReader reader = sqlCom.ExecuteReader())
                    {
                        result = reader.HasRows;
                    }
                }

                _connection.Close();
            }
            catch (MySqlException ex)
            {
                switch (ex.ErrorCode)
                {
                    case 1146: //ER_NO_SUCH_TABLE
                        throw new Exception($"단어 사전 테이블을 찾을 수 없습니다.");

                    default:
                        throw new Exception($"처리되지 못한 예외가 발생하였습니다. ({ex.ErrorCode})");
                }
            }

            return result;
        }

        /// <summary>
        /// 데이터베이스에서 단어를 검색합니다.
        /// </summary>
        /// <param name="word">찾을 단어</param>
        /// <returns>해당 글자로 시작하는 단어의 리스트</returns>
        public List<string> SearchWord(string word)
        {
            List<string> words = new List<string>();

            try
            {
                _connection.Open();

                using (MySqlCommand sqlCom = new MySqlCommand())
                {
                    sqlCom.Connection = _connection;
                    sqlCom.CommandText = $"SELECT * FROM DICTIONARY WHERE WORD LIKE @TEXT ORDER BY LENGTH(WORD);";
                    sqlCom.Parameters.AddWithValue("@TEXT", $"%{word}%");
                    sqlCom.CommandType = CommandType.Text;

                    using (MySqlDataReader reader = sqlCom.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            words.Add(Convert.ToString(reader["WORD"]));
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
                        throw new Exception($"단어 사전 테이블을 찾을 수 없습니다.");

                    default:
                        throw new Exception($"처리되지 못한 예외가 발생하였습니다. ({ex.ErrorCode})");
                }
            }

            return words;
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
                                coin: Convert.ToUInt64(reader["MONEY"]),
                                round: Convert.ToInt32(reader["ROUND"])
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
    }
}
