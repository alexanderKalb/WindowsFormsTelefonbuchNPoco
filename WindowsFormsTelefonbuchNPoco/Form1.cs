using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NPoco;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace WindowsFormsTelefonbuchNPoco
{
    public partial class Form1 : Form
    {
        List<Person> lstPerson = new List<Person>();
        string ConnectionString { get; } = ConfigurationManager.ConnectionStrings["mariadb"].ConnectionString;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DatenLaden();
            AnzeigeAktualisieren();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void buttonadd_Click(object sender, EventArgs e)
        {
            PersonHinzufügen();
            DatenLaden();
            AnzeigeAktualisieren();
        }

        private void buttonchange_Click(object sender, EventArgs e)
        {
            PersonÄndern();
            DatenLaden();
            AnzeigeAktualisieren();
        }

        private void buttondelete_Click(object sender, EventArgs e)
        {
            PersonLöschen();
            DatenLaden();
            AnzeigeAktualisieren();
        }
        private void DatenLaden()
        {

            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                using (IDatabase db = new Database(connection))
                {
                    db.Connection.Open();
                    lstPerson.Clear();
                    lstPerson = db.Fetch<Person>("order by name,vorname");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
        private void AnzeigeAktualisieren()
        {
            // dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Instance setzen
            foreach (Person p in lstPerson)
            {
                p.Instance = p;
            }
            dataGridView1.DataSource = null;
            comboBox1.DataSource = null;
            dataGridView1.DataSource = lstPerson;
            comboBox1.DataSource = lstPerson;

            dataGridView1.Columns["Instance"].Visible = false;
            dataGridView1.RowHeadersVisible = false;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Person p = dataGridView1.CurrentRow.Cells["Instance"].Value as Person;
            if (p != null)
            {
                textBoxID.Text = p.Id.ToString();
                textBoxfName.Text = p.Vorname;
                textBoxlName.Text = p.Name;
                textBoxPhone.Text = p.Telefon;
                textBoxMail.Text = p.Email;
                comboBox1.SelectedItem = p;
            }
        }

        private void PersonHinzufügen()
        {
            try
            {
                Person p = new Person();
                p.Id = 0; // wird von Datenbank vergeben
                p.Vorname = textBoxfName.Text;
                p.Name = textBoxlName.Text;
                p.Telefon = textBoxPhone.Text;
                p.Email = textBoxMail.Text;
                p.GeaendertAm = DateTime.Now;

                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                using (IDatabase db = new Database(connection))
                {
                    db.Connection.Open();
                    db.Insert(p);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
        private void PersonÄndern()
        {
            try
            {
                Person p = new Person();
                p.Id = Convert.ToInt32(textBoxID.Text); // id übernehmen
                p.Vorname = textBoxfName.Text;
                p.Name = textBoxlName.Text;
                p.Telefon = textBoxPhone.Text;
                p.Email = textBoxMail.Text;
                p.GeaendertAm = DateTime.Now;

                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                using (IDatabase db = new Database(connection))
                {
                    db.Connection.Open();
                    db.Update(p);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

        }

        private void PersonLöschen()
        {
            try
            {
                Person p = comboBox1.SelectedItem as Person;
                if (p != null)
                {
                    using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                    using (IDatabase db = new Database(connection))
                    {
                        db.Connection.Open();
                        db.Delete(p);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }

    class Person
    {
        public int Id { get; set; }
        public string Vorname { get; set; }
        public string Name { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
        public DateTime GeaendertAm { get; set; }
        
        [NPoco.Ignore]
        public Person Instance { get; set; }
        
        public Person()
        {

        }

        public override string ToString()
        {
            return $"ID: {Id}, Name: {Vorname} {Name}, Tel: {Telefon}, Email: {Email}, geändert am {GeaendertAm}";
        }
    }

}
