using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Specialized;
using DAPI.Class;

namespace DAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ComponentsController : ControllerBase
    {
        private readonly string StringConnection = "Server = localhost\\SQLEXPRESS; Database= DB_DanieliAutomation; Trusted_connection=true; ";

        /* -------------------------------------------------------------------------------------------------------------------------------------------------------------------------- */

        [Route("InsertNewComponent")]
        [HttpPost]
        public string PostComponents(Component myComponent)
        {
            SqlConnection? mysqlConnection = null;

            string result = "";

            if (myComponent.Device == "" || myComponent.Task == "" || myComponent.ProductItem == "")
            {

                result = "You must put the key information";
                return result;

            }



            try
            {

                mysqlConnection = new SqlConnection(StringConnection);
                SqlCommand mySqlCommandInsertComponent = new SqlCommand();
                mysqlConnection.Open();
                mySqlCommandInsertComponent.Connection = mysqlConnection;

                mySqlCommandInsertComponent.Parameters.Add("@Device", SqlDbType.NVarChar);
                mySqlCommandInsertComponent.Parameters["@Device"].Value = myComponent.Device;

                mySqlCommandInsertComponent.Parameters.Add("@Task", SqlDbType.NVarChar);
                mySqlCommandInsertComponent.Parameters["@Task"].Value = myComponent.Task;

                mySqlCommandInsertComponent.Parameters.Add("@DateEntry", SqlDbType.Date);
                mySqlCommandInsertComponent.Parameters["@DateEntry"].Value = myComponent.DateEntry;

                mySqlCommandInsertComponent.Parameters.Add("@ProductItem", SqlDbType.NVarChar);
                mySqlCommandInsertComponent.Parameters["@ProductItem"].Value = myComponent.ProductItem;

                mySqlCommandInsertComponent.Parameters.Add("@Note", SqlDbType.NVarChar);
                mySqlCommandInsertComponent.Parameters["@Note"].Value = myComponent.Note;

                mySqlCommandInsertComponent.Parameters.Add("@PathImage", SqlDbType.NVarChar);
                mySqlCommandInsertComponent.Parameters["@PathImage"].Value = myComponent.PathImage;

                String sql = "INSERT INTO ComponentsDA (Device, Task, DateEntry, ProductItem, Note, PathImage )" +
                    "VALUES(@Device, @Task, @DateEntry, @ProductItem, @Note, @PathImage)";

                mySqlCommandInsertComponent.CommandText = sql;
                mySqlCommandInsertComponent.ExecuteNonQuery();

                result = "Inserito";

            }
            catch (Exception ex)
            {
                result = ex.Message;    
            }
            finally
            {
                mysqlConnection?.Close();

            }

            return result;

        }

        /* -------------------------------------------------------------------------------------------------------------------------------------------------------------------------- */


        [Route("ShowAllComponents")]
        [HttpGet]

        public responsiveComponents GetComponets()

        {
            responsiveComponents myListComponents = new responsiveComponents();
            SqlConnection? mySqlConnection = null;

            try
            {
                mySqlConnection = new SqlConnection(StringConnection);
                SqlCommand mySqlSelectAll = new SqlCommand();
                mySqlSelectAll.Connection = mySqlConnection;
                mySqlSelectAll.CommandText = "select * From ComponentsDA WHERE Deleted=0 ";
                mySqlConnection.Open();
                SqlDataReader mySqlDataReader = mySqlSelectAll.ExecuteReader();


                while (mySqlDataReader.Read())
                {
                    Component myComponent = new Component();
                    myComponent.ID = mySqlDataReader.GetInt32(0);
                    myComponent.Device = mySqlDataReader.GetString(1);
                    myComponent.Task = mySqlDataReader.GetString(2);
                    myComponent.DateEntry = mySqlDataReader.GetDateTime(3);
                    myComponent.ProductItem = mySqlDataReader.GetString(4);
                    myComponent.Note = mySqlDataReader.GetString(5);
                    myComponent.PathImage = mySqlDataReader.GetString(6);

                    myListComponents.Components.Add(myComponent);
                }

                myListComponents.Message = "Success";
            }
            catch (Exception ex)
            {
                myListComponents.Message = "Ops..." + ex.Message;
            }
            finally
            {
                mySqlConnection?.Close();
            }

            return myListComponents;

        }

        /* -------------------------------------------------------------------------------------------------------------------------------------------------------------------------- */


        [HttpDelete]
        [Route("DeleteComponents")]
        public string DeleteComponent(int IdComponent)
        {

            SqlConnection? mySqlConnection = null;
            string result = "";


            try
            {
               
                mySqlConnection = new SqlConnection(StringConnection);
                mySqlConnection.Open();
                SqlCommand mySqlCommandDelete = mySqlConnection.CreateCommand();

                mySqlCommandDelete.Parameters.Add("@ID", SqlDbType.Int);
                mySqlCommandDelete.Parameters["@ID"].Value = IdComponent;

                if (!CheckifExistID(IdComponent))
                {
                    result = "The id does not exist";
                    return result;
                }

                string query = "UPDATE ComponentsDA SET Deleted=1 WHERE ID=@ID";
                mySqlCommandDelete.CommandText = query;
                mySqlCommandDelete.ExecuteNonQuery();

                string query2 = "UPDATE Details SET Deleted=1 WHERE FK=@ID";
                mySqlCommandDelete.CommandText = query2;
                mySqlCommandDelete.ExecuteNonQuery();

                string query3 = "UPDATE Positions SET Deleted=1 WHERE FK=@ID";
                mySqlCommandDelete.CommandText = query3;
                mySqlCommandDelete.ExecuteNonQuery();

                result = "Success!";
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
        [Route("ModifyComponent")]
        public string ModifyComponent(Component newComponent)
        {
            SqlConnection? mySqlConnection = null;
            string result = "";

            if (newComponent.Device == "" || newComponent.Task == "" || newComponent.ProductItem == "")
            {
                result = "You must put the key information";
                return result;
            }

            try
            {
                mySqlConnection = new SqlConnection(StringConnection);
                mySqlConnection.Open(); SqlCommand myComm = mySqlConnection.CreateCommand();

                myComm.Parameters.Add("@ID", System.Data.SqlDbType.Int);
                myComm.Parameters.Add("@newDevice", System.Data.SqlDbType.NVarChar);
                myComm.Parameters.Add("@newTask", System.Data.SqlDbType.NVarChar);
                myComm.Parameters.Add("@newProductItem", System.Data.SqlDbType.NVarChar);
                myComm.Parameters.Add("@newNotes", System.Data.SqlDbType.NVarChar);
                myComm.Parameters.Add("@newPathImage", System.Data.SqlDbType.NVarChar);

                myComm.Parameters["@ID"].Value = newComponent.ID;
                myComm.Parameters["@newDevice"].Value = newComponent.Device;
                myComm.Parameters["@newTask"].Value = newComponent.Task;
                myComm.Parameters["@newProductItem"].Value = newComponent.ProductItem;
                myComm.Parameters["@newNotes"].Value = newComponent.Note;
                myComm.Parameters["@newPathImage"].Value = newComponent.PathImage;


                if (!CheckifExistID(newComponent.ID))
                {
                    result = "The id does not exist";
                    return result;
                }

              
                myComm.CommandText = "UPDATE ComponentsDA SET Device = @newDevice, Task = @newTask, ProductItem = @newProductItem, PathImage=@newPathImage, Note = @newNotes WHERE ID = @ID";

                myComm.ExecuteNonQuery();
                result = "Success";
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

        [Route("ShowSelectedComponents")]
        [HttpGet]


        public responsiveComponents myselectedcomponents (int IdComponent)
        {
            SqlConnection? mySqlConnection = null;
            responsiveComponents mylistComponents = new responsiveComponents();

            try
            {

                mySqlConnection = new SqlConnection(StringConnection);
                SqlCommand mySqlSelectAll = new SqlCommand();
                mySqlSelectAll.Parameters.Add("@ID", SqlDbType.Int);
                mySqlSelectAll.Parameters["@ID"].Value = IdComponent;
                mySqlSelectAll.Connection = mySqlConnection;

                if (!CheckifExistID(IdComponent))
                {
                    mylistComponents.Message = "The id does not exist";
                    return mylistComponents;

                }

                mySqlSelectAll.CommandText = "SELECT * FROM ComponentsDA WHERE ID=@ID AND Deleted=0 ";
                mySqlConnection.Open();
                SqlDataReader mySqlDataReader = mySqlSelectAll.ExecuteReader();

                while (mySqlDataReader.Read())
                {
                    Component myComponent= new Component(); 
                    myComponent.ID = mySqlDataReader.GetInt32(0);
                    myComponent.Device = mySqlDataReader.GetString(1);
                    myComponent.Task = mySqlDataReader.GetString(2);
                    myComponent.DateEntry = mySqlDataReader.GetDateTime(3);
                    myComponent.ProductItem = mySqlDataReader.GetString(4);
                    myComponent.Note = mySqlDataReader.GetString(5);
                    myComponent.PathImage = mySqlDataReader.GetString(6);

                    mylistComponents.Components.Add(myComponent);   

                }

                mylistComponents.Message = "Succesfull";
            }
            catch (Exception ex)
            {
                mylistComponents.Message= ex.Message;
            }
            finally
            {
                mySqlConnection?.Close();
            }

            return mylistComponents;
        }

        /* -------------------------------------------------------------------------------------------------------------------------------------------------------------------------- */

        private bool CheckifExistID(int IdComponent)
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

                if (mySqlReader.Read()) {

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

            //private bool CheckifExistRelativeFK(int FK)
            //{
            //    SqlConnection mySqlConnection = null;

            //    try
            //    {
            //        mySqlConnection = new SqlConnection(StringConnection);
            //        mySqlConnection.Open();
            //        SqlCommand mySqlCommandCheck = mySqlConnection.CreateCommand();

            //        mySqlCommandCheck.Parameters.Add("@FK", SqlDbType.Int);
            //        mySqlCommandCheck.Parameters["@FK"].Value = FK;

            //        string query = "SELECT ID FROM ComponentsDA WHERE ID=@FK";

            //        mySqlCommandCheck.CommandText = query;
            //        mySqlCommandCheck.ExecuteNonQuery();


            //        return true;
            //    }
            //    catch (Exception ex)
            //    {
            //        return false;
            //    }
            //    finally
            //    {
            //        mySqlConnection.Close();
            //    }             
    }
}


