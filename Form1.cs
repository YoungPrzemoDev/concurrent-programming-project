using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace PODEJSCIE_1
{
    public partial class Form1 : Form
    {
        //----------------Miejsca------------
        Point startPoint;
        Point destinationPoint1;
        Point destinationPoint2;
        Point destinationPoint3;
        Point destinationPoint4; 
        Point destinationPoint5; 

        Point[] kolejakPointDo_1_Kasy = new Point[4];
        Point[] kolejakPointDo_2_Kasy = new Point[4];
        Point[] kolejakPointDo_3_Kasy = new Point[4];

        //---------------------Inicjalizacja paneli--------------
        public Form1()
        {
            InitializeComponent();
            startPoint = start_Panel.Location;
            destinationPoint1 = Kasa1_Panel.Location;
            destinationPoint2 = Kasa2_Panel.Location;
            destinationPoint3 = Kasa3_Panel.Location;
            destinationPoint4 = Panel_Korytarz_2Pietro.Location;
            destinationPoint5 = Panel_Korytarz_3Pietro.Location;

            kolejakPointDo_1_Kasy[3] = Panel_4_kolejka_1_pietro.Location;
            kolejakPointDo_1_Kasy[2] = Panel_3_kolejka_1_pietro.Location;
            kolejakPointDo_1_Kasy[1] = Panel_2_kolejka_1_pietro.Location;
            kolejakPointDo_1_Kasy[0] = Panel_1_kolejka_1_pietro.Location;

            kolejakPointDo_2_Kasy[3] = Panel_4_kolejka_2_pietro.Location;
            kolejakPointDo_2_Kasy[2] = Panel_3_kolejka_2_pietro.Location;
            kolejakPointDo_2_Kasy[1] = Panel_2_kolejka_2_pietro.Location;
            kolejakPointDo_2_Kasy[0] = Panel_1_kolejka_2_pietro.Location;

            kolejakPointDo_3_Kasy[3] = Panel_4_kolejka_3_pietro.Location;
            kolejakPointDo_3_Kasy[2] = Panel_3_kolejka_3_pietro.Location;
            kolejakPointDo_3_Kasy[1] = Panel_2_kolejka_3_pietro.Location;
            kolejakPointDo_3_Kasy[0] = Panel_1_kolejka_3_pietro.Location;
        }

       
        int counter_max_klientow = 0;
        int counter_kasa_1 = 0;
        int counter_kasa_2 = 0;
        int counter_kasa_3 = 0;

        int licznik_czasu1 = 15;
        int licznik_czasu2 = 25;
        int licznik_czasu3 = 45;

        List<Task> tasks = new List<Task>();
        Queue<PictureBox> kolejka_Klientwo = new Queue<PictureBox>();

        //-----------------------Semafory-------------------------
        SemaphoreSlim Ekran_podrozy = new SemaphoreSlim(1,2); 
        
        SemaphoreSlim kas1 = new SemaphoreSlim(1, 1);
        SemaphoreSlim kas2 = new SemaphoreSlim(1, 1);
        SemaphoreSlim kas3 = new SemaphoreSlim(1, 1);
        

        SemaphoreSlim kolejka_1_pietro_1 = new SemaphoreSlim(1, 1);
        SemaphoreSlim kolejka_1_pietro_2 = new SemaphoreSlim(1, 1);
        SemaphoreSlim kolejka_1_pietro_3 = new SemaphoreSlim(1, 1);
        SemaphoreSlim kolejka_1_pietro_4 = new SemaphoreSlim(1, 1);

        SemaphoreSlim kolejka_2_pietro_1 = new SemaphoreSlim(1, 1);
        SemaphoreSlim kolejka_2_pietro_2 = new SemaphoreSlim(1, 1);
        SemaphoreSlim kolejka_2_pietro_3 = new SemaphoreSlim(1, 1);
        SemaphoreSlim kolejka_2_pietro_4 = new SemaphoreSlim(1, 1);

        SemaphoreSlim kolejka_3_pietro_1 = new SemaphoreSlim(1, 1);
        SemaphoreSlim kolejka_3_pietro_2 = new SemaphoreSlim(1, 1);
        SemaphoreSlim kolejka_3_pietro_3 = new SemaphoreSlim(1, 1);
        SemaphoreSlim kolejka_3_pietro_4 = new SemaphoreSlim(1, 1);

        //--------------Monitory---------------------------
        /*static object dock1_lock = new object();
        static object dock2_lock = new object();
        static object dock3_lock = new object();
        static object lockmiejsca = new object();*/


        public PictureBox spawnKlient()
        {
            
            PictureBox klient = new PictureBox();
            klient.Size = new System.Drawing.Size(66, 66);
            klient.Visible = true;
            klient.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            klient.Image = Properties.Resources.klient;


            klient.Location = startPoint;
            this.Invoke((MethodInvoker)delegate
            {
                kolejka_Klientwo.Enqueue(klient);
                this.Controls.Add(klient);
                klient.BringToFront();
            });
            
            return klient;
        }

        private int Wybor_kasy(int a,int b)
        {
            Random rand = new Random();
            int wybor = rand.Next(a,b);
            return wybor; 
        }
        public int Wybor_kasy(int a, int b, int except)
        {
            Random rand = new Random();
            int result = rand.Next(a, b);
            if (result == except) result -= 1;
            return result;
        }



        public bool przerwa1()
        {

            
            if (licznik_czasu1 <= 0)
            {
                
                return true;
            }
            return false;
        }

        public bool przerwa2()
        {
            
            if (licznik_czasu2 <=0)
            {

                return true;
            }
            return false;
        }
        public bool przerwa3()
        {
            
            if (licznik_czasu3 <= 0)
            {
                /*licznik_czasu1 = 35;*/
                return true;
            }
            return false;
        }

        private  void Nowy_Klient_Button_Click(object sender, EventArgs e)
        {
            
            if (counter_max_klientow < 10)
            {

                counter_max_klientow++;
                licznik_czasu1--;
                licznik_czasu2--;
                licznik_czasu3--;
                bool wcisniety = true;
                int wybor;

                if (licznik_czasu1 <=0) { wybor = Wybor_kasy(2, 4); }
                else if (licznik_czasu2 <=0) { wybor = Wybor_kasy(1, 4, 2); }
                else if (licznik_czasu3 <=0) { wybor = Wybor_kasy(1, 3); }

                else { wybor = Wybor_kasy(1, 4); }

                if (wybor == 1) { Kasa1.Checked = wcisniety; }
                else if (wybor == 2) { Kasa2.Checked = wcisniety; }
                else if (wybor == 3) { Kasa3.Checked = wcisniety; }



                tasks.Add(Task.Run(() =>
                {
                    przyjecieKlientow(wybor, spawnKlient());

                }));
                 
                /*await Task.WhenAll(tasks);*/
            }
            

        }

        
        public void przyjecieKlientow(int wybor, PictureBox klient)
        {
            klient = kolejka_Klientwo.Dequeue();
            
            switch (wybor)
            {
                case 1:
                    {

                        counter_kasa_1++;
                        if (counter_kasa_1 > 0)
                        {
                            przyjecie_1_Kasa(klient);
                            counter_kasa_1--;
                        }
                        if(counter_kasa_1 == 0 & licznik_czasu1<=0)
                        {
                            
                            pokaz_Tekst(Przerwa);
                            Thread.Sleep(5000);
                            pokaz_Tekst(Przerwa);


                            licznik_czasu1 = 15;
                            /*pokaz_Tekst(Wrocilam);*/

                        }


                        break;
                    }
                case 2:
                    {
                        counter_kasa_2++;
                        if(counter_kasa_2 > 0)
                        {
                            przyjecie_2_Kasa(klient);
                            counter_kasa_2--;
                        }
                        if (counter_kasa_2 == 0 & licznik_czasu2 <= 0)
                        {
                            pokaz_Tekst(Przerwa_2);
                            Thread.Sleep(5000);
                            pokaz_Tekst(Przerwa_2);
                            licznik_czasu2 = 25;
                            /*pokaz_Tekst(Wrocilam_2);*/

                        }

                        break;
                    }
                case 3:
                    {
                        counter_kasa_3++;
                        if (counter_kasa_3 > 0)
                        {
                            przyjecie_3_Kasa(klient);
                            counter_kasa_3--;
                        }
                        if (counter_kasa_3 == 0 & licznik_czasu3 <= 0) 
                        {
                            pokaz_Tekst(Przerwa_3);
                            Thread.Sleep(5000);
                            pokaz_Tekst(Przerwa_3);
                            licznik_czasu3 = 35;
                        }

                        break;


                    }
            }

        }

        public void zmien_kolejke_1_Pietra(PictureBox klient)
        {
            kolejka_1_pietro_1.Wait();
            moveKlient(kolejakPointDo_1_Kasy[0], klient);
            kolejka_1_pietro_2.Wait();
            kolejka_1_pietro_1.Release();   
            moveKlient(kolejakPointDo_1_Kasy[1], klient);
            kolejka_1_pietro_3.Wait();
            kolejka_1_pietro_2.Release();
            moveKlient(kolejakPointDo_1_Kasy[2], klient);
            kolejka_1_pietro_4.Wait();
            kolejka_1_pietro_3.Release();
            moveKlient(kolejakPointDo_1_Kasy[3], klient);
        }

        public void zmien_kolejke_2_Pietra(PictureBox klient)
        {
            kolejka_2_pietro_1.Wait();
            moveKlient(kolejakPointDo_2_Kasy[0], klient);
            kolejka_2_pietro_2.Wait();
            kolejka_2_pietro_1.Release();
            moveKlient(kolejakPointDo_2_Kasy[1], klient);
            kolejka_2_pietro_3.Wait();
            kolejka_2_pietro_2.Release();
            moveKlient(kolejakPointDo_2_Kasy[2], klient);
            kolejka_2_pietro_4.Wait();
            kolejka_2_pietro_3.Release();
            moveKlient(kolejakPointDo_2_Kasy[3], klient);
        }

        public void zmien_kolejke_3_Pietra(PictureBox klient)
        {
            kolejka_3_pietro_1.Wait();
            moveKlient(kolejakPointDo_3_Kasy[0], klient);
            kolejka_3_pietro_2.Wait();
            kolejka_3_pietro_1.Release();
            moveKlient(kolejakPointDo_3_Kasy[1], klient);
            kolejka_3_pietro_3.Wait();
            kolejka_3_pietro_2.Release();
            moveKlient(kolejakPointDo_3_Kasy[2], klient);
            kolejka_3_pietro_4.Wait();
            kolejka_3_pietro_3.Release();
            moveKlient(kolejakPointDo_3_Kasy[3], klient);
        }


        public void przyjecie_1_Kasa(PictureBox klient)   
        {

            zmien_kolejke_1_Pietra(klient);

            kas1.Wait();

            moveKlient(destinationPoint1, klient);

            kolejka_1_pietro_4.Release();

            Task.Delay(1000).Wait();

            pokaz_przycisk(KlientCheckBox1);

            wyjdzOdkasy_1(klient);

            pokaz_przycisk(KlientCheckBox1);
            

        }


        public void przyjecie_2_Kasa(PictureBox klient)    
        {
            zmien_kolejke_2_Pietra(klient);

            kas2.Wait();

            moveKlient(destinationPoint2, klient);
        
            kolejka_2_pietro_4.Release();

            Task.Delay(1000).Wait();

            pokaz_przycisk(KlienttCheckBox2);

            wyjdzOdkasy_2(klient);

            pokaz_przycisk(KlienttCheckBox2);

        }

        public void przyjecie_3_Kasa(PictureBox klient)   
        {
            zmien_kolejke_3_Pietra(klient);

            kas3.Wait();

            moveKlient(destinationPoint3, klient);

            kolejka_3_pietro_4.Release();

            Task.Delay(1000).Wait();

            pokaz_przycisk(KlientCheckBox3);

            wyjdzOdkasy_3(klient);

            pokaz_przycisk(KlientCheckBox3);

        }

        public void wyjdzOdkasy_1(PictureBox klient) 
        {
            
            moveKlient(startPoint, klient);
            counter_max_klientow--;

            kas1.Release();
            
            removeKlient(klient);
        }
        public void wyjdzOdkasy_2(PictureBox klient)
        {
            
            moveKlient(destinationPoint4, klient);
            moveKlient(startPoint, klient);

            counter_max_klientow--;

            kas2.Release();
            
            removeKlient(klient);
        }

        public void wyjdzOdkasy_3(PictureBox klient)
        {
            moveKlient(destinationPoint5, klient);
            moveKlient(startPoint, klient);
            counter_max_klientow--;
            kas3.Release();
            removeKlient(klient);
        }

        private void pokaz_przycisk(CheckBox przycisk)
        {
            przycisk.Invoke((MethodInvoker)delegate {
                przycisk.Visible = !przycisk.Visible;
            });
        }

        private void pokaz_Tekst(TextBox tekst)
        {
            tekst.Invoke((MethodInvoker)delegate {
                tekst.Visible = !tekst.Visible;
            });
        }


        public void removeKlient(PictureBox klient)
        {
            klient.Invoke((MethodInvoker)delegate
            {
                klient.Dispose();
            });
        }


        private void moveKlient(Point destination,PictureBox klient)
        {
            Ekran_podrozy.Wait();
            Point p;
            if (klient.Location.Y <= destination.Y)
            {
                while (klient.Location.Y < destination.Y)
                {
                    p = new Point(klient.Location.X, klient.Location.Y + 1);
                    klient.Invoke((MethodInvoker)delegate {
                        klient.Location = p;
                    });

                }
            }
            else
            {
                while (klient.Location.Y > destination.Y)
                {
                    p = new Point(klient.Location.X, klient.Location.Y - 1);
                    klient.Invoke((MethodInvoker)delegate {
                        klient.Location = p;
                    });
                }
            }

            if (klient.Location.X <= destination.X)
            {
                while (klient.Location.X < destination.X)
                {
                    p = new Point(klient.Location.X + 1, klient.Location.Y);
                    klient.Invoke((MethodInvoker)delegate {
                        klient.Location = p;
                    });

                }

            }
            else
            {
                while (klient.Location.X > destination.X)
                {
                    p = new Point(klient.Location.X - 1, klient.Location.Y);
                    klient.Invoke((MethodInvoker)delegate {
                        klient.Location = p;
                    });
                }
            }
            if (klient.Location.Y <= destination.Y)
            {
                while (klient.Location.Y < destination.Y)
                {
                    p = new Point(klient.Location.X, klient.Location.Y + 1);
                    klient.Invoke((MethodInvoker)delegate {
                        klient.Location = p;
                    });
                }
            }
            else
            {
                while (klient.Location.Y > destination.Y)
                {
                    p = new Point(klient.Location.X, klient.Location.Y - 1);
                    klient.Invoke((MethodInvoker)delegate {
                        klient.Location = p;
                    });

                }
            }
            Ekran_podrozy.Release();
        }

        private void Przerwa_2_TextChanged(object sender, EventArgs e)
        {

        }
    }

}
