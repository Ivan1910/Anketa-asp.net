using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Drawing;
using System.Data;
using System.Web;

public partial class Admin : System.Web.UI.Page
{
    protected override void OnInit(EventArgs e)
    {
        Popis();
    }
    private void Popis()
    {
        pregledPojedinacneAnkete.Visible = false;

        string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Korisnik\source\repos\Anketa-vipdata\Anketa-vipdata\App_Data\AnketaDB.mdf;Integrated Security=True;MultipleActiveResultSets=true;";
        string query;

        Button dodajAnketu = new Button();
        dodajAnketu.ID = "dodajanketu";
        dodajAnketu.Text = "Dodaj novu anketu";

        dodajButton.Controls.Add(dodajAnketu);


        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            List<int> anketeKojeImajuOdgovor = new List<int>();
            query = "SELECT DISTINCT anketaID FROM Odgovori";
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                anketeKojeImajuOdgovor.Add(reader.GetInt32(0));
            }
            query = "SELECT naslovAnkete,anketaID FROM Anketa";
            command = new SqlCommand(query, connection);
            reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                int count = 1;
                int id;
                string naslovAnkete;
                while (reader.Read())
                {
                    naslovAnkete = reader.GetString(0);
                    id = reader.GetInt32(1);
                    Button btn = new Button();
                    btn.Text = count + ". " + naslovAnkete;
                    btn.ID = "btn_" + id;
                    btn.Click += new EventHandler(anketaClick);
                    btn.BackColor = Color.Transparent;
                    btn.BorderColor = Color.Transparent;
                    if (anketeKojeImajuOdgovor.Contains(id))
                    {
                        btn.ForeColor = Color.White;
                    }
                    else
                    {
                        btn.ForeColor = Color.Yellow;
                    }
                    btn.CssClass = "buttonListaAnketa";
                    listaAnketa.Controls.Add(btn);
                    PlaceHolder p = new PlaceHolder();
                    p.Controls.Add(new LiteralControl("<br />"));
                    listaAnketa.Controls.Add(p);
                    ++count;
                }
                reader.Close();
            }
            else
            {
                notifyLabel.Text = "Nije pronađena niti jedna anketa !";
                notifyLabel.Visible = true;
            }

        }
    }
    private void anketaClick(object sender, EventArgs e)
    {

        listaAnketa.Visible = false;
        Button btn = (Button)sender;
        int recordNum;
        string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Korisnik\source\repos\Anketa-vipdata\Anketa-vipdata\App_Data\AnketaDB.mdf;Integrated Security=True;MultipleActiveResultSets=true;";
        string query = "SELECT count(odgovorID) as brojOdgovora FROM Odgovori WHERE anketaID = " + int.Parse(btn.ID.Substring(btn.ID.IndexOf('_') + 1));
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();
            recordNum = reader.GetInt32(0);
            reader.Close();
        }

        Anketa_vipdata.Anketa anketa;

        if (recordNum > 0)
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                query = "SELECT naslovAnkete,anketaID,aktivnaDo,aktivnaOD FROM Anketa WHERE anketaID = " + int.Parse(btn.ID.Substring(btn.ID.IndexOf('_') + 1));
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                anketa = new Anketa_vipdata.Anketa(reader.GetString(0), reader.GetInt32(1), reader.GetDateTime(2), reader.GetDateTime(3));

                query = "SELECT tekstOdgovora,brojGlasova FROM Odgovori WHERE anketaID = " + anketa.anketaID;
                command = new SqlCommand(query, connection);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    anketa.listaOdgovora.Add(new KeyValuePair<string, int>(reader.GetString(0), reader.GetInt32(1)));
                }

                reader.Close();
            }
            Label naslovAnkete = new Label();
            naslovAnkete.Text = "Anketa: " + anketa.naslovAnkete;
            naslovAnkete.Font.Size = 15;
            Label aktivnost = new Label();
            aktivnost.Text = "Aktivna: " + anketa.aktivnaOd.ToLongDateString() + " - " + anketa.aktivnaDo.ToLongDateString();

            naslovAnkete.ForeColor = Color.White;
            aktivnost.ForeColor = Color.White;

            pregledPojedinacneAnkete.Controls.Add(naslovAnkete);
            pregledPojedinacneAnkete.Controls.Add(new LiteralControl("<br /> <br />"));
            pregledPojedinacneAnkete.Controls.Add(aktivnost);
            pregledPojedinacneAnkete.Controls.Add(new LiteralControl("<br /> <br />"));

            Label ankete = new Label();
            ankete.Text = "Ponuđeni odgovori:";
            ankete.ForeColor = Color.White;
            pregledPojedinacneAnkete.Controls.Add(ankete);

            Button naPopis = new Button();
            naPopis.Text = "Natrag na popis anketa";
            naPopis.ID = "btn_napopis";

            BulletedList bList = new BulletedList();
            foreach (var i in anketa.listaOdgovora)
            {
                bList.Items.Add(new ListItem(i.Key + " --- " + "glasovi: " + i.Value));

            }
            pregledPojedinacneAnkete.Controls.Add(bList);
            bList.ForeColor = Color.White;
            Label inform = new Label();
            inform.ForeColor = Color.Red;
            inform.Text = "Ovu anketu nije moguće modificirati jer postoje odgovori na nju !";
            pregledPojedinacneAnkete.Controls.Add(new LiteralControl("<br />"));
            pregledPojedinacneAnkete.Controls.Add(inform);
            pregledPojedinacneAnkete.Controls.Add(new LiteralControl("<br /> <br />"));
            pregledPojedinacneAnkete.Controls.Add(naPopis);
            pregledPojedinacneAnkete.Visible = true;
        }
        else
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                query = "SELECT naslovAnkete,anketaID,aktivnaDo,aktivnaOD FROM Anketa WHERE anketaID = " + int.Parse(btn.ID.Substring(btn.ID.IndexOf('_') + 1));
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                anketa = new Anketa_vipdata.Anketa(reader.GetString(0), reader.GetInt32(1), reader.GetDateTime(2), reader.GetDateTime(3));
            }
            dodajUSession(anketa);
            PrikazEditabilneAnkete();
        }
    }


    private void dodajUSession(Anketa_vipdata.Anketa a)
    {
        Session["aID"] = a.anketaID;
        Session["naslov"] = a.naslovAnkete;
        Session["aktivnaOd"] = a.aktivnaOd;
        Session["aktivnaDo"] = a.aktivnaDo;
    }

    private void Page_Load()
    {
        if (IsPostBack)
        {
            Control control = null;

            string ctrlname = Page.Request.Params["__EVENTTARGET"];
            if (ctrlname != null && ctrlname != String.Empty)
            {
                control = Page.FindControl(ctrlname);
            }
            else
            {
                string ctrlStr = String.Empty;
                foreach (string ctl in Page.Request.Form)
                {
                    if (ctl.EndsWith(".x") || ctl.EndsWith(".y"))
                    {
                        ctrlStr = ctl.Substring(0, ctl.Length - 2);
                        if (ctrlStr == "imgbtnOd" || ctrlStr == "imgbtnDo")
                        {
                            PrikazEditabilneAnkete();
                            break;
                        }
                    }
                    else
                    {
                        if (ctl == "btn_dodaj" || ctl == "btn_izbrisi" || ctl == "btn_spremi")
                        {
                            PrikazEditabilneAnkete();
                            break;
                        }
                        if (ctl == "btn_dodaj1" || ctl == "btn_izbrisi1" || ctl == "btn_spremi1")
                        {
                            DodajAnketu(2);
                            break;
                        }
                        if (ctl == "btn_napopis")
                        {
                            PrikaziPopis();
                            break;
                        }
                        if (ctl == "dodajanketu")
                        {
                            listaAnketa.Visible = false;
                            DodajAnketu(2);
                            break;
                        }
                    }
                }

            }
        }
    }

    private void PrikaziPopis()
    {
        pregledPojedinacneAnkete.Visible = false;
        listaAnketa.Visible = true;
    }

    private void PrikazEditabilneAnkete()
    {
        DodajAnketu(1);
        pregledPojedinacneAnkete.Visible = true;
    }

    private void SpremiAnketuClick(object sender, EventArgs e)
    {
        Control c = FindControl("lb");
        ListBox listB = (ListBox)c;
        c = FindControl("naslovtb");
        TextBox tbNaslov = (TextBox)c;
        c = FindControl("aktivnaodtb");
        TextBox aktivnaOd = (TextBox)c;
        c = FindControl("aktivnadotb");
        TextBox aktivnaDo = (TextBox)c;
        c = FindControl("notify");
        Label notLb = (Label)c;
        DateTime aOd;
        DateTime aDo;

        if (tbNaslov.Text == "" || aktivnaOd.Text == "" || aktivnaDo.Text == "" || listB.Items.Count < 2 || !DateTime.TryParse(aktivnaOd.Text, out aOd) || !DateTime.TryParse(aktivnaOd.Text, out aDo))
        {
            notLb.Text = "Niti jedno polje označeno zvjezdicom ne smije ostati prazno, anketa mora imati minimalno dva dodana odgovora i mora biti ispravan format datuma !";
        }
        else
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Korisnik\source\repos\Anketa-vipdata\Anketa-vipdata\App_Data\AnketaDB.mdf;Integrated Security=True;MultipleActiveResultSets=true;";
            string query;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                if (tbNaslov.Text != Session["naslov"].ToString())
                {
                    query = "UPDATE Anketa SET naslovAnkete = @naslov WHERE anketaID = @id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.Add("@id", SqlDbType.Int);
                    command.Parameters["@id"].Value = Session["aID"];
                    command.Parameters.Add("@naslov", SqlDbType.VarChar);
                    command.Parameters["@naslov"].Value = tbNaslov.Text;

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                if (aktivnaOd.Text != Session["aktivnaOd"].ToString())
                {
                    query = "UPDATE Anketa SET aktivnaOd = @aktivnaOd WHERE anketaID = @id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.Add("@id", SqlDbType.Int);
                    command.Parameters["@id"].Value = Session["aID"];
                    command.Parameters.Add("@aktivnaOd", SqlDbType.DateTime);
                    command.Parameters["@aktivnaOd"].Value = aOd;

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                if (aktivnaDo.Text != Session["aktivnaDo"].ToString())
                {
                    query = "UPDATE Anketa SET aktivnaDo = @aktivnaDo WHERE anketaID = @id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.Add("@id", SqlDbType.Int);
                    command.Parameters["@id"].Value = Session["aID"];
                    command.Parameters.Add("@aktivnaDo", SqlDbType.DateTime);
                    command.Parameters["@aktivnaDo"].Value = aDo;

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                connection.Open();
                foreach (var item in listB.Items)
                {
                    query = "INSERT INTO Odgovori(anketaID,tekstOdgovora,brojGlasova) VALUES (@id,@odgovor,0)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.Add("@id", SqlDbType.Int);
                    command.Parameters["@id"].Value = Session["aID"];
                    command.Parameters.Add("@odgovor", SqlDbType.VarChar);
                    command.Parameters["@odgovor"].Value = item.ToString();
                    command.ExecuteNonQuery();

                }
                connection.Close();
            }

            Response.Redirect("Admin.aspx");
        }
    }
    private void IzbrisiOdgovorClick(object sender, EventArgs e)
    {
        Control c = FindControl("lb");
        ListBox l = (ListBox)c;
        if (l.SelectedItem != null) { l.Items.Remove(l.SelectedItem); }
    }

    private void DodajOdgovorClick(object sender, EventArgs e)
    {
        Control c = FindControl("lb");
        Control ct = FindControl("odgtb");
        TextBox tb = (TextBox)ct;
        ListBox l = (ListBox)c;
        if (tb.Text != "")
        {
            l.Items.Add(tb.Text);
            tb.Text = "";
        }
    }

    private void DodajAnketu(int call)
    {
        Label naslovAnkete = new Label();
        Label aktivnaOd = new Label();
        Label aktivnaDo = new Label();
        Label odgovori = new Label();
        Label notify = new Label();

        notify.ID = "notify";
        notify.ForeColor = Color.Red;

        ListBox odgovoriLb = new ListBox();
        odgovoriLb.Width = 280;
        odgovoriLb.ID = "lb";

        naslovAnkete.Text = "Naslov ankete:* ";
        aktivnaOd.Text = "Aktivna od:* ";
        aktivnaDo.Text = "Aktivna do:* ";
        odgovori.Text = "Odgovori: ";

        TextBox naslovAnketeTb = new TextBox();
        TextBox aktivnaOdTb = new TextBox();
        TextBox aktivnaDoTb = new TextBox();
        TextBox dodajOdgovor = new TextBox();
        dodajOdgovor.ID = "odgtb";
        naslovAnketeTb.ID = "naslovtb";
        aktivnaOdTb.ID = "aktivnaodtb";
        aktivnaDoTb.ID = "aktivnadotb";

        Button dodaj = new Button();
        Button izbrisi = new Button();
        Button dodajAnketu = new Button();

        if (call == 1)
        {
            dodajAnketu.ID = "btn_spremi";
            dodaj.ID = "btn_dodaj";
            izbrisi.ID = "btn_izbrisi";
        }
        else
        {
            dodajAnketu.ID = "btn_spremi1";
            dodaj.ID = "btn_dodaj1";
            izbrisi.ID = "btn_izbrisi1";
        }

        dodaj.Click += new EventHandler(DodajOdgovorClick);
        izbrisi.Click += new EventHandler(IzbrisiOdgovorClick);
        if (call == 1)
        {
            dodajAnketu.Click += new EventHandler(SpremiAnketuClick);
            naslovAnketeTb.Text = Session["naslov"].ToString();
            aktivnaOdTb.Text = Session["aktivnaOd"].ToString();
            aktivnaDoTb.Text = Session["aktivnaDo"].ToString();
        }
        else
        {
            dodajAnketu.Click += new EventHandler(DodajAnketuUBazu);
            naslovAnketeTb.Text = "";
            aktivnaOdTb.Text = "";
            aktivnaDoTb.Text = "";
        }

        dodaj.Text = "Dodaj odgovor";
        izbrisi.Text = "Izbriši selektirani odgovor";
        dodajAnketu.Text = "Spremi anketu";


        Button naPopis = new Button();
        naPopis.Text = "Natrag na popis anketa";
        naPopis.ID = "btn_napopis";


        //add to form
        pregledPojedinacneAnkete.Controls.Add(naslovAnkete);
        pregledPojedinacneAnkete.Controls.Add(naslovAnketeTb);
        pregledPojedinacneAnkete.Controls.Add(new LiteralControl("<br /><br />"));
        pregledPojedinacneAnkete.Controls.Add(aktivnaOd);
        pregledPojedinacneAnkete.Controls.Add(aktivnaOdTb);
        pregledPojedinacneAnkete.Controls.Add(new LiteralControl("<br />"));
        pregledPojedinacneAnkete.Controls.Add(aktivnaDo);
        pregledPojedinacneAnkete.Controls.Add(aktivnaDoTb);
        pregledPojedinacneAnkete.Controls.Add(new LiteralControl("<br /> <br />"));
        pregledPojedinacneAnkete.Controls.Add(odgovori);
        pregledPojedinacneAnkete.Controls.Add(dodajOdgovor);
        pregledPojedinacneAnkete.Controls.Add(dodaj);
        pregledPojedinacneAnkete.Controls.Add(new LiteralControl("<br /> <br />"));
        pregledPojedinacneAnkete.Controls.Add(odgovoriLb);
        pregledPojedinacneAnkete.Controls.Add(new LiteralControl("<br />"));
        pregledPojedinacneAnkete.Controls.Add(izbrisi);
        pregledPojedinacneAnkete.Controls.Add(new LiteralControl("<br /> <br />"));
        pregledPojedinacneAnkete.Controls.Add(dodajAnketu);
        pregledPojedinacneAnkete.Controls.Add(new LiteralControl("<br /> <br />"));
        pregledPojedinacneAnkete.Controls.Add(naPopis);
        pregledPojedinacneAnkete.Controls.Add(new LiteralControl("<br /> <br />"));
        pregledPojedinacneAnkete.Controls.Add(notify);

        pregledPojedinacneAnkete.Visible = true;
    }

    private void DodajAnketuUBazu(object sender, EventArgs e)
    {
        Control c = FindControl("lb");
        ListBox listB = (ListBox)c;
        c = FindControl("naslovtb");
        TextBox tbNaslov = (TextBox)c;
        c = FindControl("aktivnaodtb");
        TextBox aktivnaOd = (TextBox)c;
        c = FindControl("aktivnadotb");
        TextBox aktivnaDo = (TextBox)c;
        c = FindControl("notify");
        Label notLb = (Label)c;
        DateTime aOd;
        DateTime aDo;

        if (tbNaslov.Text == "" || aktivnaOd.Text == "" || aktivnaDo.Text == "" || listB.Items.Count < 2 || !DateTime.TryParse(aktivnaOd.Text, out aOd) || !DateTime.TryParse(aktivnaOd.Text, out aDo))
        {
            notLb.Text = "Niti jedno polje označeno zvjezdicom ne smije ostati prazno, anketa mora imati minimalno dva dodana odgovora i mora biti ispravan format datuma !";
        }
        else
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Korisnik\source\repos\Anketa-vipdata\Anketa-vipdata\App_Data\AnketaDB.mdf;Integrated Security=True;MultipleActiveResultSets=true;";
            string query;
            SqlCommand command;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                query = "INSERT INTO Anketa(naslovAnkete,aktivnaDo,aktivnaOd) VALUES (@naslov,@aktivnaDo,@aktivnaOd)";
                command = new SqlCommand(query, connection);
                command = new SqlCommand(query, connection);
                command.Parameters.Add("@naslov", SqlDbType.VarChar);
                command.Parameters["@naslov"].Value = tbNaslov.Text;
                command.Parameters.Add("@aktivnaDo", SqlDbType.DateTime);
                command.Parameters["@aktivnaDo"].Value = aDo;
                command.Parameters.Add("@aktivnaOd", SqlDbType.DateTime);
                command.Parameters["@aktivnaOd"].Value = aOd;
                command.Parameters["@aktivnaOd"].Value = aOd;

                connection.Open();
                command.ExecuteNonQuery();
                foreach (var item in listB.Items)
                {
                    query = "INSERT INTO Odgovori(anketaID,tekstOdgovora,brojGlasova) VALUES (IDENT_CURRENT('Anketa'),@odgovor,0)";
                    command = new SqlCommand(query, connection);
                    command.Parameters.Add("@odgovor", SqlDbType.VarChar);
                    command.Parameters["@odgovor"].Value = item.ToString();
                    command.ExecuteNonQuery();

                }
                connection.Close();
            }
        }
        Response.Redirect("Admin.aspx");
    }
}