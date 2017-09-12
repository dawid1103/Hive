using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace HiveSymulator
{
    public partial class Form1 : Form
    {
        private World _world;
        private Random _random = new Random();
        private DateTime _start = DateTime.Now;
        private DateTime _end;
        private int _framesRun = 0;
        private HiveForm _hiveForm = new HiveForm();
        private FieldForm _fieldForm = new FieldForm();
        private Renderer _renderer;

        public Form1()
        {
            MoveChildForms();
            _hiveForm.Show(this);
            _fieldForm.Show(this);
            ResetSimulator();
            InitializeComponent();

            timer1.Interval = 10;
            timer1.Tick += new EventHandler(RunFrame);
            timer1.Enabled = false;
            UpdateStats(new TimeSpan());
        }

        private void MoveChildForms()
        {
            _hiveForm.Location = new Point(Location.X + Width + 10, Location.Y);
            _fieldForm.Location = new Point(Location.X, Location.Y + Math.Max(Height, _hiveForm.Height) + 10);
        }

        private void ResetSimulator()
        {
            _framesRun = 0;
            _world = new World(new BeeMessage(SendMessage));
            _renderer = new Renderer(_world, _hiveForm, _fieldForm);
        }

        private void UpdateStats(TimeSpan frameDuration)
        {
            lblBees.Text = _world.Bees.Count.ToString();
            lblFlowers.Text = _world.Flowers.Count.ToString();
            lblHoneyInHive.Text = string.Format("{0:f3}", _world.Hive.Honey);
            double nectar = 0;
            foreach (Flower flower in _world.Flowers)
            {
                nectar += flower.Nectar;
            }
            lblNectarInFlowers.Text = string.Format("{0:f3}", nectar);
            lblFramesRun.Text = _framesRun.ToString();
            double miliSeconds = frameDuration.TotalMilliseconds;
            if (miliSeconds != 0.0)
            {
                lblFrameRate.Text = string.Format("{0:f0} ({1:f1}ms)", 1000 / miliSeconds, miliSeconds);
            }
            else
            {
                lblFrameRate.Text = "Brak";
            }
        }

        public void RunFrame(object sender, EventArgs e)
        {
            _framesRun++;
            _world.Go(_random);
            _end = DateTime.Now;
            TimeSpan framesDuration = _end - _start;
            _start = _end;
            UpdateStats(framesDuration);
            _hiveForm.Invalidate();
            _fieldForm.Invalidate();
        }

        private void tsbStartSimulation_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
            {
                toolStrip1.Items[0].Text = "Wznów symulację";
                timer1.Stop();
            }
            else
            {
                toolStrip1.Items[0].Text = "Zatrzymaj symulację";
                timer1.Start();
            }
        }

        private void tsbRestart_Click(object sender, EventArgs e)
        {
            ResetSimulator();

            if (!timer1.Enabled)
                toolStrip1.Items[0].Text = "Rozpocznij symulację";
        }

        private void SendMessage(int ID, string message)
        {
            statusStrip1.Items[0].Text = string.Format("Pszczoła nr {0} : {1}", ID, message);
            //var beeGroups = from bee in _world.Bees group bee by bee.CurrentState into beeGroup orderby beeGroup.Key select beeGroup;
            var beeGroups = _world.Bees.GroupBy(_ => _.CurrentState).OrderBy(o => o.Key);
            listBox.Items.Clear();
            foreach (var group in beeGroups)
            {
                string s;
                if (group.Count() == 1)
                    s = "pszczoła";
                else if (group.Count() > 4)
                    s = "pszczół";
                else
                    s = "pszczoły";

                string stringState;
                switch (group.Key)
                {
                    case BeeState.FlyingToFlower:
                        stringState = "Lot w kierunku kwiatów";
                        break;
                    case BeeState.GatheringNectar:
                        stringState = "Zbieranie nektaru";
                        break;
                    case BeeState.ReturningToHive:
                        stringState = "Powrót do ula";
                        break;
                    case BeeState.MakingHoney:
                        stringState = "Wytwarzanie miodu";
                        break;
                    case BeeState.Retired:
                        stringState = "Na emeryturze";
                        break;
                    default:
                        stringState = "Bezrobocie";
                        break;
                }

                listBox.Items.Add(stringState + ": " + group.Count() + " " + s);
                if (group.Key == BeeState.Idle && group.Count() == _world.Bees.Count() && _framesRun > 0)
                {
                    listBox.Items.Add("Symulacja zakończona: wszystkie pszczoły są bezrobotne.");
                    toolStrip1.Items[0].Text = "Symulacja zakończona";
                    statusStrip1.Items[0].Text = "Symulacja zakończona!";
                    timer1.Enabled = false;
                }
            }
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            bool enabled = timer1.Enabled;
            if (enabled)
                timer1.Stop();

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Plik symulatora (*.bees)|*.bees";
            saveDialog.CheckPathExists = true;
            saveDialog.Title = "Wybierz plik do zapisania bieżącej symulacji.";
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    using (Stream fileToWrite = File.OpenWrite(saveDialog.FileName))
                    {
                        binaryFormatter.Serialize(fileToWrite, _world);
                        binaryFormatter.Serialize(fileToWrite, _framesRun);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Nie można zapisać pliku symulatora.\r\n" + ex.Message,
                    "Błąd symulatora ula.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (enabled)
                    timer1.Start();
            }
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            World currentWorld = _world;
            int currentFramesRun = _framesRun;

            bool enabled = timer1.Enabled;
            if (enabled)
                timer1.Stop();

            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Plik symulatora (*.bees)|*.bees";
            openDialog.CheckPathExists = true;
            openDialog.CheckFileExists = true;

            openDialog.Title = "Wybierz plik z symulacją do odczytu.";
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    using (Stream fileToRead = File.OpenRead(openDialog.FileName))
                    {
                        _world = (World)binaryFormatter.Deserialize(fileToRead);
                        _framesRun = (int)binaryFormatter.Deserialize(fileToRead);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Nie można odczytać pliku symulatora.\r\n" + ex.Message,
                     "Błąd symulatora ula.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _world = currentWorld;
                    _framesRun = currentFramesRun;
                }
            }

            _world.Hive.MessageSender = new BeeMessage(SendMessage);

            foreach (Bee bee in _world.Bees)
                bee.MessageSender = new BeeMessage(SendMessage);
            if (enabled)
                timer1.Start();

            _renderer = new Renderer(_world, _hiveForm, _fieldForm);
        }

        private void Form1_Move(object sender, EventArgs e)
        {
            MoveChildForms();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            _renderer.AnimateBees();
        }
    }
}
