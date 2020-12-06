//Eduardo Migueis - 19167 e Rodrigo Smith - 19197

using apCaminhosMarte.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace apCaminhosMarte
{
    public partial class FrmApp : Form
    {
        private bool achou = false;
        private Graphics g;
        private Cidade origem;
        private Cidade destino;
        private ArvoreBinaria<Cidade> arvore;
        private AvancoCaminho[,] matrizCaminhos;
        private List<AvancoCaminho> listaCaminhos;
        private List<AvancoCaminho[]> resultados;
        private Stack<AvancoCaminho> caminhoEncontrado;
        private AvancoCaminho[] listaMelhorCaminho;

        internal ArvoreBinaria<Cidade> Arvore { get => arvore; set => arvore = value; }
        internal AvancoCaminho[,] MatrizCaminhos { get => matrizCaminhos; set => matrizCaminhos = value; }
        internal List<AvancoCaminho> ListaCaminhos { get => listaCaminhos; set => listaCaminhos = value; }
        internal List<AvancoCaminho[]> Resultados { get => resultados; set => resultados = value; }
        internal Stack<AvancoCaminho> CaminhoEncontrado { get => caminhoEncontrado; set => caminhoEncontrado = value; }
        internal AvancoCaminho[] ListaMelhorCaminho { get => listaMelhorCaminho; set => listaMelhorCaminho = value; }

        private bool dgvClicado = false;
        private string cidadeClicada;
        private bool dbClick = false;
        private bool radio = false;
        private List<AvancoCaminho> listaCaminho = new List<AvancoCaminho>();

        public FrmApp()
        {
            InitializeComponent();
        }

        private void FrmApp_Load(object sender, EventArgs e)
        {
            panel2.BackColor = Color.FromArgb(255, 60, 80, 185);
            lsbDestino.BackColor = Color.FromArgb(255, 60, 80, 185);
            lsbOrigem.BackColor = Color.FromArgb(255, 60, 80, 185);
            btnBuscar.BackColor = Color.FromArgb(255, 60, 80, 185);

            label5.Visible = false;
            label8.Visible = false;

            dataGridView1.RowHeadersVisible = false;
            dataGridView2.RowHeadersVisible = false;

            var leitor = new LeitorDeArquivoMarsMap();

            Arvore = leitor.LerCidades();
            ListaCaminhos = leitor.LerCaminhos();
            MatrizCaminhos = leitor.LerCaminhosComoMatriz();

            var lista = Arvore.ToList();

            List<LsbItems> dataOrigem = new List<LsbItems>();
            List<LsbItems> dataDestino = new List<LsbItems>();

            for (int i = 0; i < lista.Count; i++)
            {
                dataOrigem.Add(new LsbItems(lista[i].Id, lista[i].Nome));
                dataDestino.Add(new LsbItems(lista[i].Id, lista[i].Nome));
            }

            dataDestino.Sort();
            dataOrigem.Sort();

            lsbDestino.DataSource = dataDestino;
            lsbOrigem.DataSource = dataOrigem;

            lsbDestino.DisplayMember = "Text";
            lsbOrigem.DisplayMember = "Text";
        }


        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            achou = false;
            radio = true;
            dgvClicado = false;
            pbMapa.Refresh();

            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView2.Columns.Clear(); //limpa dgvs

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            if (lsbOrigem.SelectedIndex == lsbDestino.SelectedIndex)
            {
                label3.Visible = false;
                label4.Visible = false;
                dataGridView1.Visible = false;
                dataGridView2.Visible = false;
                radioButton4.Visible = false;
                radioButton5.Visible = false;
                radioButton6.Visible = false;
                label8.Visible = false;
                label5.Visible = true;
                return;
            }

            origem = Arvore.Busca(new Cidade((lsbOrigem.SelectedItem as LsbItems).Id, default, default, default));
            destino = Arvore.Busca(new Cidade((lsbDestino.SelectedItem as LsbItems).Id, default, default, default));

            bool temSolucao = false;

            if (radioButton7.Checked)
                temSolucao = Solucionador.BuscarCaminhosR(ref caminhoEncontrado, ref resultados, arvore, origem, destino, ref matrizCaminhos);
            else if (radioButton9.Checked)
                temSolucao = Solucionador.BuscarCaminhosP(ref caminhoEncontrado, ref resultados, arvore, origem, destino, ref matrizCaminhos);
            else
                temSolucao = Solucionador.BuscarCaminhosR(ref caminhoEncontrado, ref resultados, arvore, origem, destino, ref matrizCaminhos);
            if (!temSolucao) // chama o método de solução de caminhos
            {
                //não achou caminhos
                label5.Visible = false;
                label8.Visible = true;
                label3.Visible = false;
                label4.Visible = false;
                dataGridView1.Visible = false;
                dataGridView2.Visible = false;
                radioButton4.Visible = false;
                radioButton5.Visible = false;
                radioButton6.Visible = false;
                achou = false;
            }
            else
            {
                //achou caminhos
                label5.Visible = false;
                label8.Visible = false;
                label3.Visible = true;
                label4.Visible = true;
                dataGridView1.Visible = true;
                dataGridView2.Visible = true;
                radioButton4.Visible = true;
                radioButton5.Visible = true;
                radioButton6.Visible = true;
                achou = true;
                ExibirTodosOsCaminhosNoDGV();
                if (!radioButton8.Checked)
                    GetMelhorCaminho();
                else
                    GetMelhorCaminhoDijkstra();
                ExibirMelhorCaminhoNoDGV();
                pbMapa.Refresh();
                radioButton4.PerformClick();
            }
        }

        //Ambos os event handlers abaixo para DrawItem são para personalização da cor de seleção das ListBoxes.
        private void lsbOrigem_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (lsbOrigem.Items.Count == 0)
                return;

            if (e.Index < 0)
                return;

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                e = new DrawItemEventArgs(e.Graphics,
                          e.Font,
                          e.Bounds,
                          e.Index,
                          e.State ^ DrawItemState.Selected,
                          e.ForeColor,
                          Color.FromArgb(229, 237, 250));
                e.DrawBackground();
                e.Graphics.DrawString(lsbOrigem.Items[e.Index].ToString(), e.Font, Brushes.Black, e.Bounds, StringFormat.GenericDefault);
            }
            else
            {
                e.DrawBackground();
                e.Graphics.DrawString(lsbOrigem.Items[e.Index].ToString(), e.Font, Brushes.WhiteSmoke, e.Bounds, StringFormat.GenericDefault);
            }

            e.DrawFocusRectangle();
        }

        private void lsbDestino_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (lsbDestino.Items.Count == 0)
                return;

            if (e.Index < 0)
                return;

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                e = new DrawItemEventArgs(e.Graphics,
                                          e.Font,
                                          e.Bounds,
                                          e.Index,
                                          e.State ^ DrawItemState.Selected,
                                          e.ForeColor,
                                          Color.FromArgb(229, 237, 250));

                e.DrawBackground();
                e.Graphics.DrawString(lsbDestino.Items[e.Index].ToString(), e.Font, Brushes.Black, e.Bounds, StringFormat.GenericDefault);
            }
            else
            {
                e.DrawBackground();
                e.Graphics.DrawString(lsbDestino.Items[e.Index].ToString(), e.Font, Brushes.WhiteSmoke, e.Bounds, StringFormat.GenericDefault);
            }

            e.DrawFocusRectangle();
        }


        private void pbMapa_Paint(object sender, PaintEventArgs e)
        {
            g = e.Graphics;

            if (dgvClicado)
            {
                DesenharCidade(cidadeClicada, "Poppins");
            }
            else if (dbClick)
            {

                if (radioButton1.Checked)
                {
                    DesenharCidades("Poppins");
                }
                else if (radioButton2.Checked)
                {
                    DesenharLinhas();
                    DesenharCidades("Poppins");
                }
                else
                {
                    DesenharLinhasComSetas();
                    DesenharCidades("Poppins");
                }

                DesenharCaminhoEspecifico(Color.FromArgb(210, 30, 20));
                listaCaminho.Clear();
            }
            else
            {
                if (radioButton1.Checked)
                {
                    DesenharCidades("Poppins");
                }
                else if (radioButton2.Checked)
                {
                    DesenharLinhas();
                    DesenharCidades("Poppins");
                }
                else
                {
                    DesenharLinhasComSetas();
                    DesenharCidades("Poppins");
                }

                if (achou)
                {
                    DesenharMelhorCaminho();
                    DesenharCidades("Poppins");
                }
            }

            dbClick = false;
        }

        //Inicialização do form
        private void FrmApp_Shown(object sender, EventArgs e)
        {
            var pb = new PictureBox();
            pb.Width = panel7.Width;
            pb.Height = panel7.Height;
            pb.SizeMode = PictureBoxSizeMode.AutoSize;
            Bitmap bmp = new Bitmap(panel7.Width, panel7.Height);
            DesenharArvore(true, Arvore.Raiz, bmp.Width / 2, 80, (Math.PI / 180) * 90, 1.2, 500, "Poppins", Graphics.FromImage(bmp));
            pb.Image = bmp;

            panel7.Controls.Add(pb);

            panel7.Controls[panel7.Controls.IndexOf(panel8)].BringToFront();

            radioButton3.PerformClick();
            radioButton7.PerformClick();
        }

        private void FrmApp_Resize(object sender, EventArgs e)
        {
            if (Arvore == null)
                return;

            var pb = new PictureBox();
            pb.Width = panel7.Width;
            pb.Height = panel7.Height;
            pb.SizeMode = PictureBoxSizeMode.AutoSize;
            Bitmap bmp = new Bitmap(panel7.Width, panel7.Height);
            DesenharArvore(true, Arvore.Raiz, bmp.Width / 2, 80, (Math.PI / 180) * 90, 1, 400, "Poppins", Graphics.FromImage(bmp));
            pb.Image = bmp;

            panel7.Controls.Add(pb);

            panel7.Controls[panel7.Controls.IndexOf(panel8)].BringToFront();

        }

        private void radioButton1_Click(object sender, EventArgs e)
        {

            pbMapa.Refresh();
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            pbMapa.Refresh();
        }

        private void radioButton3_Click(object sender, EventArgs e)
        {
            pbMapa.Refresh();
        }

        private void radioButton4_Click(object sender, EventArgs e)
        {
            if (!radio)
                return;

            if (radioButton8.Checked)
                GetMelhorCaminhoDijkstra();
            else
                GetMelhorCaminho();

            ExibirMelhorCaminhoNoDGV();
            pbMapa.Refresh();
        }

        private void radioButton5_Click(object sender, EventArgs e)
        {
            if (!radio)
                return;

            if (radioButton8.Checked)
                GetMelhorCaminhoDijkstra();
            else
                GetMelhorCaminho();

            ExibirMelhorCaminhoNoDGV();
            pbMapa.Refresh();
        }

        private void radioButton6_Click(object sender, EventArgs e)
        {
            if (!radio)
                return;

            if (radioButton8.Checked)
                GetMelhorCaminhoDijkstra();
            else
                GetMelhorCaminho();

            ExibirMelhorCaminhoNoDGV();
            pbMapa.Refresh();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
                return;
            dgvClicado = true;
            radio = false;
            cidadeClicada = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
            pbMapa.Refresh();
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView2.Rows[0].Cells[e.ColumnIndex].Value.ToString() == null)
                return;
            dgvClicado = true;
            radio = false;
            cidadeClicada = dataGridView2.Rows[0].Cells[e.ColumnIndex].Value.ToString();
            dataGridView2.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dataGridView2.Rows[0].Cells[e.ColumnIndex].Selected = true;
            pbMapa.Refresh();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            dgvClicado = false;
            radio = false;
            dbClick = true;

            for (int i = 0; i < Resultados[e.RowIndex].Length; i++)
            {
                listaCaminho.Add(Resultados[e.RowIndex][i]);
            }

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Rows[e.RowIndex].Selected = true;
            pbMapa.Refresh();
        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            dgvClicado = false;
            radio = true;
            dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView2.Rows[0].Selected = true;
            pbMapa.Refresh();
        }

        private void DesenharArvore(bool primeiraVez, NoArvore<Cidade> raiz, int x, int y, double angulo, double incremento, double comprimento, string font, Graphics g)
        {
            int xf, yf;
            if (raiz != null)
            {
                Pen caneta = new Pen(Color.FromArgb(85, 85, 85), 2);
                xf = (int)Math.Round(x + Math.Cos(angulo) * comprimento);
                yf = (int)Math.Round(y + Math.Sin(angulo) * comprimento);
                if (primeiraVez)
                    yf = 80;
                else
                    comprimento -= 20;
                g.DrawLine(caneta, x, y, xf, yf);
                DesenharArvore(false, raiz.Esq, xf, yf, Math.PI / 2 + incremento,
                incremento * 0.60, comprimento * 0.8, font, g);
                DesenharArvore(false, raiz.Dir, xf, yf, Math.PI / 2 - incremento,
                incremento * 0.60, comprimento * 0.8, font, g);
                SolidBrush preenchimento = new SolidBrush(Color.MediumTurquoise);
                g.FillRectangle(preenchimento, xf - 45, yf, 80, 30);
                g.DrawString(Convert.ToString(raiz.Info.Nome), new Font(font, 7),
                new SolidBrush(Color.White), xf - 40, yf + 8);
            }
        }

        private void DesenharCidade(string cidadeClicada, string font)
        {
            for (int i = 0; i < Arvore.Qtd; i++)
            {
                string cidadeDGV;

                if (cidadeClicada.Contains(" ->"))
                    cidadeDGV = Arvore.Busca(new Cidade(i, default, default, default)).Nome + " ->";
                else
                    cidadeDGV = Arvore.Busca(new Cidade(i, default, default, default)).Nome;

                if (cidadeDGV.Equals(cidadeClicada))
                {
                    var cidade = new Cidade(i, default, default, default);
                    int x = (Arvore.Busca(cidade).X * pbMapa.Width) / 4096;
                    int y = (Arvore.Busca(cidade).Y * pbMapa.Height) / 2048;

                    g.FillRectangle(new SolidBrush(Color.Black), x - 3, y - 3, 6, 6);
                    g.DrawString(Arvore.Busca(cidade).Nome, new Font(font, 8, FontStyle.Bold), new SolidBrush(Color.Black), x + 3, y + 2);
                }
            }
        }

        private void DesenharCidades(string font)
        {
            for (int i = 0; i < Arvore.Qtd; i++)
            {
                var cidade = new Cidade(i, default, default, default);
                int x = (Arvore.Busca(cidade).X * pbMapa.Width) / 4096;
                int y = (Arvore.Busca(cidade).Y * pbMapa.Height) / 2048;

                g.FillRectangle(new SolidBrush(Color.Black), x - 3, y - 3, 6, 6);
                g.DrawString(Arvore.Busca(cidade).Nome, new Font(font, 8, FontStyle.Bold), new SolidBrush(Color.Black), x + 3, y + 2);
            }
        }

        private void DesenharLinhas()
        {
            for (int i = 0; i < ListaCaminhos.Count; i++)
            {
                Pen c = new Pen(Color.DarkRed, 1);
                c.DashStyle = DashStyle.Dash;
                c.DashPattern = new float[] { 4.0f, 4.0f, 4.0f, 4.0f };
                c.DashCap = DashCap.Round;
                g.DrawLine(c, (ListaCaminhos[i].Origem.X * pbMapa.Width) / 4096, (ListaCaminhos[i].Origem.Y * pbMapa.Height) / 2048, (ListaCaminhos[i].Destino.X * pbMapa.Width) / 4096, (ListaCaminhos[i].Destino.Y * pbMapa.Height) / 2048);
            }
        }

        private void DesenharLinhasComSetas()
        {
            for (int i = 0; i < ListaCaminhos.Count; i++)
            {
                Pen c = new Pen(Color.Black, 1f);
                AdjustableArrowCap bigArrow = new AdjustableArrowCap(5, 8);
                c.CustomEndCap = bigArrow;
                c.DashStyle = DashStyle.Dash;
                c.DashPattern = new float[] { 4.0f, 4.0f, 4.0f, 4.0f };
                c.DashCap = DashCap.Round;
                g.DrawLine(c, (ListaCaminhos[i].Origem.X * pbMapa.Width) / 4096, (ListaCaminhos[i].Origem.Y * pbMapa.Height) / 2048, (ListaCaminhos[i].Destino.X * pbMapa.Width) / 4096, (ListaCaminhos[i].Destino.Y * pbMapa.Height) / 2048);
            }
        }

        private void ExibirTodosOsCaminhosNoDGV()
        {
            var listLength = new List<int>();

            for (int i = 0; i < Resultados.Count; i++)
            {
                listLength.Add(Resultados[i].Length);
            }

            var maxLength = listLength.Max();

            for (int i = 0; i < maxLength + 1; i++)
            {
                dataGridView1.Columns.Add(i + "", i + "");
            }

            for (int i = 0; i < Resultados.Count; i++)
            {
                dataGridView1.Rows.Add();
                int k = Resultados[i].Length - 1;

                for (int j = 0; j < Resultados[i].Length; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Value = Resultados[i][k].Origem.Nome + " ->";

                    if (j == Resultados[i].Length - 1)
                        dataGridView1.Rows[i].Cells[j + 1].Value = Resultados[i][k].Destino.Nome + "";

                    k--;
                }
            }

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.Width = 127;
            }
        }

        private void GetMelhorCaminho()
        {
            if (radioButton4.Checked)
                ListaMelhorCaminho = Solucionador.BuscarMelhorCaminhoDistancia(Resultados);
            else if (radioButton5.Checked)
                ListaMelhorCaminho = Solucionador.BuscarMelhorCaminhoTempo(Resultados);
            else
                ListaMelhorCaminho = Solucionador.BuscarMelhorCaminhoCusto(Resultados);
        }

        private void GetMelhorCaminhoDijkstra()
        {
            for (int i = 0; i < matrizCaminhos.GetLength(0); i++)
            {
                for (int j = 0; j < matrizCaminhos.GetLength(1); j++)
                {
                    AvancoCaminho ac = matrizCaminhos[i, j];

                    if (ac != null)
                    {
                        if (radioButton4.Checked)
                            SolucionadorDijkstra.NovaAresta(ac.Origem.Id, ac.Destino.Id, ac.Caminho.Distancia);
                        else if (radioButton5.Checked)
                            SolucionadorDijkstra.NovaAresta(ac.Origem.Id, ac.Destino.Id, ac.Caminho.Tempo);
                        else
                            SolucionadorDijkstra.NovaAresta(ac.Origem.Id, ac.Destino.Id, ac.Caminho.Custo);
                    }
                }
            }
        }

        private void ExibirMelhorCaminhoNoDGV()
        {
            dataGridView2.Rows.Clear();
            dataGridView2.Columns.Clear();

            for (int i = 0; i < ListaMelhorCaminho.Length + 1; i++)
            {
                dataGridView2.Columns.Add(i + "", i + "");
            }

            dataGridView2.Rows.Add();

            int k = ListaMelhorCaminho.Length - 1;

            for (int i = 0; i < ListaMelhorCaminho.Length; i++)
            {
                dataGridView2.Rows[0].Cells[i].Value = ListaMelhorCaminho[k].Origem.Nome + " ->";

                if (i == ListaMelhorCaminho.Length - 1)
                    dataGridView2.Rows[0].Cells[i + 1].Value = ListaMelhorCaminho[k].Destino.Nome + "";

                k--;
            }

            foreach (DataGridViewColumn column in dataGridView2.Columns)
            {
                column.Width = 127;
            }
        }

        private void DesenharMelhorCaminho()
        {
            for (int i = 0; i < ListaMelhorCaminho.Length; i++)
            {
                Pen c = new Pen(Color.FromArgb(0, 0, 185), 2);
                AdjustableArrowCap bigArrow = new AdjustableArrowCap(5, 8);
                c.CustomEndCap = bigArrow;
                g.DrawLine(c, (ListaMelhorCaminho[i].Origem.X * pbMapa.Width) / 4096, (ListaMelhorCaminho[i].Origem.Y * pbMapa.Height) / 2048, (ListaMelhorCaminho[i].Destino.X * pbMapa.Width) / 4096, (ListaMelhorCaminho[i].Destino.Y * pbMapa.Height) / 2048);
            }
        }

        private void DesenharCaminhoEspecifico(Color color)
        {
            for (int i = 0; i < listaCaminho.Count; i++)
            {
                Pen c = new Pen(color, 2);
                AdjustableArrowCap bigArrow = new AdjustableArrowCap(5, 8);
                c.CustomEndCap = bigArrow;
                g.DrawLine(c, (listaCaminho[i].Origem.X * pbMapa.Width) / 4096, (listaCaminho[i].Origem.Y * pbMapa.Height) / 2048, (listaCaminho[i].Destino.X * pbMapa.Width) / 4096, (listaCaminho[i].Destino.Y * pbMapa.Height) / 2048);
            }
        }
    }
}
