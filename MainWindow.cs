using System;
using System.IO;
using Gtk;

namespace RTSP_Timelapse_App
{
    class MainWindow
    {
        private string ffmpegPath = "/usr/bin/ffmpeg";
        private string ffprobePath = "/usr/bin/ffprobe";
        private string tempPath = "/tmp/timelapse_app";

        private Window mainWindow;
        private Entry rtspEntry;
        private Button ffplayButton;
        private Entry pathToSaveEntry;
        private Button pathToSaveButton;
        private Entry daysCountNumericEntry;
        private Button startButton;

        public MainWindow() 
        {
            Application.Init();

            mainWindow = new("RTSP Timelaps");
            mainWindow.DeleteEvent += delegate { Application.Quit(); };
            
            Box box = new(Orientation.Vertical, 20) { Margin = 10, WidthRequest = 640 };

            Label emptyLabel1 = new() { Text = "", HeightRequest = 25 };
            box.Add(emptyLabel1);
            
            Box childBox1 = new(Orientation.Vertical, 10);
            Label rtspLabel = new() { Text = "Ссылка на поток", Halign = Align.Center};
            rtspEntry = new() { Halign = Align.Fill, MarginStart = 100, MarginEnd = 100 };
            ffplayButton = new() { Label = "Показать поток", Name = "ffplay", Halign = Align.End, MarginEnd = 100 };
            ffplayButton.Clicked += delegate { Bash.Execute($"ffplay -rtsp_transport tcp {rtspEntry.Text}"); };
            childBox1.Add(rtspLabel);
            childBox1.Add(rtspEntry);
            childBox1.Add(ffplayButton);
            box.Add(childBox1);

            Box childBox2 = new(Orientation.Vertical, 10);
            Label pathLabel = new() { Text = "Путь сохранения", Halign = Align.Center};
            pathToSaveEntry = new() { Halign = Align.Fill, MarginStart = 100, MarginEnd = 100, Text = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) };
            pathToSaveButton = new() { Label = "Открыть", Name = "save", Halign = Align.End, MarginEnd = 100 };
            pathToSaveButton.Clicked += OpenFolderViewButton_Clicked;
            childBox2.Add(pathLabel);
            childBox2.Add(pathToSaveEntry);
            childBox2.Add(pathToSaveButton);
            box.Add(childBox2);

            Box childBox3 = new(Orientation.Horizontal, 10) { Homogeneous = true };
            Label daysLabel = new Label() { Text = "Количество дней", Halign = Align.End};
            daysCountNumericEntry = new() { Halign = Align.Fill, InputPurpose = InputPurpose.Digits, PlaceholderText = "180" };
            Label emptyLabel2 = new Label() { Text = "" };
            childBox3.Add(daysLabel);
            childBox3.Add(daysCountNumericEntry);
            childBox3.Add(emptyLabel2);
            box.Add(childBox3);

            Box childBox4 = new(Orientation.Vertical, 20) { Valign = Align.End };
            Separator separator1 = new(Orientation.Horizontal);
            startButton = new() { Label = "Начать съёмку", Halign = Align.Center };
            startButton.Clicked += StartButton_Clicked;
            Separator separator2 = new(Orientation.Horizontal);
            childBox4.Add(separator1);
            childBox4.Add(startButton);
            childBox4.Add(separator2);
            box.PackEnd(childBox4, true, true, 10);
            
            mainWindow.Add(box);
            mainWindow.ShowAll();

            Application.Run();
        }

        private void OpenFolderViewButton_Clicked(object sender, EventArgs e)
        {
            FileChooserDialog fileChooser = new(
                "Выберите папку сохранения файла", null,FileChooserAction.SelectFolder,
                "Отмена", ResponseType.Cancel,
                "Выбрать", ResponseType.Accept)
            {
                SelectMultiple = false,
                Margin = 10
            };
            fileChooser.SetCurrentFolder(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));

            fileChooser.SetPosition(WindowPosition.CenterOnParent);

            if (fileChooser.Run() == (int)ResponseType.Accept)
                pathToSaveEntry.Text = fileChooser.CurrentFolder;

            fileChooser.Destroy();
        }

        private void StartButton_Clicked(object sender, EventArgs e)
        {
            if (rtspEntry.TextLength == 0)
            {
                MessageDialog md = new(
                    mainWindow,
                    DialogFlags.Modal,
                    MessageType.Error,
                    ButtonsType.Close,
                    "Не указана ссылка на поток");
                md.Run();
                md.Destroy();
            }
            else if (pathToSaveEntry.TextLength == 0)
            {
                MessageDialog md = new(
                    mainWindow,
                    DialogFlags.Modal,
                    MessageType.Error,
                    ButtonsType.Close,
                    "Не указан путь для сохранения");
                md.Run();
                md.Destroy();
            }
            else if (!Directory.Exists(pathToSaveEntry.Text))
            {
                MessageDialog md = new(
                    mainWindow,
                    DialogFlags.Modal,
                    MessageType.Error,
                    ButtonsType.Close,
                    "Не правильно указан путь сохранения\nПроверьте, правильно ли вы указали путь");
                md.Run();
                md.Destroy();
            }
            else if (!File.Exists(ffmpegPath) | !ffmpegPath.Contains("ffmpeg"))
            {
                MessageDialog md = new(
                    mainWindow,
                    DialogFlags.Modal,
                    MessageType.Error,
                    ButtonsType.Close,
                    "Не правильно указан файл FFmpeg\nПроверьте, установлен ли у вас FFmpeg, или правильно ли вы указали к нему путь");
                md.Run();
                md.Destroy();
            }
            else if (!File.Exists(ffprobePath) | !ffprobePath.Contains("ffprobe"))
            {
                MessageDialog md = new(
                    mainWindow,
                    DialogFlags.Modal,
                    MessageType.Error,
                    ButtonsType.Close,
                    "Не правильно указан файл FFprobe\nПроверьте, установлен ли у вас FFprobe, или правильно ли вы указали к нему путь");
                md.Run();
                md.Destroy();
            }
            else if (!File.Exists("/usr/bin/crontab"))
            {
                MessageDialog md = new(
                    mainWindow,
                    DialogFlags.Modal,
                    MessageType.Error,
                    ButtonsType.Close,
                    "На вашей системе не найден Cron\nПроверьте, установлен ли у вас Cron или сопутствующие ему пакеты");
                md.Run();
                md.Destroy();
            }
            else if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);
            else if (daysCountNumericEntry.TextLength == 0 || !daysCountNumericEntry.Text.IsNumber())
            {
                MessageDialog md = new(
                    mainWindow,
                    DialogFlags.Modal,
                    MessageType.Error,
                    ButtonsType.Close,
                    "Произошла ошибка!\nВы не вписали число");
                md.Run();
                md.Destroy();
            }
            else if (int.Parse(daysCountNumericEntry.Text) == 0)
            {
                MessageDialog md = new(
                    mainWindow,
                    DialogFlags.Modal,
                    MessageType.Error,
                    ButtonsType.Close,
                    "Число слишком маленькое\nК большому сожалению, нельзя записать 0 секунд видео");
                md.Run();
                md.Destroy();
            }
            else if (int.Parse(daysCountNumericEntry.Text) > 5760)
            {
                MessageDialog md = new(
                    mainWindow,
                    DialogFlags.Modal,
                    MessageType.Error,
                    ButtonsType.Close,
                    $"Число слишком большое\nВам не хватает около {5760/365} лет записи потока?");
                md.Run();
                md.Destroy();
            }
            else
            {
                Script.Create(rtspEntry.Text, pathToSaveEntry.Text, int.Parse(daysCountNumericEntry.Text));

                MessageDialog md = new(
                    mainWindow,
                    DialogFlags.Modal,
                    MessageType.Info,
                    ButtonsType.Ok,
                    "Готово!\nCron создан\nВ течение " + daysCountNumericEntry.Text + " дней будет создаваться таймлапс");
                md.Run();
                md.Destroy();
            }
        }
    }
}
