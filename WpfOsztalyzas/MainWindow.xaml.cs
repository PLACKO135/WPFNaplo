using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Microsoft.Win32;
namespace WpfOsztalyzas
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string fajlNev = "naplo.txt";
        //Így minden metódus fogja tudni használni.
        ObservableCollection<Osztalyzat> jegyek = new ObservableCollection<Osztalyzat>();

        public MainWindow()
        {
            InitializeComponent();
           
            OpenFileDialog open = new OpenFileDialog();
          
            if ((bool)open.ShowDialog()! && open.FileName.EndsWith(".csv"))
            {
                fajlNev = open.FileName;
            }
            using(StreamReader sr = new StreamReader(fajlNev))
            {
                while(!sr.EndOfStream)
                {
                    
                        string[] split = sr.ReadLine()!.Split(";");
                        jegyek.Add(new Osztalyzat(split[0], split[1], split[2], Convert.ToInt32(split[^1])));
                    
                    
                }
            }
     
            dgJegyek.ItemsSource = jegyek;
            FilePath_txt.Text = fajlNev;
            Grades_txt.Text = $"Jegyek száma: {jegyek.Count()}, Jegyek átlaga: {jegyek.Average(x => x.Jegy):.0}";
        }

        private void btnRogzit_Click(object sender, RoutedEventArgs e)
        {
            
            string[] split_name = txtNev.Text.Split(" ");
            if (split_name.Count() == 1)
            {
                MessageBox.Show("A névnek legalább 2 szóból kell állnia", "Hibás név", MessageBoxButton.OK);
                return;
            }
            if (Array.FindAll(split_name, x => x.Count() < 3).ToArray().Count() > 0)
            {
                MessageBox.Show("A névnek legalább 3 karakterből kell állnia szavanként", "Hibás név", MessageBoxButton.OK);
                return;
            }
            if (DateTime.Compare(datDatum.SelectedDate!.Value,DateTime.Now) > 0) 
            {
                MessageBox.Show("Nem lehet jövőbeli dátum!","Hibás dátum",MessageBoxButton.OK);
            }
           



            //A CSV szerkezetű fájlba kerülő sor előállítása
            string csvSor = $"{txtNev.Text};{datDatum.Text};{cboTantargy.Text};{sliJegy.Value}";
            //Megnyitás hozzáfűzéses írása (APPEND)
            StreamWriter sw = new StreamWriter(fajlNev, append: true);
            sw.WriteLine(csvSor);
            sw.Close();
           
            jegyek.Add(new Osztalyzat(txtNev.Text,datDatum.Text,cboTantargy.Text,Convert.ToInt32(sliJegy.Value)));
            Grades_txt.Text = $"Jegyek száma: {jegyek.Count()}, Jegyek átlaga: {jegyek.Average(x => x.Jegy):.0}";

        }

        private void btnBetolt_Click(object sender, RoutedEventArgs e)
        {
            jegyek.Clear();  //A lista előző tartalmát töröljük
            StreamReader sr = new StreamReader(fajlNev); //olvasásra nyitja az állományt
            while (!sr.EndOfStream) //amíg nem ér a fájl végére
            {
                string[] mezok = sr.ReadLine()!.Split(";"); //A beolvasott sort feltördeli mezőkre
                //A mezők értékeit felhasználva létrehoz egy objektumot
                Osztalyzat ujJegy = new Osztalyzat(mezok[0], mezok[1], mezok[2], int.Parse(mezok[3])); 
                jegyek.Add(ujJegy); //Az objektumot a lista végére helyezi
            }
            sr.Close(); //állomány lezárása

            //A Datagrid adatforrása a jegyek nevű lista lesz.
            //A lista objektumokat tartalmaz. Az objektumok lesznek a rács sorai.
            //Az objektum nyilvános tulajdonságai kerülnek be az oszlopokba.
            dgJegyek.ItemsSource = jegyek;
            Grades_txt.Text = $"Jegyek száma: {jegyek.Count()}, Jegyek átlaga: {jegyek.Average(x => x.Jegy):.0}";
        }


        private void Nevcsere(object? sender, RoutedEventArgs e)
        {
            foreach(Osztalyzat osztalyzat in  jegyek)
            {
                osztalyzat.ForditottNev();
            }
            dgJegyek.Items.Refresh();
        }
    }
}

