using MySql.Data.MySqlClient;

using System;
using System.Data;

using static Stonks.Core.Module.GameModule;
using static Stonks.Core.Module.SettingModule;

namespace Stonks.Core.Class
{
    public class User
    {
        public ulong Id { get; }
        public ulong GuildId { get; }
        public ulong UserId { get; }
        public ulong Money { get; }
        public int Round { get; }

        public User(ulong guildid, ulong id)
        {
            UserId = id;
            GuildId = guildid;

            try
            {
                using (MySqlConnection sCon = new MySqlConnection(Program.Setting.ConnectionString))
                {
                    sCon.Open();

                    using (MySqlCommand sqlCom = new MySqlCommand())
                    {
                        sqlCom.Connection = sCon;
                        sqlCom.CommandText = $"SELECT * FROM TABLE_{guildid} WHERE USERID=@ID";
                        sqlCom.Parameters.AddWithValue("@ID", id);
                        sqlCom.CommandType = CommandType.Text;

                        using (MySqlDataReader reader = sqlCom.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Id = Convert.ToUInt64(reader["_ID"]);
                                Money = Convert.ToUInt64(reader["MONEY"]);
                                Round = Convert.ToInt32(reader["ROUND"]);
                            }

                            if (!reader.HasRows)
                            {
                                AddNewUser(guildid, id);
                            }
                        }
                    }

                    sCon.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                AddNewGuild(guildid);
                AddNewUser(guildid, id);
            }
        }

        public void AddMoney(ulong money)
        {
            using (MySqlConnection sCon = new MySqlConnection(Program.Setting.ConnectionString))
            {
                sCon.Open();

                using (MySqlCommand sqlCom = new MySqlCommand())
                {
                    sqlCom.Connection = sCon;
                    sqlCom.CommandText = $"UPDATE TABLE_{GuildId} SET MONEY=MONEY+@MONEY WHERE USERID=@ID";
                    sqlCom.Parameters.AddWithValue("@MONEY", money);
                    sqlCom.Parameters.AddWithValue("@ID", UserId);
                    sqlCom.CommandType = CommandType.Text;
                    sqlCom.ExecuteNonQuery();
                }

                sCon.Close();
            }
        }

        public void SubMoney(ulong money)
        {
            if (Money > 0)
            {
                using (MySqlConnection sCon = new MySqlConnection(Program.Setting.ConnectionString))
                {
                    sCon.Open();

                    using (MySqlCommand sqlCom = new MySqlCommand())
                    {
                        sqlCom.Connection = sCon;
                        sqlCom.CommandText = $"UPDATE TABLE_{GuildId} SET MONEY=MONEY-@MONEY WHERE USERID=@ID";
                        sqlCom.Parameters.AddWithValue("@MONEY", money);
                        sqlCom.Parameters.AddWithValue("@ID", UserId);
                        sqlCom.CommandType = CommandType.Text;
                        sqlCom.ExecuteNonQuery();
                    }

                    sCon.Close();
                }
            }
        }

        public void SetScore(int round)
        {
            using (MySqlConnection sCon = new MySqlConnection(Program.Setting.ConnectionString))
            {
                sCon.Open();

                using (MySqlCommand sqlCom = new MySqlCommand())
                {
                    sqlCom.Connection = sCon;
                    sqlCom.CommandText = $"UPDATE TABLE_{GuildId} SET ROUND=@ROUND WHERE USERID=@id";
                    sqlCom.Parameters.AddWithValue("@ROUND", round);
                    sqlCom.Parameters.AddWithValue("@ID", UserId);
                    sqlCom.CommandType = CommandType.Text;
                    sqlCom.ExecuteNonQuery();
                }

                sCon.Close();
            }
        }
    }
}