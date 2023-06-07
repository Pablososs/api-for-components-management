using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using MySql.Data.MySqlClient;
using DAPI.Class;


namespace DAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class PositionsController : ControllerBase

    {
        private readonly string StringConnection = "Server = localhost\\SQLEXPRESS; Database= DB_DanieliAutomation; Trusted_connection=true; ";

        /* -------------------------------------------------------------------------------------------------------------------------------------------------------------------------- */

        [Route("ShowAllPositions")]
        [HttpGet]

        public List<responsivePosition> GetPosition()
        {

            List<responsivePosition> myListPositions = new List<responsivePosition>();
            SqlConnection? mySqlConnection = null;


            try
            {
                mySqlConnection = new SqlConnection(StringConnection);
                SqlCommand mySqlCommanSelectAll = mySqlConnection.CreateCommand();
                mySqlCommanSelectAll.Connection = mySqlConnection;
                mySqlCommanSelectAll.CommandText = "SELECT * FROM ComponentsDA LEFT JOIN Positions ON ComponentsDA.ID = Positions.FK LEFT JOIN Details ON ComponentsDA.ID = Details.FK WHERE (ComponentsDA.Deleted IS NULL OR ComponentsDA.Deleted = 0) AND (Details.Deleted IS NULL OR Details.Deleted = 0) AND (Positions.Deleted IS NULL OR Positions.Deleted = 0) ";

                mySqlConnection.Open();



                SqlDataReader mySqlRead = mySqlCommanSelectAll.ExecuteReader();


                // ComponentsController componentsController = new ComponentsController();

                int i = 0;


                while (mySqlRead.Read())
                {
                    Detail ausDetail = new Detail();
                    responsivePosition ausResponse = new responsivePosition();

                    ausResponse.MyComponent.ID = mySqlRead.GetInt32(0);
                    ausResponse.MyComponent.Device = mySqlRead.GetString(1);
                    ausResponse.MyComponent.Task = mySqlRead.GetString(2);
                    ausResponse.MyComponent.ProductItem = mySqlRead.GetString(4);
                    ausResponse.MyComponent.Note = mySqlRead.GetString(5);

                    if (!mySqlRead.IsDBNull(8))
                    {
                        ausResponse.Position.ID = (mySqlRead.GetInt32(8));
                        ausResponse.Position.xPosition = mySqlRead.GetInt32(9);
                        ausResponse.Position.yPosition = mySqlRead.GetInt32(10);
                        ausResponse.Position.FK = mySqlRead.GetInt32(11);
                    }

                    if (!mySqlRead.IsDBNull(13))
                    {
                        ausDetail.ID = mySqlRead.GetInt32(13);
                        ausDetail.Parameter = mySqlRead.GetString(14);
                        ausDetail.Description = mySqlRead.GetString(15);
                        ausDetail.Value = (float)mySqlRead.GetSqlDouble(16);
                        ausDetail.FK = mySqlRead.GetInt32(18);
                        ausDetail.Note = mySqlRead.GetString(19);
                        ausDetail.GreenLimit = (float)mySqlRead.GetSqlDouble(20);
                        ausDetail.YellowLimit = (float)mySqlRead.GetSqlDouble(21);

                        ausResponse.Details.Add(ausDetail);
                    }


                    if (i != 0)
                    {
                        if (myListPositions[i - 1].MyComponent.ID == ausResponse.MyComponent.ID)
                        {
                            myListPositions[i - 1].Details.Add(ausDetail);
                        }
                        else
                        {
                            myListPositions.Add(ausResponse);
                            i++;
                        }

                    }
                    else
                    {
                        myListPositions.Add(ausResponse);
                        i++;
                    }


                }


            }

            catch (Exception)
            {

            }
            finally
            {
                mySqlConnection?.Close();

            }

            return myListPositions;

        }

        /* -------------------------------------------------------------------------------------------------------------------------------------------------------------------------- */


        [Route("InsertNewPosition")]
        [HttpPost]

        public string PostPosition(Position myPosition)
        {
            SqlConnection? mySqlConnection = null;

            string result = "";

            if (myPosition.xPosition.Equals(null) || myPosition.yPosition.Equals(null))
            {
                result = "You must put the key information";
                return result;
            }

            try
            {

                mySqlConnection = new SqlConnection(StringConnection);
                SqlCommand mySqlCommandInsertPosition = new SqlCommand();
                mySqlConnection.Open();
                mySqlCommandInsertPosition.Connection = mySqlConnection;

                mySqlCommandInsertPosition.Parameters.Add("@xPosition", SqlDbType.Float);
                mySqlCommandInsertPosition.Parameters.Add("@yPosition", SqlDbType.Float);
                mySqlCommandInsertPosition.Parameters.Add("@FK", SqlDbType.Int);

                mySqlCommandInsertPosition.Parameters["@xPosition"].Value = myPosition.xPosition;
                mySqlCommandInsertPosition.Parameters["@yPosition"].Value = myPosition.yPosition;
                mySqlCommandInsertPosition.Parameters["@FK"].Value = myPosition.FK;

                if (!CheckifExistRelativeFK(myPosition.FK)) {

                    result = "Use a valid FK";
                    return result;
                }

                if (!CheckifAlreadyExist(myPosition.FK))
                {
                    result = "This Component already has a position";
                    return result;  
                }



                String sql = "INSERT INTO Positions(xPosition, yPosition, FK) VALUES (@xPosition, @yPosition,@FK)";


                mySqlCommandInsertPosition.CommandText = sql;
                mySqlCommandInsertPosition.ExecuteNonQuery();
                result = "Success";

            }

            catch (Exception ex)
            {

                result = "" + ex.Message;

            }
            finally
            {
                mySqlConnection?.Close();

            }
            return result;

        }

        /* -------------------------------------------------------------------------------------------------------------------------------------------------------------------------- */

        [HttpDelete]
        [Route("DeletePosition")]

        public string DeletePosition(int IdPosition)
        {
            SqlConnection? mySqlConnection = null;
            string result = "";


            try
            {


                mySqlConnection = new SqlConnection(StringConnection);
                mySqlConnection.Open();
                SqlCommand mySqlCommandDelete = mySqlConnection.CreateCommand();

                mySqlCommandDelete.Parameters.Add("@ID", SqlDbType.Int);
                mySqlCommandDelete.Parameters["@ID"].Value = IdPosition;

                if (!CheckifPositionExist(IdPosition))
                {
                    result = "The id does not exist ";
                    return result;
                }


                string query = "UPDATE Positions SET Deleted=1 WHERE ID=@ID";

                mySqlCommandDelete.CommandText = query;
                mySqlCommandDelete.ExecuteNonQuery();

                result = "Success";
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
        [Route("ModifyPosition")]

        public string PatchPosition(Position newPosition)
        {

            SqlConnection? mySqlConnection = null;
            string result = "";

            try
            {

                mySqlConnection = new SqlConnection(StringConnection);
                mySqlConnection.Open();

                SqlCommand myCommandUpdate = mySqlConnection.CreateCommand();
                myCommandUpdate.Parameters.Add("@ID", System.Data.SqlDbType.Int);
                myCommandUpdate.Parameters.Add("@xPosition", System.Data.SqlDbType.Int);
                myCommandUpdate.Parameters.Add("@yPosition", System.Data.SqlDbType.Int);

                myCommandUpdate.Parameters["@ID"].Value = newPosition.ID;
                myCommandUpdate.Parameters["@xPosition"].Value = newPosition.xPosition;
                myCommandUpdate.Parameters["@yPosition"].Value = newPosition.yPosition;

                if (!CheckifPositionExist(newPosition.ID))
                {
                    result = "The id does not exist";
                    return result;
                }


                myCommandUpdate.CommandText = "UPDATE Positions SET xPosition=@xPosition, yPosition=@yPosition WHERE ID=@ID  ";

                myCommandUpdate.ExecuteNonQuery();

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

        [HttpGet]
        [Route("getComponentsWithoutPosition")]

        public IEnumerable<Component> myComponents()
        {
            List<Component>? mylistComponents = new List<Component>();
            SqlConnection? mySqlConnection = null;
            string result = "";

            try
            {
                mySqlConnection = new SqlConnection(StringConnection);
                SqlCommand mySqlSelect = mySqlConnection.CreateCommand();
                mySqlSelect.Connection = mySqlConnection;
                mySqlSelect.CommandText = "SELECT * FROM ( SELECT ComponentsDA.ID, ComponentsDA.Device, ComponentsDA.Task, ComponentsDA.ProductItem, ComponentsDA.Note, ComponentsDA.PathImage, MIN(CAST(Positions.Deleted AS INT)) AS haventAPosition   FROM ComponentsDA    LEFT JOIN Positions ON ComponentsDA.ID = Positions.FK WHERE ComponentsDA.Deleted = 0 GROUP BY ComponentsDA.ID, ComponentsDA.Device, ComponentsDA.Task, ComponentsDA.ProductItem, ComponentsDA.Note, ComponentsDA.PathImage, Positions.FK) DbVirtual WHERE DbVirtual.haventAPosition = 1 OR DbVirtual.haventAPosition IS NULL";
                mySqlConnection.Open();
                SqlDataReader mySqlDataReader = mySqlSelect.ExecuteReader();




                while (mySqlDataReader.Read()) {
                    Component myComponent = new Component();

                    myComponent.ID = mySqlDataReader.GetInt32(0);
                    myComponent.Device = mySqlDataReader.GetString(1);
                    myComponent.Task = mySqlDataReader.GetString(2);
                    myComponent.ProductItem = mySqlDataReader.GetString(3);
                    myComponent.Note = mySqlDataReader.GetString(4);
                    myComponent.PathImage = mySqlDataReader.GetString(5);

                    mylistComponents.Add(myComponent);
                }

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

            return mylistComponents;
        }

        /* -------------------------------------------------------------------------------------------------------------------------------------------------------------------------- */

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

                string query = "SELECT ID FROM ComponentsDA WHERE ID=@FK";

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

        /* -------------------------------------------------------------------------------------------------------------------------------------------------------------------------- */

        private bool CheckifPositionExist(int IdPosition)
        {
            SqlConnection? mySqlConnection = null;

            try
            {
                mySqlConnection = new SqlConnection(StringConnection);
                mySqlConnection.Open();
                SqlCommand mySqlCommandCheck = mySqlConnection.CreateCommand();

                mySqlCommandCheck.Parameters.Add("@ID", SqlDbType.Int);
                mySqlCommandCheck.Parameters["@ID"].Value = IdPosition;

                string query = "SELECT ID FROM Positions WHERE ID=@ID AND Deleted=0 ";

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

        /* -------------------------------------------------------------------------------------------------------------------------------------------------------------------------- */

        private bool CheckifAlreadyExist(int FkPosition)
        {
            SqlConnection? mySqlConnection = null;

            try
            {
                mySqlConnection = new SqlConnection(StringConnection);
                mySqlConnection.Open();
                SqlCommand mySqlCommandCheck = mySqlConnection.CreateCommand();

                string query = "SELECT FK FROM Positions WHERE FK=" + Convert.ToString(FkPosition) + " AND Deleted=0";

                mySqlCommandCheck.CommandText = query;
                SqlDataReader mySqlReader = mySqlCommandCheck.ExecuteReader();

                if (mySqlReader.Read()) {

                    return false;
                
                }

                return true; 
            }
            catch(Exception) 
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