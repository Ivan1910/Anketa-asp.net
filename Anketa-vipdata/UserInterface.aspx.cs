using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using Anketa_vipdata;
using System.Web.UI.WebControls;
using System.Web;

public partial class User : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Korisnik\source\repos\Anketa-vipdata\Anketa-vipdata\App_Data\AnketaDB.mdf;Integrated Security=True;MultipleActiveResultSets=true;";
            string query = "SELECT DISTINCT anketaID FROM Odgovori";
            List<int> anketeKojeImajuOdgovore = new List<int>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    anketeKojeImajuOdgovore.Add(reader.GetInt32(0));
                }
            }
            query = "SELECT anketaID FROM Anketa WHERE GETDATE() < aktivnaDo AND GETDATE() >= aktivnaOd";
            List<int> anketaID = new List<int>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                int id;
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                try
                {
                    while (reader.Read())
                    {
                        id = reader.GetInt32(0);
                        if (anketeKojeImajuOdgovore.Contains(id)) { anketaID.Add(id); }
                    }
                    int randomID;
                    while (true)
                    {
                        if (anketaID.Count() == 0)
                        {
                            notifyLabel.Text = "Žao nam je ali trenutno nema anketa za prikaz !";
                            notifyLabel.Visible = true;
                            voteButton.Visible = false;
                            return;
                        }
                        else
                        {
                            randomID = anketaID[ChooseRandomIndex(anketaID.Count())];
                            break;
                        }
                    }
                    query = "SELECT naslovAnkete,anketaID,aktivnaDo,aktivnaOd FROM Anketa WHERE anketaID = " + randomID;
                    command = new SqlCommand(query, connection);
                    reader = command.ExecuteReader();
                    reader.Read();
                    Anketa anketa = new Anketa(reader.GetString(0), reader.GetInt32(1), reader.GetDateTime(2), reader.GetDateTime(3));
                    query = "SELECT tekstOdgovora,brojGlasova FROM Anketa INNER JOIN Odgovori ON Anketa.anketaID = Odgovori.anketaID WHERE Odgovori.anketaID = " + randomID;
                    command = new SqlCommand(query, connection);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (anketa != null)
                        {
                            anketa.listaOdgovora.Add(new KeyValuePair<string, int>(reader.GetString(0), reader.GetInt32(1)));
                        }
                    }
                    naslovAnkete.Text = anketa.naslovAnkete;
                    foreach (var i in anketa.listaOdgovora)
                    {
                        RadioButtonList1.Items.Add(i.Key.ToString());
                    }
                    Session["indexAnkete"] = anketa.anketaID;
                }
                catch (SqlException)
                {
                    HideElementsAndShowNotifyLabel("Žao nam je ali došlo je do pogreške s bazom podataka ! Molimo pokušajte kasnije !");
                }
                catch (Exception)
                {
                    HideElementsAndShowNotifyLabel("Ups, nepoznata greška ! Molimo pokušajte kasnije !");
                }
                finally
                {
                    reader.Close();
                }
            }

        }
    }

    private int ChooseRandomIndex(int count)
    {
        var rand = new Random();
        return rand.Next(count);
    }

    private void HideElementsAndShowNotifyLabel(string s)
    {
        RadioButtonList1.Visible = false;
        voteButton.Visible = false;
        naslovAnkete.Visible = false;
        notifyLabel.Text = s;
        notifyLabel.Visible = true;
    }

    protected void VoteButton_Click(object sender, EventArgs e)
    {
        if (Session["indexAnkete"] != null && RadioButtonList1.SelectedItem != null)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Korisnik\source\repos\Anketa-vipdata\Anketa-vipdata\App_Data\AnketaDB.mdf;Integrated Security=True;MultipleActiveResultSets=true;";
            string query = "UPDATE Odgovori SET brojGlasova = brojGlasova+1 WHERE anketaID = " + Session["indexAnkete"] + " AND tekstOdgovora = '" + RadioButtonList1.SelectedValue.ToString() + "'";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (SqlException)
                {
                    HideElementsAndShowNotifyLabel("Žao nam je ali došlo je do pogreške s bazom podataka ! Molimo pokušajte kasnije !");

                }
                catch (Exception)
                {
                    HideElementsAndShowNotifyLabel("Ups, nepoznata greška ! Molimo pokušajte kasnije !");
                }
            }
            RadioButtonList1.Visible = false;
            voteButton.Visible = false;
            naslovAnkete.Visible = false;
            notifyLabel.Visible = true;
        }
        else
        {
            Label label = new Label();
            label.Text = "Odaberite jedan od odgovora prije slanja !";
            label.ForeColor = System.Drawing.Color.Yellow;
            label.Font.Size = 10;
            Anketa.Controls.Add(label);
        }
    }
}