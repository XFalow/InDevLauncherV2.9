using Microsoft.Win32;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace InDevLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string gamePath { get; set; }
        public string pathInDev = $@"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\AppData\Roaming\IndevLauncher\";
        bool isCheck = false;

#pragma warning disable CS8618
        public MainWindow()
#pragma warning restore CS8618
        {
            InitializeComponent();

            gamename.Content = "Selectionne un Logiciel";
            IMG_INDEV.Visibility = Visibility.Visible;

            HideAddGameForm();
            HideSettings();

            ReadGameAtStart();

            SetActu();

            cbbtest.SelectedIndex = Settings.Default.lang;

        }

        public void SetActu()
        {
            try
            {
                string url = "https://indevs.000webhostapp.com/misc/actu.txt";
                HttpClient client = new HttpClient();
                {
                    using (HttpResponseMessage reponse = client.GetAsync(url).Result)
                    {
                        using (HttpContent content = reponse.Content)
                        {
                            var json = content.ReadAsStringAsync().Result;
                            txt.Text = json;
                        }
                    }
                }
            }
            catch
            {
                File.AppendAllText($"{pathInDev}actuClient.txt", "");
                txt.Text = File.ReadAllText($"{pathInDev}actuClient.txt");
            }
        }

        public void ReadGameAtStart()
        {
            try
            {
                string[] lines = File.ReadAllLines($@"{pathInDev}Data.txt");
                foreach (string s in lines)
                {
                    if (s.StartsWith("- "))
                    {
                        string[] gamedata_path = s.Split("|");
                        combo_b_game.Items.Add(gamedata_path[0]);
                    }
                }
            }
            catch
            {
                Directory.CreateDirectory(pathInDev);
                File.WriteAllText($@"{pathInDev}Data.txt", $"");
                MessageBox.Show("Création du fichier Data...");
            }
        }



        private void play_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(gamePath);
            }
            catch
            {
                MessageBoxResult result = MessageBox.Show($"Le Chemin vers l'executable n'est pas correct :\n - Vous pouvez le Modifier dans le dossier :\n\nC:/Users/{Environment.UserName}/AppData/Roaming/IndevLauncher/Data.txt\n\n - Puis redermarer le launcher en attente de Mise a jour sur le sujet...\nOuvrir l'explorer a cette endroit ?", "ExePath invalide", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        string unevar = $@"{pathInDev}";
                        Process.Start("explorer.exe", string.Format("/select,\"{0}\"", unevar));
                        break;

                    case MessageBoxResult.No:
                        break;
                }
            }

        }


        public void HideAddGameForm()
        {
            NomDuJeu.Visibility = Visibility.Hidden;
            PathToExe.Visibility = Visibility.Hidden;
            GAME_NAME_FIELD.Visibility = Visibility.Hidden;
            PATH_TO_EXE_FIELD.Visibility = Visibility.Hidden;
            Btn_save.Visibility = Visibility.Hidden;
            Btn_cancel.Visibility = Visibility.Hidden;
            uploadBtn.Visibility = Visibility.Hidden;

        }

        public void ShowAddGameForm()
        {
            NomDuJeu.Visibility = Visibility.Visible;
            PathToExe.Visibility = Visibility.Visible;
            GAME_NAME_FIELD.Visibility = Visibility.Visible;
            PATH_TO_EXE_FIELD.Visibility = Visibility.Visible;
            Btn_save.Visibility = Visibility.Visible;
            Btn_cancel.Visibility = Visibility.Visible;
            uploadBtn.Visibility = Visibility.Visible;

        }

        private void Btn_Add_Game_Click(object sender, RoutedEventArgs e)
        {
            GAME_NAME_FIELD.Clear();
            PATH_TO_EXE_FIELD.Clear();
            ShowAddGameForm();
        }


        private void Btn_save_Click(object sender, RoutedEventArgs e)
        {
            string GameName = GAME_NAME_FIELD.Text;
            string GamePath = PATH_TO_EXE_FIELD.Text;

            HideAddGameForm();

            string gameData = $@"{pathInDev}Data.txt";
            File.AppendAllText(gameData, $"- {GameName} -|{GamePath}\n");

            combo_b_game.Items.Add($"- {GameName} -");

        }


        private void combo_b_game_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HideAddGameForm();
            Object selectedItem = combo_b_game.SelectedItem;

            string[] lines = File.ReadAllLines($@"{pathInDev}Data.txt"); //ya une erreur quand on supprime un element dans GameData.txt (soit refresh la page chaque seconde / soit fermer le launcher et le redemarer)
            foreach (string s in lines)
            {
                try
                {

    #pragma warning disable CS8604
                    if (s.StartsWith(Convert.ToString(selectedItem)))
                    {
                        string[] gamedata_path = s.Split("|");
                        label_Pathexe.Content = gamedata_path[1];
                        gamename.Content = gamedata_path[0];
                        gamePath = gamedata_path[1];

                    }

    #pragma warning restore CS8604
                }
                catch {
                    MessageBox.Show("erreur");
                }
            }
        }


        private void Btn_Edit_Game_Click(object sender, RoutedEventArgs e)
        {

        }

        private void closebutton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void closebutton_Click_1(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            HideAddGameForm();
        }

        private void uploadBtn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new OpenFileDialog();

            bool? response = openFileDialog.ShowDialog();

            if (response == true)
            {
                string filepath = openFileDialog.FileName;
                string extension = Path.GetExtension(filepath);
                string name = Path.GetFileNameWithoutExtension(filepath);

                if (extension == ".exe")
                {
                    PATH_TO_EXE_FIELD.Text = filepath;
                }
                else
                {
                    MessageBox.Show("Path not end with .exe");
                    return;
                }

                if (PATH_TO_EXE_FIELD.Text == filepath)
                {
                    if (GAME_NAME_FIELD.Text == "")
                    {
                        GAME_NAME_FIELD.Text = name;
                    }
                    else
                    {
                        return;
                    }

                }
                else
                {
                    MessageBox.Show("Path cannot be added.");
                    return;
                }
            }
        }

        private void Btn_Supp_Game_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HideAddGameForm();
                string gameData = $@"{pathInDev}Data.txt";
                Object selectedItem = combo_b_game.SelectedItem;
                int selectedIndex = combo_b_game.SelectedIndex;
                int selectedLang = cbbtest.SelectedIndex;

                List<string> quotelist = File.ReadAllLines(gameData).ToList();
                quotelist.RemoveAt(selectedIndex);
                File.WriteAllLines(gameData, quotelist.ToArray());

                combo_b_game.Items.Clear();
                ReadGameAtStart();
                if (selectedLang.Equals(0))
                {
                    gamename.Content = "Selectionne un Logiciel";
                    play.Content = "LANCER";
                    Label_jeu.Content = "Logiciels :";
                    Btn_Add_Game.Content = "+ Ajouter";
                    Btn_Supp_Game.Content = "- Supprimer";
                    Btn_save.Content = "Sauvegarder";
                    Btn_cancel.Content = "Annuler";
                    NomDuJeu.Content = "Nom du Logiciel :";
                    PathToExe.Content = "Chemin de l'exe :";
                    label_settings_langue.Content = "Langue :";
                    path_to_exe.Content = "Chemin de l'exe";
                    label_Pathexe.Content = "";
                }
                else if (selectedLang.Equals(1))
                {
                    gamename.Content = "Select a Software";
                    play.Content = "START";
                    Label_jeu.Content = "Softwares :";
                    Btn_Add_Game.Content = "+ Add";
                    Btn_Supp_Game.Content = "- Delete";
                    Btn_save.Content = "Save";
                    Btn_cancel.Content = "Cancel";
                    NomDuJeu.Content = "Software's name :";
                    PathToExe.Content = "Path to exe :";
                    label_settings_langue.Content = "Language :";
                    path_to_exe.Content = "Path to exe :";
                    label_Pathexe.Content = "";
                }
            }
            catch
            {
                MessageBox.Show("Impossible de suppromer un element inexistant !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void settingsbutton_Click(object sender, RoutedEventArgs e)
        {
            

            isCheck = !isCheck;
            if (isCheck)
            {
            cbbtest.Visibility = Visibility.Visible;
            label_settings_langue.Visibility = Visibility.Visible;
            }
            else
            {
                HideSettings();
            }
        }


        public void HideSettings()
        {
            cbbtest.Visibility = Visibility.Hidden;
            label_settings_langue.Visibility=Visibility.Hidden;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Object ogContent = gamename.Content;
            int selectedIndex = cbbtest.SelectedIndex;
            if (selectedIndex.Equals(0))
            {
                


                if (Convert.ToString(ogContent) != "Select a Software")
                {
                    gamename.Content = gamename.Content;
                }
                else
                {


                    gamename.Content = "Selectionne un Logiciel";
                }
                    play.Content = "LANCER";
                    Label_jeu.Content = "Logiciels :";
                    Btn_Add_Game.Content = "+ Ajouter";
                    Btn_Supp_Game.Content = "- Supprimer";
                    Btn_save.Content = "Sauvegarder";
                    Btn_cancel.Content = "Annuler";
                    NomDuJeu.Content = "Nom du Logiciel :";
                    PathToExe.Content = "Chemin de l'exe :";
                    label_settings_langue.Content = "Langue :";
                    path_to_exe.Content = "Chemin de l'exe :";

            }
            else if (selectedIndex.Equals(1))
            {

                if (Convert.ToString(ogContent) != "Selectionne un Logiciel")
                {
                    gamename.Content = gamename.Content;
                }
                else
                {
                    gamename.Content = "Select a Software";

                }

                    play.Content = "START";
                    Label_jeu.Content = "Softwares :";
                    Btn_Add_Game.Content = "+ Add";
                    Btn_Supp_Game.Content = "- Delete";
                    Btn_save.Content = "Save";
                    Btn_cancel.Content = "Cancel";
                    NomDuJeu.Content = "Software's name :";
                    PathToExe.Content = "Path to exe :";
                    label_settings_langue.Content = "Language :";
                    path_to_exe.Content = "Path to exe :";
                
                


            }

            Settings.Default.lang = selectedIndex;
            Settings.Default.Save();
        }

        private void DiscordBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                Process.Start(@"explorer", "https://discord.gg/CaFaRyrej8");
            }
            catch
            {
                MessageBox.Show("contacter le support pour regler le probleme :\nIndev.support@gmail.com");
            }
        }
    }
}
