using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EF_CRUD
{
    public partial class Form1 : Form
    {
        Car model = new Car();
        public Form1()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearBoxes();
        }

        void ClearBoxes()
        {
            txtBrand.Text = txtModel.Text = txtYear.Text = "";
            btnSave.Text = "Save";
            btnDelete.Enabled = false;
            model.CarID = 0;
        }
        //lo que se ejecuta al abrir la ventana
        private void Form1_Load(object sender, EventArgs e)
        {
            ClearBoxes();
            FillDGV();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            model.Brand = txtBrand.Text.Trim();
            model.Model = txtModel.Text.Trim();
            model.Year = Convert.ToInt32(txtYear.Text.Trim());

            using (DBEntities db = new DBEntities())
            {
                if (model.CarID == 0)//Insert
                    db.Cars.Add(model);
                else //update
                    db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
            ClearBoxes();
            FillDGV();
            MessageBox.Show("Saved!");
        }

        //llenar la data grid view con los registros de la DB
        void FillDGV()
        {
            dgvCars.AutoGenerateColumns = false;
            using (DBEntities db = new DBEntities())
            {
                dgvCars.DataSource = db.Cars.ToList<Car>();
            }
        }

        private void dgvCars_DoubleClick(object sender, EventArgs e)
        {
            //si la fila seleccionada no es la de los headers (para saber si es una fila valida)
            if(dgvCars.CurrentRow.Index != -1)
            {
                model.CarID = Convert.ToInt32(dgvCars.CurrentRow.Cells["CarID"].Value);
                using (DBEntities db = new DBEntities())
                {

                    model = db.Cars.Where(x => x.CarID == model.CarID).FirstOrDefault();
                    txtBrand.Text = model.Brand;
                    txtModel.Text = model.Model;
                    txtYear.Text = model.Year.ToString();
                    btnSave.Text = "Update";
                    btnDelete.Enabled = true;
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Delete?", "CRUD",MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using(DBEntities db = new DBEntities())
                {
                    var entry = db.Entry(model);
                    if(entry.State == EntityState.Detached)
                    {
                        db.Cars.Attach(model);
                    }
                    db.Cars.Remove(model);
                    db.SaveChanges();
                    FillDGV();
                    ClearBoxes();
                    MessageBox.Show("Deleted");
                }
            }
        }
    }
}
