using System;
using System.Data;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace SLIL.Classes
{
    internal enum AccountStates { AllOk, AlreadyUsed, NotFound, ErrorDownloading, Error }

    internal class PSD
    {
        public string ServerIp { get; set; }
        public string Username { get; set; }
        public string DBPassword { get; set; }
        public string DatabaseName { get; set; }
    }

    internal class SLILAccount
    {
        private readonly PSD AppSecrets;
        internal bool AllOk { get; set; }
        internal string HWID { get; set; }
        internal string Key { get; set; }
        
        internal SLILAccount(string key)
        {
            Key = key;
            HWID = Hasher.GetHWID();
            AppSecrets = JsonConvert.DeserializeObject<PSD>(Hasher.DecryptFile());
        }

        private bool CheckKey(string hashedKey) => Key == hashedKey;

        internal async Task<AccountStates> LoadAccount()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection($"Server={AppSecrets.ServerIp};Database={AppSecrets.DatabaseName};User Id={AppSecrets.Username};Password={AppSecrets.DBPassword};"))
                {
                    try
                    {
                        await connection.OpenAsync();
                        return await PerformDatabaseOperationsAndLoadUser(connection);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при открытии соединения: {ex.Message}");
                        return AccountStates.Error;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при подключении к базе данных: {ex.Message}");
                return AccountStates.Error;
            }
        }

        private async Task<AccountStates> PerformDatabaseOperationsAndLoadUser(MySqlConnection connection)
        {
            try
            {
                using (MySqlCommand command = new MySqlCommand("SELECT * FROM `keys` WHERE `key` = @key", connection))
                {
                    command.Parameters.AddWithValue("@key", Key);
                    DataTable dataTable = new DataTable();
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        await Task.Run(() => adapter.Fill(dataTable));
                        if (dataTable.Rows.Count > 0)
                        {
                            DataRow row = dataTable.Rows[0];
                            string hashedKey = (string)row["key"];
                            if (!string.IsNullOrEmpty(hashedKey) && CheckKey(hashedKey))
                            {
                                if (row["HWID"] == DBNull.Value || string.IsNullOrEmpty(row["HWID"].ToString()))
                                {
                                    using (MySqlCommand updateCommand = new MySqlCommand("UPDATE `keys` SET HWID = @hwid WHERE `key` = @key AND HWID IS NULL", connection))
                                    {
                                        updateCommand.Parameters.AddWithValue("@key", Key);
                                        updateCommand.Parameters.AddWithValue("@hwid", HWID);
                                        await updateCommand.ExecuteNonQueryAsync();
                                        AllOk = true;
                                        return AccountStates.AllOk;
                                    }
                                }
                                string dbHWID = (string)row["HWID"];
                                if (dbHWID == HWID)
                                {
                                    AllOk = true;
                                    return AccountStates.AllOk;
                                }
                                else return AccountStates.AlreadyUsed;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при работе с базой данных: {ex.Message}");
                return AccountStates.Error;
            }
            return AccountStates.NotFound;
        }
    }
}