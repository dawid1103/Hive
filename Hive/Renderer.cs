using System.Drawing;

namespace HiveSymulator
{
    public class Renderer
    {
        private World _world;
        private HiveForm _hiveForm;
        private FieldForm _fieldForm;

        private Bitmap _hiveInside;
        private Bitmap _hiveOutside;
        private Bitmap _flower;
        private Bitmap[] _beeAnimationSmall;
        private Bitmap[] _beeAnimationLarge;
        private int _cell = 0;
        private int _frame = 0;

        public Renderer(World world, HiveForm hiveForm, FieldForm fieldForm)
        {
            _world = world;
            _hiveForm = hiveForm;
            _fieldForm = fieldForm;
            _hiveForm.renderer = this;
            _fieldForm.renderer = this;
            InitializeImages();
        }

        public static Bitmap ResizeImage(Bitmap picture, int width, int height)
        {
            Bitmap resizedPicture = new Bitmap(width, height);
            using (Graphics graphic = Graphics.FromImage(resizedPicture))
            {
                graphic.DrawImage(picture, 0, 0, width, height);
            }
            return resizedPicture;
        }

        public void AnimateBees()
        {
            _frame++;
            if (_frame >= 6)
                _frame = 0;
            switch (_frame)
            {
                case 0:
                    _cell = 0;
                    break;
                case 1:
                    _cell = 1;
                    break;
                case 2:
                    _cell = 2;
                    break;
                case 3:
                    _cell = 3;
                    break;
                case 4:
                    _cell = 2;
                    break;
                case 5:
                    _cell = 1;
                    break;
                default:
                    _cell = 0;
                    break;
            }
            _hiveForm.Invalidate();
            _fieldForm.Invalidate();
        }

        private void InitializeImages()
        {
            _hiveOutside = ResizeImage(Properties.Resources.Hive__outside_, 85, 100);
            _hiveInside = ResizeImage(Properties.Resources.Hive__inside_, _hiveForm.ClientRectangle.Width, _hiveForm.ClientRectangle.Height);
            _flower = ResizeImage(Properties.Resources.Flower, 75, 75);

            _beeAnimationLarge = new Bitmap[4];
            _beeAnimationLarge[0] = ResizeImage(Properties.Resources.Bee_animation_1, 40, 40);
            _beeAnimationLarge[1] = ResizeImage(Properties.Resources.Bee_animation_2, 40, 40);
            _beeAnimationLarge[2] = ResizeImage(Properties.Resources.Bee_animation_3, 40, 40);
            _beeAnimationLarge[3] = ResizeImage(Properties.Resources.Bee_animation_4, 40, 40);

            _beeAnimationSmall = new Bitmap[4];
            _beeAnimationSmall[0] = ResizeImage(Properties.Resources.Bee_animation_1, 40, 40);
            _beeAnimationSmall[1] = ResizeImage(Properties.Resources.Bee_animation_2, 40, 40);
            _beeAnimationSmall[2] = ResizeImage(Properties.Resources.Bee_animation_3, 40, 40);
            _beeAnimationSmall[3] = ResizeImage(Properties.Resources.Bee_animation_4, 40, 40);
        }

        public void PaintHive(Graphics g)
        {
            g.FillRectangle(Brushes.SkyBlue, _hiveForm.ClientRectangle);
            g.DrawImageUnscaled(_hiveInside, 0, 0);
            foreach (Bee bee in _world.Bees)
            {
                if (bee.InsideHive)
                    g.DrawImageUnscaled(_beeAnimationLarge[_cell], bee.Location.X, bee.Location.Y);
            }
        }

        public void PaintField(Graphics g)
        {
            using (Pen brownPen = new Pen(Color.Brown, 6.0F))
            {
                g.FillRectangle(Brushes.SkyBlue, 0, 0, _fieldForm.ClientRectangle.Width, _fieldForm.ClientRectangle.Height);
                g.FillEllipse(Brushes.Yellow, new RectangleF(50, 15, 70, 70));
                g.FillRectangle(Brushes.Green, 0, _fieldForm.ClientRectangle.Height / 2, _fieldForm.ClientRectangle.Width, _fieldForm.ClientRectangle.Height / 2);
                g.DrawLine(brownPen, new Point(593, 0), new Point(593, 30));
                g.DrawImageUnscaled(_hiveOutside, 550, 20);
                foreach (Flower flower in _world.Flowers)
                {
                    g.DrawImageUnscaled(_flower, flower.Location.X, flower.Location.Y);
                }

                foreach (Bee bee in _world.Bees)
                {
                    if (!bee.InsideHive)
                    {
                        g.DrawImageUnscaled(_beeAnimationSmall[_cell], bee.Location.X, bee.Location.Y);
                    }
                }
            }
        }
    }
}