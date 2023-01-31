using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Data.SqlClient;

namespace SQLITE_TEST
{
    public partial class Form1 : Form
    {
        //path of data base
        string path = "data_table3.db";
        string cs = @"URI=file:"+Application.StartupPath+ "\\data_table3.db"; //database creat debug folder

        SQLiteConnection con;
        SQLiteCommand cmd;
        SQLiteDataReader dr;

        public Form1()
        {
            InitializeComponent();
        }
        //show data in table
        private void data_show()
        {
            var con = new SQLiteConnection(cs);
            con.Open();

            string stm = "SELECT * FROM test";
            var cmd = new SQLiteCommand(stm,con);
            dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                dataGridView1.Rows.Insert(0,dr.GetString(0),dr.GetString(1), dr.GetString(2), dr.GetString(3), dr.GetString(4), dr.GetString(5));
            }
        }

        //create database and table
        private void Create_db()
        {
            if (!System.IO.File.Exists(path))
            {
                SQLiteConnection.CreateFile(path);
                using (var sqlite = new SQLiteConnection(@"Data Source=" + path))
                {
                    sqlite.Open();
                    string sql = "create table test(id varchar(20),name varchar(20),surname varchar(20),telephone varchar(20),email varchar(20),birthday varchar(20))";
                    SQLiteCommand command = new SQLiteCommand(sql,sqlite);
                    command.ExecuteNonQuery();
                }
            }
            else
            {
                Console.WriteLine("Database cannot create");
                return;
            }
        }

        //insert data
        private void Insert_btn_Click(object sender, EventArgs e)
        {
            var con = new SQLiteConnection(cs);
            con.Open();
            var cmd = new SQLiteCommand(con);

            try
            {
                if (id_txt.Text == "" || name_txt.Text == "" || textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "")
                {
                    MessageBox.Show("Some input are empty");
                }
                else
                {
                    cmd.CommandText = "INSERT INTO test(id,name,surname,telephone,email,birthday) VALUES(@id,@name,@surname,@telephone,@email,@birthday)";

                    string NAME = name_txt.Text;
                    string ID = id_txt.Text;
                    string SURNAME = textBox1.Text;
                    string TEL = textBox2.Text;
                    string EMAIL = textBox3.Text;
                    string BIRTH = textBox4.Text;

                    cmd.Parameters.AddWithValue("@id", ID);
                    cmd.Parameters.AddWithValue("@name", NAME);
                    cmd.Parameters.AddWithValue("@surname", SURNAME);
                    cmd.Parameters.AddWithValue("@telephone", TEL);
                    cmd.Parameters.AddWithValue("@email", EMAIL);
                    cmd.Parameters.AddWithValue("@birthday", BIRTH);

                    dataGridView1.ColumnCount = 6;
                    dataGridView1.Columns[0].Name = "Id";
                    dataGridView1.Columns[1].Name = "Name";
                    dataGridView1.Columns[2].Name = "Surname";
                    dataGridView1.Columns[3].Name = "Telephone";
                    dataGridView1.Columns[4].Name = "Email";
                    dataGridView1.Columns[5].Name = "Birthday";
                    string[] row = new string[] { ID, NAME, SURNAME, TEL, EMAIL, BIRTH };
                    dataGridView1.Rows.Add(row);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("cannot insert data");
                return;
            }

        }

        // update data
        private void update_btn_Click(object sender, EventArgs e)
        {
            var con = new SQLiteConnection(cs);
            con.Open();

            var cmd = new SQLiteCommand(con);

            try
            {
                cmd.CommandText = "UPDATE test SET name=@name,surname=@surname, telephone=@telephone,email=@email,birthday=@birthday where id=@id";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@Id", id_txt.Text);
                cmd.Parameters.AddWithValue("@Name", name_txt.Text);
                cmd.Parameters.AddWithValue("@Surname", textBox1.Text);
                cmd.Parameters.AddWithValue("@Telephone", textBox2.Text);
                cmd.Parameters.AddWithValue("@Email", textBox3.Text);
                cmd.Parameters.AddWithValue("@Birthday", textBox4.Text);

                cmd.ExecuteNonQuery();
                dataGridView1.Rows.Clear();
                data_show();

            }
            catch(Exception)
            {
                Console.WriteLine("cannot update data");
                return;
            }
        }

        // delete data
        private void delete_btn_Click(object sender, EventArgs e)
        {
            var con = new SQLiteConnection(cs);
            con.Open();

            var cmd = new SQLiteCommand(con);

            try
            {
                cmd.CommandText = "DELETE FROM test where name =@Name";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@Name", name_txt.Text);

                cmd.ExecuteNonQuery();
                dataGridView1.Rows.Clear();
                data_show();
            }
            catch (Exception)
            {
                Console.WriteLine("cannot delete data");
                return;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value !=null)
            {
                dataGridView1.CurrentRow.Selected = true;
                name_txt.Text = dataGridView1.Rows[e.RowIndex].Cells["Name"].FormattedValue.ToString();
                id_txt.Text = dataGridView1.Rows[e.RowIndex].Cells["Id"].FormattedValue.ToString();
                textBox1.Text = dataGridView1.Rows[e.RowIndex].Cells["Surname"].FormattedValue.ToString();
                textBox2.Text = dataGridView1.Rows[e.RowIndex].Cells["Telephone"].FormattedValue.ToString();
                textBox3.Text = dataGridView1.Rows[e.RowIndex].Cells["Email"].FormattedValue.ToString();
                textBox4.Text = dataGridView1.Rows[e.RowIndex].Cells["Birthday"].FormattedValue.ToString();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Create_db();
            data_show();
        }
        private void Search_bd()
        {
            try
            {
                var con = new SQLiteConnection(cs);
                con.Open();


                var sql = "SELECT * FROM test";
                sql += " WHERE id Like @keyword";
                sql += " OR name Like @keyword";
                sql += " OR surname Like @keyword";
                sql += " OR telephone Like @keyword";
                sql += " OR email Like @keyword";
                sql += " OR birthday Like @keyword";
                sql += " ORDER BY id ASC";


                var cmd = new SQLiteCommand(sql, con);
                //dr = cmd.ExecuteReader();
                cmd.CommandType= CommandType.Text;
                cmd.Parameters.Clear();

                string keyword = string.Format("%{0}%", textBox5.Text);
                cmd.Parameters.AddWithValue("keyword", keyword);
                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    dataGridView1.DataSource = dr.Read();
                }
            }
            catch
            {
                Console.WriteLine("Cannot search");
                return;
            }

        }
        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                Search_bd();
            }
            else
            {
               
            }
        }
    }
}
