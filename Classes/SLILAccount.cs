using System;
using System.Data;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace SLIL.Classes
{
    public enum AccountStates { AllOk, AlreadyUsed, NotFound, ErrorDownloading, Error }

    public class PSD
    {
        public string ServerIp { get; set; }
        public string Username { get; set; }
        public string DBPassword { get; set; }
        public string DatabaseName { get; set; }
    }

    public class SLILAccount
    {
        private readonly PSD AppSecrets;
        public bool AllOk { get; set; }
        public string HWID { get; set; }
        public string Key { get; set; }
        
        public SLILAccount(string key)
        {
            Key = key;
            HWID = Hasher.GetHWID();
            AppSecrets = JsonConvert.DeserializeObject<PSD>(Hasher.DecryptFile());
        }

        private bool CheckKey(string hashedKey) => Key == hashedKey;

        public async Task<AccountStates> LoadAccount()
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
                Console.WriteLine($"Ошибка при работе с базой данных: {ex.Message}");
                return AccountStates.Error;
            }
            return AccountStates.NotFound;
        }
    }
}