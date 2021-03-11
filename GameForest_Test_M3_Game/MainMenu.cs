using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameForest_Test_M3_Game
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
        }

        //Запуск игры при нажатии на кнопку Play в главном меню
        private void PlayButton_Click(object sender, EventArgs e)
        {
            Level Next = new Level();
            Next.FormClosed += delegate { this.Show(); };
            Next.Show();
            Hide();
        }

        //Выход по кнопке ESC
        private void MainMenu_KeyDown(object sender, KeyEventArgs e) { if (e.KeyCode == Keys.Escape){ Close(); } }
    }
}
