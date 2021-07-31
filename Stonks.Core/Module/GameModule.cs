using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Data;

using Stonks.Core.Class;

namespace Stonks.Core.Module
{
    public static class GameModule
    {
        /// <summary>
        /// 길드 테이블에 새로운 유저 추가
        /// </summary>
        /// <param name="guildid">길드 아이디</param>
        /// <param name="userid">추가할 유저 아이디</param>
        public static void AddNewUser(ulong guildid, ulong userid)
        {
            using (MySqlConnection sCon = new MySqlConnection(Program.Setting.Config.ConnectionString))
            {
                sCon.Open();

                using (MySqlCommand sqlCom = new MySqlCommand())
                {
                    sqlCom.Connection = sCon;
                    sqlCom.CommandText = $"INSERT INTO TABLE_{guildid} (USERID, MONEY, ROUND) VALUES(@USERID, 0, 0)";
                    sqlCom.Parameters.AddWithValue("@USERID", userid);
                    sqlCom.CommandType = CommandType.Text;
                    sqlCom.ExecuteNonQuery();
                }

                sCon.Close();
            }
        }

        /// <summary>
        /// 길드 추가
        /// </summary>
        /// <param name="guildid">길드 아이디</param>
        public static void AddNewGuild(ulong guildid)
        {
            using (MySqlConnection sCon = new MySqlConnection(Program.Setting.Config.ConnectionString))
            {
                sCon.Open();

                using (MySqlCommand sqlCom = new MySqlCommand())
                {
                    sqlCom.Connection = sCon;
                    sqlCom.CommandText = $"CREATE TABLE TABLE_{guildid} (_ID INT PRIMARY KEY AUTO_INCREMENT, USERID BIGINT NOT NULL, MONEY BIGINT NOT NULL, ROUND INT NOT NULL) ENGINE = INNODB;";
                    sqlCom.CommandType = CommandType.Text;
                    sqlCom.ExecuteNonQuery();
                }

                sCon.Close();
            }
        }

        /// <summary>
        /// 재력 랭킹 가져오기
        /// </summary>
        /// <param name="guildid">길드 아이디</param>
        /// <param name="limit">제한</param>
        /// <returns></returns>
        public static List<User> GetRanking(ulong guildid, int limit)
        {
            List<User> users = new List<User>();

            try
            {
                using (MySqlConnection sCon = new MySqlConnection(Program.Setting.Config.ConnectionString))
                {
                    sCon.Open();

                    using (MySqlCommand sqlCom = new MySqlCommand())
                    {
                        sqlCom.Connection = sCon;
                        sqlCom.CommandText = $"SELECT * FROM TABLE_{guildid} WHERE NOT MONEY=0 ORDER BY MONEY DESC LIMIT @LIMIT;";
                        sqlCom.Parameters.AddWithValue("@LIMIT", limit);
                        sqlCom.CommandType = CommandType.Text;

                        using (MySqlDataReader reader = sqlCom.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(new Class.User(guildid, Convert.ToUInt64(reader["userid"])));
                            }
                        }
                    }

                    sCon.Close();
                }
            }
            catch (Exception)
            {
                users = new List<User>();
            }

            return users;
        }

        /// <summary>
        /// 끝말잇기 랭킹 가져오기
        /// </summary>
        /// <param name="guildid">길드 아이디</param>
        /// <param name="limit">제한</param>
        /// <returns></returns>
        public static List<User> GetRoundRanking(ulong guildid, int limit)
        {
            List<User> users = new List<User>();

            using (MySqlConnection sCon = new MySqlConnection(Program.Setting.Config.ConnectionString))
            {
                sCon.Open();

                using (MySqlCommand sqlCom = new MySqlCommand())
                {
                    sqlCom.Connection = sCon;
                    sqlCom.CommandText = $"SELECT * FROM TABLE_{guildid} WHERE NOT ROUND=0 ORDER BY ROUND DESC LIMIT @LIMIT;";
                    sqlCom.Parameters.AddWithValue("@LIMIT", limit);
                    sqlCom.CommandType = CommandType.Text;

                    using (MySqlDataReader reader = sqlCom.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new Class.User(guildid, Convert.ToUInt64(reader["USERID"])));
                        }
                    }
                }

                sCon.Close();
            }

            return users;
        }

        /// <summary>
        /// 끝말잇기 단어 데이터베이스에서 랜덤한 단어 하나 뽑아서 가져오기
        /// </summary>
        /// <returns>랜덤한 단어</returns>
        public static string GetRandomWords()
        {
            string result = string.Empty;

            using (MySqlConnection sCon = new MySqlConnection(Program.Setting.Config.ConnectionString))
            {
                sCon.Open();

                using (MySqlCommand sqlCom = new MySqlCommand())
                {
                    sqlCom.Connection = sCon;
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

                sCon.Close();

                return result;
            }
        }

        /// <summary>
        /// 해당 글자로 시작하는 단어 가져오기
        /// </summary>
        /// <param name="startwith">시작하는 단어</param>
        /// <returns></returns>
        public static List<string> GetStartWords(string startwith)
        {
            List<string> result = new List<string>();

            using (MySqlConnection sCon = new MySqlConnection(Program.Setting.Config.ConnectionString))
            {
                sCon.Open();

                using (MySqlCommand sqlCom = new MySqlCommand())
                {
                    sqlCom.Connection = sCon;
                    sqlCom.CommandText = $"SELECT * FROM DICTIONARY WHERE WORD LIKE @WORD;";
                    sqlCom.Parameters.AddWithValue("@WORD", $"{startwith}%");
                    sqlCom.CommandType = CommandType.Text;

                    using (MySqlDataReader reader = sqlCom.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(Convert.ToString(reader["WORD"]));
                        }
                    }
                }

                sCon.Close();

                return result;
            }
        }

        /// <summary>
        /// 해당 단어가 존재하는지 확인하기
        /// </summary>
        /// <param name="word">확인할 단어</param>
        /// <returns></returns>
        public static bool IsWordExist(string word)
        {
            bool result = false;

            using (MySqlConnection sCon = new MySqlConnection(Program.Setting.Config.ConnectionString))
            {
                sCon.Open();

                using (MySqlCommand sqlCom = new MySqlCommand())
                {
                    sqlCom.Connection = sCon;
                    sqlCom.CommandText = $"SELECT WORD FROM DICTIONARY WHERE WORD=@WORD LIMIT 1;";
                    sqlCom.Parameters.AddWithValue("@WORD", word);
                    sqlCom.CommandType = CommandType.Text;

                    using (MySqlDataReader reader = sqlCom.ExecuteReader())
                    {
                        result = reader.HasRows;
                    }
                }

                sCon.Close();

                return result;
            }
        }

        /// <summary>
        /// 단어 찾기
        /// </summary>
        /// <param name="word">찾을 단어</param>
        /// <returns></returns>
        public static List<string> SearchWord(string word)
        {
            List<string> words = new List<string>();

            using (MySqlConnection sCon = new MySqlConnection(Program.Setting.Config.ConnectionString))
            {
                sCon.Open();

                using (MySqlCommand sqlCom = new MySqlCommand())
                {
                    sqlCom.Connection = sCon;
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

                sCon.Close();

                return words;
            }
        }
    }
}