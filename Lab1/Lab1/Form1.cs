using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace Lab1
{
    public partial class Form1 : Form
    {


        //Create an SQL Connection Object via which to connect to the database
        SqlConnection con;

        //For the parent table(Store) and the child table(Shop)
        SqlDataAdapter adapterShop;
        SqlDataAdapter adapterStore;

        DataSet ds;

        string queryShop;
        string queryStore;

        SqlCommandBuilder commandBuilder;

        BindingSource bindingStore;
        BindingSource binndingShop;


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


        public Form1()
        {
            this.BackColor = Color.FromArgb(255, 232, 232);
            InitializeComponent();
            fillData();
            this.dataGridView2.CellValidating += new DataGridViewCellValidatingEventHandler(dataGridView2_CellValidating);

        }
        
        void fillData()
        {
            //sqlconnection
            con = new SqlConnection(getConnectionString());

            queryStore = "SELECT * FROM Store";
            queryShop = "SELECT * FROM Shop";

            //sql adapter, dataset
            adapterStore = new SqlDataAdapter(queryStore, con);
            adapterShop = new SqlDataAdapter(queryShop, con);

            ds = new DataSet();
            adapterStore.Fill(ds, "Store");
            adapterShop.Fill(ds, "Shop");

            //fill in the insert, update delete methods to the data adapter
            commandBuilder = new SqlCommandBuilder(adapterShop);



            ds.Relations.Add("Store_Shop", ds.Tables["Store"].Columns["storeID"], ds.Tables["Shop"].Columns["storeID"]);



            //fill the data into the grid view
            //Method1()
            /*this.dataGridView1.DataSource = ds.Tables["Store"];
            this.dataGridView2.DataSource = this.dataGridView1.DataSource;
            this.dataGridView2.DataMember = "Store_Shop";*/


            //Method2()
            bindingStore = new BindingSource();
            bindingStore.DataSource = ds.Tables["Store"];
            binndingShop = new BindingSource(bindingStore, "Store_Shop");

            this.dataGridView1.DataSource = bindingStore;
            this.dataGridView2.DataSource = binndingShop;
        }



        private void label1_Click(object sender, EventArgs e)
        {

        }
        string getConnectionString()
        {
            //Connection string pointing to database "Clothing Store 2"
            return "Data Source=LAPTOP-NI9TIBLO\\SQLEXPRESS;" + "Initial Catalog=Clothing Store 2;Integrated Security=true;";
        }
        private bool ValidateRow(DataGridViewRow row)
        {
            return true;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                adapterShop.Update(ds, "Shop");
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
