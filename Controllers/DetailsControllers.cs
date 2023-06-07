using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Specialized;
using DAPI.Class;


namespace DAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class DetailsController : ControllerBase
    {


        private readonly string StringConnection = "Server = localhost\\SQLEXPRESS; Database= DB_DanieliAutomation; Trusted_connection=true; ";

        /* -------------------------------------------------------------------------------------------------------------------------------------------------------------------------- */

        [Route("InsertNewDetail")]
        [HttpPost]
        public string PostParameters(Detail myDetail)
        {

            SqlConnection? mysqlConnection = null;
            string result = "";


            if (myDetail.Parameter == "" || myDetail.Description == "" || myDetail.Value.Equals(null) || myDetail.FK.Equals(null))
            {
                result = "You must put the key information";
                return result;
            }

            try
            {
                mysqlConnection = new SqlConnection(StringConnection);
                SqlCommand mySqlCommandInsertDetail = new SqlCommand();
                mysqlConnection.Open();
                mySqlCommandInsertDetail.Connection = mysqlConnection;

                mySqlCommandInsertDetail.Parameters.Add("@Parameter", SqlDbType.NVarChar);
                mySqlCommandInsertDetail.Parameters.Add("@Description", SqlDbType.NVarChar);
                mySqlCommandInsertDetail.Parameters.Add("@Value", SqlDbType.Float);
                mySqlCommandInsertDetail.Parameters.Add("@DateEntry", SqlDbType.DateTime);
                mySqlCommandInsertDetail.Parameters.Add("@Note", SqlDbType.NVarChar);
                mySqlCommandInsertDetail.Parameters.Add("@FK", SqlDbType.Int);
                mySqlCommandInsertDetail.Parameters.Add("@GreenLimit", SqlDbType.Float);
                mySqlCommandInsertDetail.Parameters.Add("@YellowLimit", SqlDbType.Float);

                mySqlCommandInsertDetail.Parameters["@Parameter"].Value = myDetail.Parameter;
                mySqlCommandInsertDetail.Parameters["@Description"].Value = myDetail.Description;
                mySqlCommandInsertDetail.Parameters["@Value"].Value = myDetail.Value;
                mySqlCommandInsertDetail.Parameters["@DateEntry"].Value = myDetail.DateEntry;
                mySqlCommandInsertDetail.Parameters["@Note"].Value = myDetail.Note;
                mySqlCommandInsertDetail.Parameters["@FK"].Value = myDetail.FK;
                mySqlCommandInsertDetail.Parameters["@GreenLimit"].Value = myDetail.GreenLimit;
                mySqlCommandInsertDetail.Parameters["@YellowLimit"].Value = myDetail.YellowLimit;

                if (!CheckifExistRelativeFK(myDetail.FK))
                {
                    result = "Use a valid FK";
                    return result;
                }

                String sql = "INSERT INTO Details (Parameter, Description, Value, DateEntry, Note, FK, GreenLimit, YellowLimit )" +
                    "VALUES(@Parameter, @Description, @Value, @DateEntry, @Note, @FK, @GreenLimit, @YellowLimit)";


                mySqlCommandInsertDetail.CommandText = sql;
                mySqlCommandInsertDetail.ExecuteNonQuery();

                result = "Entered Correctly";

            }
            catch (Exception ex)
            {
                result = "" + ex.Message;
            }
            finally
            {
                mysqlConnection?.Close();

            }
            return result;
        }

        /* -------------------------------------------------------------------------------------------------------------------------------------------------------------------------- */

        [Route("ShowAllDetails")]
        [HttpGet]

        public responsiveDetails GetDetails()

        {
            responsiveDetails myListDetails = new responsiveDetails();
            SqlConnection? mySqlConnection = null;

            try
            {
                mySqlConnection = new SqlConnection(StringConnection);
                SqlCommand mySqlSelectAll = new SqlCommand();
                mySqlSelectAll.Connection = mySqlConnection;
                mySqlSelectAll.CommandText = "select * From Details WHERE Deleted=0 ";
                mySqlConnection.Open();
                SqlDataReader mySqlDataReader = mySqlSelectAll.ExecuteReader();

                while (mySqlDataReader.Read())
                {
                    Detail myDetail = new Detail();
                    myDetail.ID = mySqlDataReader.GetInt32(0);
                    myDetail.Parameter = mySqlDataReader.GetString(1);
                    myDetail.Description = mySqlDataReader.GetString(2);
                    myDetail.Value = (float)mySqlDataReader.GetSqlDouble(3);
                    myDetail.DateEntry = mySqlDataReader.GetDateTime(4);
                    myDetail.FK = mySqlDataReader.GetInt32(5);
                    myDetail.Note = mySqlDataReader.GetString(6);
                    myDetail.GreenLimit = (float)mySqlDataReader.GetSqlDouble(7);
                    myDetail.YellowLimit = (float)mySqlDataReader.GetSqlDouble(8);

                    myListDetails.Details.Add(myDetail);
                }

                myListDetails.Message = "Succesfull";
            }
            catch (Exception ex)
            {
                myListDetails.Message = ex.Message;
            }
            finally
            {
                mySqlConnection?.Close();
            }

            return myListDetails;

        }

        /* -------------------------------------------------------------------------------------------------------------------------------------------------------------------------- */


        [HttpDelete]
        [Route("DeleteDetails")]

        public string DeleteDetail(int IdDetail)
        {

            SqlConnection? mySqlConnection = null;
            string result = "";

            
            try
            {

                mySqlConnection = new SqlConnection(StringConnection);
                mySqlConnection.Open();
                SqlCommand mySqlCommandDelete = mySqlConnection.CreateCommand();

                mySqlCommandDelete.Parameters.Add("@ID", SqlDbType.Int);
                mySqlCommandDelete.Parameters["@ID"].Value = IdDetail;

                if (!CheckifDetailExist(IdDetail))
                {
                    result = "The id does not exist ";
                    return result;
                }


                string query = "UPDATE Details SET Deleted=1 WHERE ID=@ID";

                mySqlCommandDelete.CommandText = query;
                mySqlCommandDelete.ExecuteNonQuery();

                result = "Deleted correctly";

            }
            catch (Exception ex)
            {
                result = "Ops... " + ex.Message;
            }
            finally
            {
                mySqlConnection?.Close();
            }

            return result;

        }

        /* -------------------------------------------------------------------------------------------------------------------------------------------------------------------------- */

        [HttpPatch]
        [Route("ModifyDetail")]
        public string modifyParameter(Detail newDetail)
        {
            string result = "";
            SqlConnection? mySqlConnection = null;

            if (newDetail.Parameter == "" ||  newDetail.Description == "")
            {
                result = "You must put the correct key information";
                return result;
            }


            try
            {
                mySqlConnection = new SqlConnection(StringConnection);
                mySqlConnection.Open();
                SqlCommand myCommandUpdate = mySqlConnection.CreateCommand();
                myCommandUpdate.Parameters.Add("@ID", System.Data.SqlDbType.Int);
                myCommandUpdate.Parameters.Add("@newParameter", System.Data.SqlDbType.NVarChar);
                myCommandUpdate.Parameters.Add("@newDescription", System.Data.SqlDbType.NVarChar);
                myCommandUpdate.Parameters.Add("@newNotes", System.Data.SqlDbType.NVarChar);
                myCommandUpdate.Parameters.Add("@newGreenLimit", System.Data.SqlDbType.Float);
                myCommandUpdate.Parameters.Add("@newYellowLimit", System.Data.SqlDbType.Float);

                myCommandUpdate.Parameters["@ID"].Value = newDetail.ID;
                myCommandUpdate.Parameters["@newParameter"].Value = newDetail.Parameter;
                myCommandUpdate.Parameters["@newDescription"].Value = newDetail.Description;
                myCommandUpdate.Parameters["@newNotes"].Value = newDetail.Note;
                myCommandUpdate.Parameters["@newGreenLimit"].Value = newDetail.GreenLimit;
                myCommandUpdate.Parameters["@newYellowLimit"].Value = newDetail.YellowLimit;

                if (!CheckifDetailExist(newDetail.ID))
                {
                    result = "The id does not exist";
                    return result;
                }


                myCommandUpdate.CommandText = "UPDATE Details SET Parameter = @newParameter, Description = @newDescription, Note = @newNotes, GreenLimit=@newGreenLimit, YellowLimit=@newYellowLimit WHERE ID = @ID";

                myCommandUpdate.ExecuteNonQuery();
                result = "Updated successfully";
            }
            catch (Exception ex)
            {
                result = "Fail: " + ex.Message;
            }
            finally
            {
                mySqlConnection?.Close();
            }
            return result;
        }

        /* -------------------------------------------------------------------------------------------------------------------------------------------------------------------------- */

        [Route("ShowSelectedDetails")]
        [HttpGet]

        public responsiveDetails mySelectedItems(int IdComponent)
        {
            responsiveDetails myListDetails = new responsiveDetails();
            SqlConnection? mySqlConnection = null;

            try
            {
                mySqlConnection = new SqlConnection(StringConnection);
                SqlCommand mySqlSelectAll = new SqlCommand();
                mySqlSelectAll.Parameters.Add("@ID", SqlDbType.Int);
                mySqlSelectAll.Parameters["@ID"].Value = IdComponent;
                mySqlSelectAll.Connection = mySqlConnection;

                if (!CheckifComponentsIDExist(IdComponent))
                {
                   myListDetails.Message = "The id does not exist";
                   return myListDetails;
                }

                mySqlSelectAll.CommandText = "select Details.* From ComponentsDA INNER JOIN Details ON ComponentsDA.ID= Details.FK WHERE ComponentsDA.ID=@ID AND ComponentsDA.Deleted=0 AND Details.Deleted=0 ";
                mySqlConnection.Open();
                SqlDataReader mySqlDataReader = mySqlSelectAll.ExecuteReader();

                while (mySqlDataReader.Read())
                {
                    Detail myDetail = new Detail();
                    myDetail.ID = mySqlDataReader.GetInt32(0);
                    myDetail.Parameter = mySqlDataReader.GetString(1);
                    myDetail.Description = mySqlDataReader.GetString(2);
                    myDetail.Value = (float)mySqlDataReader.GetSqlDouble(3);
                    myDetail.DateEntry = mySqlDataReader.GetDateTime(4);
                    myDetail.FK = mySqlDataReader.GetInt32(5);
                    myDetail.Note = mySqlDataReader.GetString(6);
                    myDetail.GreenLimit = (float)mySqlDataReader.GetSqlDouble(7);
                    myDetail.YellowLimit = (float)mySqlDataReader.GetSqlDouble(8);

                    myListDetails.Details.Add(myDetail);
                }

                myListDetails.Message = "Succesfull";
            }
            catch (Exception ex)
            {
                myListDetails.Message = ex.Message;
            }
            finally
            {
                mySqlConnection?.Close();
            }

            return myListDetails;

        }
                                                                                                            
        /* -------------------------------------------------------------------------------------------------------------------------------------------------------------------------- */

        private bool CheckifDetailExist(int IdDetail)
        {
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
            catch (Exception)
            {
                return false;
            }
            finally
            {
                mySqlConnection?.Close();
            }
        }
        private bool CheckifExistRelativeFK(int FK)
        {
            SqlConnection? mySqlConnection = null;

            try
            {
                mySqlConnection = new SqlConnection(StringConnection);
                mySqlConnection.Open();
                SqlCommand mySqlCommandCheck = mySqlConnection.CreateCommand();

                mySqlCommandCheck.Parameters.Add("@FK", SqlDbType.Int);
                mySqlCommandCheck.Parameters["@FK"].Value = FK;

                string query = "SELECT ID FROM ComponentsDA WHERE ID=@FK AND Deleted=0 ";

                mySqlCommandCheck.CommandText = query;
                SqlDataReader mySqlReader = mySqlCommandCheck.ExecuteReader();

                if (mySqlReader.Read())
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                mySqlConnection?.Close();
            }
        }

        private bool CheckifComponentsIDExist(int IdComponent)
        {
            SqlConnection? mySqlConnection = null;

            try
            {
                mySqlConnection = new SqlConnection(StringConnection);
                mySqlConnection.Open();
                SqlCommand mySqlCommandCheck = mySqlConnection.CreateCommand();

                mySqlCommandCheck.Parameters.Add("@ID", SqlDbType.Int);
                mySqlCommandCheck.Parameters["@ID"].Value = IdComponent;

                string query = "SELECT ID FROM ComponentsDA WHERE ID=@ID AND Deleted=0";


                mySqlCommandCheck.CommandText = query;
                SqlDataReader mySqlReader = mySqlCommandCheck.ExecuteReader();

                if (mySqlReader.Read())
                {

                    return true;

                }

                return false;
            }
            catch (Exception)
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