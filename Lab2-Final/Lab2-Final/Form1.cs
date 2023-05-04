using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;


namespace Lab2_Final
{
    public partial class Form1 : Form
    {

        SqlConnection con;
        SqlDataAdapter daParent;
        SqlDataAdapter daChild;
        DataSet ds;
        BindingSource bsParent;
        BindingSource bsChild;


        string queryParent;
        string queryChild;
        SqlCommandBuilder cmdBuilder;


        private void dataGridView2_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == 0) // ID column
            {
                int id;
                if (!int.TryParse(e.FormattedValue.ToString(), out id))
                {
                    e.Cancel = true;
                    MessageBox.Show("The ID must be a valid integer.");
                }
            }
        }
        void fillData()
        {

            con = new SqlConnection(getConnectionString());
            MessageBox.Show(ConfigurationManager.AppSettings["greeting"]);


            queryParent = System.Configuration.ConfigurationManager.AppSettings["queryParent"];
            queryChild = System.Configuration.ConfigurationManager.AppSettings["queryChild"];


            daParent = new SqlDataAdapter(queryParent, con);
            daChild = new SqlDataAdapter(queryChild, con);
            ds = new DataSet();

            daParent.Fill(ds, System.Configuration.ConfigurationManager.AppSettings["parentTable"]);

            daChild.Fill(ds, System.Configuration.ConfigurationManager.AppSettings["childTable"]);


            //fill in the Insert, update, delete commands
            cmdBuilder = new SqlCommandBuilder(daChild);


            string parentTable = System.Configuration.ConfigurationManager.AppSettings["parentTable"];
            string childTable = System.Configuration.ConfigurationManager.AppSettings["childTable"];
            string parentColumnName = System.Configuration.ConfigurationManager.AppSettings["parentColumn"];
            string childColumnName = System.Configuration.ConfigurationManager.AppSettings["childColumn"];




            ds.Relations.Add("Parent_Child",
            ds.Tables[parentTable].Columns[parentColumnName],
            ds.Tables[childTable].Columns[childColumnName]);




            bsParent = new BindingSource();
            bsParent.DataSource = ds.Tables[parentTable];
            bsChild = new BindingSource(bsParent, "Parent_Child");

            this.dataGridView1.DataSource = bsParent;
            this.dataGridView2.DataSource = bsChild;
        }
        string getConnectionString()
        {
            //Connection string pointing to database "Clothing Store 2"
            return "Data Source=LAPTOP-NI9TIBLO\\SQLEXPRESS;" + "Initial Catalog=Clothing Store 2;Integrated Security=True;";
            //System.Configuration.ConfigurationManager.ConnectionStrings["connection_string"].ConnectionString;
        }
        public Form1()
        {

            this.BackColor = Color.FromArgb(255, 232, 232);
            InitializeComponent();
            fillData();
            this.dataGridView2.CellValidating += new DataGridViewCellValidatingEventHandler(dataGridView2_CellValidating);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                daChild.Update(ds, System.Configuration.ConfigurationManager.AppSettings["childTable"]);
                MessageBox.Show("Data uploaded successfully!");

            }
            catch (SqlException ex)
            {

                if (ex.Number == 2627) // validates the primary key; checks if it appears already or not
                {
                    MessageBox.Show("A shop with the same ID already exists");
                }
                else if (ex.Message.Contains("Cannot insert the value NULL into column"))
                {
                    MessageBox.Show("The ID field cannot be epmty. Please fill in the required field.");
                }
                else
                {
                    MessageBox.Show("An error occured while updating the data: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while updating the data: " + ex.Message);
            }
        }
    }
}
