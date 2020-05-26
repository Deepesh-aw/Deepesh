using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;

namespace DeepeshWeb.DAL
{
    public class AuthenticateUser
    {
        public List<object> CheckUser(string DomainName)
        {
            List<object> obj = new List<object>();
            try
            {
                var connection = ConfigurationManager.ConnectionStrings["DBContext"].ConnectionString;

                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand("SP_CheckDomain", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@DomainName", SqlDbType.VarChar).Value = DomainName;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        obj.Add("Authanticated");
                    }
                }
                else
                {
                    obj.Add("Error");
                }
                con.Close();
            }
            catch (Exception ex)
            {
                //DataAccessExceptionHandler.HandleException(ref ex);
            }

            return obj;

        }

    }
}