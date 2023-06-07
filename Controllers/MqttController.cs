using Microsoft.AspNetCore.Mvc;
using DAPI.Class;
using System.Data;
using System.Data.SqlClient;


namespace DAPI.Controllers
{
    public class MqttController : Controller
    {
        private readonly string StringConnection = "Server = localhost\\SQLEXPRESS; Database= DB_DanieliAutomation; Trusted_connection=true; ";

        [HttpPost]
        [Route("MqttApi")]

        public string MqttApi(MqttRequest myMqttRequest)
        {

            SqlConnection? mySqlConnection = null;
            string result = "";

            try
            {
                mySqlConnection = new SqlConnection(StringConnection);
                mySqlConnection.Open();
                SqlCommand myCommandUpdate = mySqlConnection.CreateCommand();

                myCommandUpdate.Parameters.Add("@IdDetail", System.Data.SqlDbType.Int);
                myCommandUpdate.Parameters.Add("@Value", System.Data.SqlDbType.Float);
                myCommandUpdate.Parameters["@IdDetail"].Value = myMqttRequest.ID;
                myCommandUpdate.Parameters["@Value"].Value = myMqttRequest.Value;

                if (!CheckifDetailExist(myMqttRequest.ID))
                {
                    result = "The id does not exist";
                    return result;  
                }


                myCommandUpdate.CommandText = "UPDATE Details SET Value=@Value WHERE ID=@IdDetail AND Deleted=0";

                myCommandUpdate.ExecuteNonQuery();

                result = "Success";

            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            finally
            {
                mySqlConnection?.Close();
            }
            return result;
        }  
        
        private bool CheckifDetailExist (int IdDetail) {

            SqlConnection? mySqlConnection = null;

            try
            {
                mySqlConnection = new SqlConnection(StringConnection);
                mySqlConnection.Open();
                SqlCommand mySqlCommandCheck = mySqlConnection.CreateCommand();

                mySqlCommandCheck.Parameters.Add("@ID", SqlDbType.Int);
                mySqlCommandCheck.Parameters["@ID"].Value = IdDetail;

                string query = "SELECT ID FROM Details WHERE ID=@ID AND Deleted=0";


                mySqlCommandCheck.CommandText = query;
                SqlDataReader mySqlReader = mySqlCommandCheck.ExecuteReader();

                if (mySqlReader.Read())
                {
                    return true;
                }

                return false;

            }
            catch
            {
                return false;
            }
            finally 
            { 
                mySqlConnection?.Close(); 
            }   
        }
        
    }

}