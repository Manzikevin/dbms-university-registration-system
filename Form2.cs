using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UniRegstrationSys
{
    public partial class MDIParent : Form
    {
        public MDIParent()
        {
            InitializeComponent();
            this.IsMdiContainer = true; // Ensures this form is set as the MDI container
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void MDIParent_Load(object sender, EventArgs e)
        {
           
        }


        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = this.MdiChildren.Length - 1; i >= 0; i--)
            {
                Form openForm = this.MdiChildren[i];

                if (openForm is Form3)
                {
                    openForm.Activate();
                    return;
                }

                openForm.Close();
            }

            Form3 addStudentForm = new Form3();
            addStudentForm.MdiParent = this;
            addStudentForm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = this.MdiChildren.Length - 1; i >= 0; i--)
            {
                Form openForm = this.MdiChildren[i];

                if (openForm is StudentForm)
                {
                    openForm.Activate();
                    return;
                }

                openForm.Close();
            }

            StudentForm ManageStudentForm = new StudentForm();
            ManageStudentForm.MdiParent = this;
            ManageStudentForm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = this.MdiChildren.Length - 1; i >= 0; i--)
            {
                Form openForm = this.MdiChildren[i];

                if (openForm is Form3)
                {
                    openForm.Activate();
                    return;
                }

                openForm.Close();
            }

            Form3 addStudentForm = new Form3();
            addStudentForm.MdiParent = this;
            addStudentForm.Show();
        }


    }
}