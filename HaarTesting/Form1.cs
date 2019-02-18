using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;

using System.IO.Ports;
using System.Timers;

using ODE01;

namespace HaarTesting
{
    public partial class Form1 : Form
    {
    	public static SerialPort serialPort1 = new SerialPort();
        /// <summary>
        /// Начало выделения мышкой
        /// </summary>
        Point StartMouseSelection;
        /// <summary>
        /// Начало первого фрагмента
        /// </summary>
        Point StartFragment;
        /// <summary>
        /// Размер фрагмента
        /// </summary>
        Size SizeOfFragment;
        /// <summary>
        /// Триггер захвата первого кадра
        /// </summary>
        Boolean CaptureModeOn = false;
        /// <summary>
        /// Успешно ли захвачен первый элемент
        /// </summary>
        Boolean SomeThingSetAsFirstPic = false;
        /// <summary>
        /// Вектор искомого элемента
        /// </summary>
        List<double> HaarDecompessionMainImage;

        public Form1()
        {
            InitializeComponent();
            //Созхдание каскада из набора примитивов
            #region CreateCascade
            int l = 6; // Размер минимального квадрата паттерна
            // Используемые признаки хаара
            // 0 - белый, * - чёрный
            for (int s = l; s < l*3+1; s += l)
            {
                //000
                //***
                HaarFeatures HF = new HaarFeatures();
                HF.AddWhite(0, 0, s * 3, s);
                HF.AddBlack(0, s, s * 3, s);
                HF.SetSize(s * 3, s * 2);
                Cascade.Add(HF);
                //0*
                //0*
                //0*
                HF = new HaarFeatures();
                HF.AddWhite(0, 0, s, s*3);
                HF.AddBlack(s, 0, s, s*3);
                HF.SetSize(s * 2, s * 3);
                Cascade.Add(HF);
                //0
                //*
                //0
                HF = new HaarFeatures();
                HF.AddWhite(0, 0, s, s);
                HF.AddWhite(0, s*2, s, s);
                HF.AddBlack(0, s, s, s);
                HF.SetSize(s, s * 3);
                Cascade.Add(HF);
                //0*0
                HF = new HaarFeatures();
                HF.AddWhite(0, 0, s, s);
                HF.AddWhite(s*2, 0, s, s);
                HF.AddBlack(s, 0, s, s);
                HF.SetSize(s*3, s);
                Cascade.Add(HF);
                //0*
                //*0
                HF = new HaarFeatures();
                HF.AddWhite(0, 0, s, s);
                HF.AddWhite(s, s, s, s);
                HF.AddBlack(s, 0, s, s);
                HF.AddBlack(0, s, s, s);
                HF.SetSize(s * 2, s*2);
                //000
                //0*0
                //000
                HF = new HaarFeatures();
                HF.AddWhite(0, 0, s*3, s);
                HF.AddWhite(0, s*2, s * 3, s);
                HF.AddWhite(0, s, s, s);
                HF.AddWhite(s*2, s, s, s);
                HF.AddBlack(s, s, s, s);
                HF.SetSize(s * 3, s * 3);
                Cascade.Add(HF);
            }
            #endregion
        }
        private Capture _capture;

        /// <summary>
        /// Отдельный элемент примитива
        /// </summary>
        class Block
        {
            public Point P;
            public Size S;
            public Block(Point p, Size s)
            {
                P = p;
                S = s;
            }
            public Block(int x, int y, int w, int h)
            {
                P = new Point(x, y);
                S = new Size(w, h);
            }
        }
        /// <summary>
        /// Отдельный примитив каскада
        /// </summary>
        class HaarFeatures
        {
            public Size S;
            public List<Block> WhiteHaar = new List<Block>(); //Набор белых примитивов
            public List<Block> BlackHaar = new List<Block>(); //Набор тёмных примитивов
            /// <summary>
            /// Добавить белый примитив
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="w"></param>
            /// <param name="h"></param>
            public void AddWhite(int x, int y, int w, int h)
            {
                WhiteHaar.Add(new Block(x, y, w, h));
            }
            /// <summary>
            /// Добавить белый элемент
            /// </summary>
            /// <param name="p"></param>
            /// <param name="s"></param>
            public void AddWhite(Point p, Size s)
            {
                WhiteHaar.Add(new Block(p,s));
            }
            /// <summary>
            /// Добавить тёмный элемент
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="w"></param>
            /// <param name="h"></param>
            public void AddBlack(int x, int y, int w, int h)
            {
                BlackHaar.Add(new Block(x, y, w, h));
            }
            /// <summary>
            /// Добавить тёмный элемент
            /// </summary>
            /// <param name="p"></param>
            /// <param name="s"></param>
            public void AddBlack(Point p, Size s)
            {
                BlackHaar.Add(new Block(p, s));
            }
            /// <summary>
            /// Установить размер примитива
            /// </summary>
            /// <param name="w"></param>
            /// <param name="h"></param>
            public void SetSize(int w, int h)
            {
                S = new Size(w, h);
            }
            /// <summary>
            /// Установить размер примитива
            /// </summary>
            /// <param name="s"></param>
            public void SetSize(Size s)
            {
                S = s;
            }
        }

        List<HaarFeatures> Cascade = new List<HaarFeatures>();
        /// <summary>
        /// При получении нового кадра OpenCV вызывает эту процедуру
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        
        private void ProcessFrame(object sender, EventArgs arg)
        {
            Image<Gray, Byte> frame = _capture.QueryFrame().Convert<Gray,Byte>(); //Текущий кадр с камеры
            Image<Gray,double> IntegralImage = frame.Integral();  //Интегральное изображение, строим средствами OpenCV
            //Проверяем, нужно ли захватить первый кадр
            if (CaptureModeOn)
            {
                ProccedeFirstElement(IntegralImage); //Для первого элемента сохраняем отдельно
                CaptureModeOn = false;
            }
            else
                if (SomeThingSetAsFirstPic)
                {
                    Point p= ProccedeOtherElement(IntegralImage); //Проводим поиск образца
                    LastCorresponding = p; //Последняя точка где нашли
                    Rectangle Rec = new Rectangle(p, SizeOfFragment); //Рисуем найденный фрагмент
                    frame.Draw(Rec, new Gray(230), 3);
                }
            VideoWindow.Image = frame;
            
            //frame.Width
           label1.Text = "";//"x=" +LastCorresponding.X.ToString()+" y="+LastCorresponding.Y.ToString();
            // label1.Text += " x=" +frame.Width.ToString()+" y="+frame.Height.ToString();
             
            // label1.Text += " x=" +((char)(LastCorresponding.X+SizeOfFragment.Width/2)*256/frame.Width).ToString()+
             	//" y="+((char)(LastCorresponding.Y+SizeOfFragment.Height/2)*256/frame.Height).ToString();
             if(LastCorresponding.Y>0 && SizeOfFragment.Height>0)
             	y = (LastCorresponding.Y+SizeOfFragment.Height/2)*1000/frame.Height-500;
             else
             	y= 0 ;
             if(LastCorresponding.X>0 && SizeOfFragment.Width>0)
             	x = (LastCorresponding.X+SizeOfFragment.Width/2)*1000/frame.Width-500;
             else
             	x= 0 ;
              dt = DateTime.Now.Millisecond-t;
             
            // ax+=x;
             //ay+=y;
             
             
             
             //label1.Text += " ax=" +ax.ToString()+" ay="+ay.ToString();	
             
             //label1.Text += " dt=" +dt.ToString();
             
             label2.Text = str;
             
             t= DateTime.Now.Millisecond;
             
             if(!SomeThingSetAsFirstPic){
		        	x= 0;
		        	y= 0;
             }
             label1.Text += " x = " +x.ToString()+";       y = "+y.ToString() ;	
        }
 		void Button1Click(object sender, EventArgs e)
        {
        	serialPort1.PortName = "COM6"; //Указываем наш порт - в данном случае COM1.
			serialPort1.BaudRate = 115200;//9600; //указываем скорость.
			serialPort1.DataBits = 8; 
			serialPort1.Open(); //Открываем порт.
			serialPort1.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler_);
			str += "Con";
			timer1.Enabled = true;
        }
 		int t = DateTime.Now.Millisecond;
 		int dt = 0;
 		string str = "";
        void DataReceivedHandler_(object sender, System.IO.Ports.SerialDataReceivedEventArgs e){
			SerialPort sp = (SerialPort)sender;
			
			str+= sp.ReadLine()+"\n";
				 
		}
        /// <summary>
        /// Первый элемент сохраняется
        /// </summary>
        /// <param name="Im">Изображение</param>
        private void ProccedeFirstElement(Image<Gray, double> Im)
        {
            //Выделение первого элемента
            HaarDecompessionMainImage = ExtractHaar(StartFragment,SizeOfFragment,Im);
            LastCorresponding = StartFragment; //Последняя точка нахождения объекта
            SomeThingSetAsFirstPic = true; //Первый элемент загружен
        }

        Point LastCorresponding; //Последняя точка нахождения искомого элемента
        /// <summary>
        /// Поиск области в кадре камеры
        /// </summary>
        /// <param name="Im">Изображение</param>
        /// <returns>Точка ЛВУ найденного объекта</returns>
        private Point ProccedeOtherElement(Image<Gray, double> Im)
        {
            double b = double.MaxValue;
            Point InterestingZone = new Point(0, 0); //Итоговый ответ
            List<double> HaarDecompessionMax = new List<double>();
            for (int x = LastCorresponding.X - 100; x < LastCorresponding.X + 100; x+=4)
                for (int y = LastCorresponding.Y - 100; y < LastCorresponding.Y + 100;y+=4)
                {
                    List<double> HaarDecompession = ExtractHaar(new Point(x, y), SizeOfFragment, Im);
                    if (HaarDecompession.Count > 0)
                    {
                        double d = CorrelateTwoDecompression(HaarDecompession, HaarDecompessionMainImage);
                        if (b > d)//Берём точку как можно более похожую 
                        {
                            b = d;
                            InterestingZone = new Point(x, y); // Сохраним её
                            HaarDecompessionMax = new List<double>(HaarDecompession);
                        }
                    }

                }
       
            return InterestingZone;
        }
        /// <summary>
        /// Сравниваем два вектора полученных из каскадов
        /// </summary>
        /// <param name="D1">Первый вектор</param>
        /// <param name="D2">Второй вектор</param>
        /// <returns></returns>
        private double CorrelateTwoDecompression(List<double> D1, List<double> D2)
        {
            //Проверка на случай вдруг что пошло не так
            if (D1.Count == D2.Count)
            {
                double d = 0;
                for (int i = 0; i < D1.Count; i++)
                {
                    d += Math.Abs(D1[i] - D2[i]); //Характеристическим числом близасти векторов будет являтся абсолютная разность между ними
                }
                return d;
            }
            else
                return 0;
        }
        /// <summary>
        /// Получаем хаар репрезентацию области заданного размера
        /// </summary>
        /// <param name="Start">Координаты xy верхнего левого угла области</param>
        /// <param name="S">Размер области</param>
        /// <param name="Im">Большое изображение</param>
        /// <returns></returns>
        public List<double> ExtractHaar(Point Start, Size S, Image<Gray, double> Im)
        {
            /////////////////////////////////
            //Проверяем граничные условия
            //  Перебираем все элементы из каскада
            //    Перебираем все возможные положения элемента по X, Y
            //      Учитываем тёмные элементы
            //      Учитываем светлые элементы
            List<double>Decompression = new List<double>(); //Получаемый Хаар
            //проверяем граничные условия
            if ((Start.X>0)&&(Start.Y>0)&&(Start.X+S.Width<Im.Width)&&(Start.Y+S.Height<Im.Height))
                //Для всех созданных в каскаде объектов
                //Делаем их экстракцию
                for (int i = 0; i < Cascade.Count; i++)
                {
                    for (int x = Start.X; x < Start.X + S.Width - Cascade[i].S.Width; x += Cascade[i].S.Width)
                        for (int y = Start.Y; y < Start.Y + S.Height - Cascade[i].S.Height; y += Cascade[i].S.Height)
                        {
                            double Sum1 = 0;
                            double Sum2 = 0;
                            //Тёмные части паттерна
                            for (int j = 0; j < Cascade[i].BlackHaar.Count; j++)
                            {
                                double subsum = 0;
                                Point xy = Cascade[i].BlackHaar[j].P;
                                Size ss = Cascade[i].BlackHaar[j].S;
                                //////////////////////////////////////
                                //Подсчёт суммы в интересующей области
                                subsum += Im.Data[y + xy.Y + ss.Height, x + xy.X + ss.Width, 0];
                                subsum -= Im.Data[y + xy.Y + ss.Height, x + xy.X, 0];
                                subsum -= Im.Data[y + xy.Y, x + xy.X + ss.Width, 0];
                                subsum += Im.Data[y + xy.Y, x + xy.X, 0];
                                /////////////////////////////////////
                                Sum1 += subsum; //Интеграл по тёмной области
                            }
                            //Светлые части паттерна
                            for (int j = 0; j < Cascade[i].WhiteHaar.Count; j++)
                            {
                                double subsum = 0;
                                Point xy = Cascade[i].WhiteHaar[j].P;
                                Size ss = Cascade[i].WhiteHaar[j].S;
                                //////////////////////////////////////
                                //Подсчёт суммы в интересующей области
                                subsum += Im.Data[y + xy.Y + ss.Height, x + xy.X + ss.Width, 0];
                                subsum -= Im.Data[y + xy.Y + ss.Height, x + xy.X, 0];
                                subsum -= Im.Data[y + xy.Y, x + xy.X + ss.Width, 0];
                                subsum += Im.Data[y + xy.Y, x + xy.X, 0];
                                /////////////////////////////////////
                                Sum2 += subsum; //Интеграл по светлой области
                            }
                            if (Sum2 != 0)
                                Decompression.Add(Sum1 / Sum2); //Сохраняем число, характеризующее элемент
                            else
                                Decompression.Add(0);
                        }
                }
            return Decompression;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                _capture = new Capture(); //Проверка доступа камеры
            }
            catch (NullReferenceException excpt)
            {
                MessageBox.Show(excpt.Message);
            }
            Application.Idle += ProcessFrame; //Привязываем к событию камеры процедуру
        }


        
        private void VideoWindow_MouseDown(object sender, MouseEventArgs e)
        {
            //Захват первого клика мыши
            StartMouseSelection = new Point(e.X, e.Y);
        }

        private void VideoWindow_MouseUp(object sender, MouseEventArgs e)
        {
            //Захват последнего клика, пересчёт координат
            StartFragment = new Point(Math.Min(e.X, StartMouseSelection.X), Math.Min(e.Y, StartMouseSelection.Y));
            SizeOfFragment = new Size(Math.Abs(e.X - StartMouseSelection.X), Math.Abs(e.Y-StartMouseSelection.Y));
            //Так как размер паттернов и их колличество предварительно заложены
            //то изображение должно быть не очень большое
            if (SizeOfFragment.Width > 75)
                SizeOfFragment.Width = 75;
            if (SizeOfFragment.Height > 75)
                SizeOfFragment.Height = 75;
            CaptureModeOn = true;
            
           // timer1.Enabled= true;

        }

 
        
       int x = 0;
       int y = 0;
       int ay = 0;
       int ax = 0;
       
       List<SData> data= new List<SData>();
        
        void Timer1Tick(object sender, EventArgs e)
        {
        	ay+=y;
        	ax+=x;
        	serialPort1.WriteLine("A"+ (-x/10).ToString() );//"par dRoll "
        	serialPort1.WriteLine("B"+ (y/10).ToString() );//"par dPitch "
        	//
        	data.Add( new SData(x,y,20) );
        	//i+=100;
        }
        
        class SData{
        	public int x;
        	public int y;
        	public int dt;
        	public SData(int X, int Y, int dT){
        		x = X;
        		y=Y;
        		dt = dT;
        	}
        }
        //
        
        void Button2Click(object sender, EventArgs e)
        {
        	SomeThingSetAsFirstPic = false;
        	x= 0;
        	y= 0;
        	timer1.Enabled= false;
        }
        
        
        void Button3Click(object sender, EventArgs e)
        {
        	double[] xx = new double[data.Count];
        	double[] yy = new double[data.Count];
        	double[] tt = new double[data.Count];
        	
        	int j = 0;
        	foreach (SData prime in data) // Loop through List with foreach
			{
        		xx[j] = prime.x;
        		yy[j] = prime.y;
        		tt[j] = 0.020*j;
			   j++;
			}
        	
        	CDataIO io=new CDataIO();
        	io.WriteArrayToFile("x.dat",(uint)data.Count,xx);
        	io.WriteArrayToFile("y.dat",(uint)data.Count,yy);
        	io.WriteArrayToFile("t.dat",(uint)data.Count,tt);
        	
        	io.ShowDataImager("DataImager2.2.exe","t x");
        	//io.ShowDataImager("DataImager2.2.exe","t y");
        }
    }
    
}
