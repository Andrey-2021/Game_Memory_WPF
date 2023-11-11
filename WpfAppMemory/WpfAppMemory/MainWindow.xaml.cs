using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfAppMemory
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Объект, представляющий игровое поле, содержащий логику игры
        /// </summary>
        GameLogic game;

        /// <summary>
        /// Список доступных картинок
        /// </summary>
        List<BitmapImage> imagesList;

        /// <summary>
        /// Конструктор
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            
            game = new GameLogic();
            //подписываемся на события
            game.infoChanged += ShowInfo;
            game.gameOver += GameOver;
            game.newBordCreated+= DrawCleanGameBord;
            Beginer.IsChecked = true;
            imagesList = ImageHalper.LoadImages();
            StopGame();
        }
        private void RadioButton_Checked_FirstLevel(object sender, RoutedEventArgs e)
        {
            game.SetSize((int)Level.first);
        }
		private void RadioButton_Checked_NextLevel(object sender, RoutedEventArgs e)
		{
			game.SetSize((int)Level.nextFirst);
		}
		private void RadioButton_Checked_Second(object sender, RoutedEventArgs e)
        {
            game.SetSize((int)Level.second);
        }

		private void RadioButton_Checked_NextSecond(object sender, RoutedEventArgs e)
		{
			game.SetSize((int)Level.nextSecond);
		}
		private void RadioButton_Checked_Professional(object sender, RoutedEventArgs e)
        {
            game.SetSize((int)Level.third);
        }

        /// <summary>
        /// Вывод информации
        /// </summary>
        void ShowInfo(Object sender, EventArgs args)
        {
            BorgSizeInfo.Text = game.BordSizeInfo;
            GameStepCounter.Text = game.StepCounter.ToString();
            GameTime.Text = game.TimeFromStart.ToLongTimeString();
            Info.Text = game.GameStatus;
        }

        /// <summary>
        /// Конец игре
        /// </summary>
        void GameOver(Object sender, EventArgs args)
        {
            StopGame();
            WinWindow winWindow = new WinWindow();
            winWindow.ShowDialog();
        }

        /// <summary>
        /// Рисуем чистую доску (UniformGrid)
        /// </summary>
        void DrawCleanGameBord(Object sender, EventArgs args)
        {
            UniformGridGameBord.Children.Clear(); //удаляем все дочерние элементы Grid

            //размер ячейки
            const int cellSize = 50;

            int n = game.NumberRows;

            UniformGridGameBord.Rows = n;
            UniformGridGameBord.Columns = n;

            UniformGridGameBord.Width = n * cellSize;
            UniformGridGameBord.Height = n * cellSize;

            DrawButtons(UniformGridGameBord);
        }

        /// <summary>
        /// Символ, с которого начинается Имя кнопки
        /// </summary>
        readonly char firstCharInButtonName = '_';

        /// <summary>
        /// Рисуем кнопки
        /// </summary>
        /// <param name="grid">UniformGrid на котором рисуем кнопки</param>
        void DrawButtons(UniformGrid grid)
        {

            // Создаём область имен для grid. Для работы с именами создаваемых кнопок
            NameScope.SetNameScope(grid, new NameScope());

            int n = grid.Rows;
            int m = grid.Columns;

            for (int i = 0; i < n; i++) //цикл по строкам UniformGrid
            {
                for (int j = 0; j < m; j++) //цикл по столбцам UniformGrid
                {
                    //создание кнопки
                    Button btn = new Button();

                    btn.Name = firstCharInButtonName+i.ToString() + firstCharInButtonName + j.ToString();
                    grid.RegisterName(btn.Name, btn);

                    //запись номера кнопки
                    btn.Tag = (i, j); // (i, j) - используем кортеж

                    //толщина границ кнопки
                    btn.Margin = new Thickness(2);

                    //при нажатии кнопки, будет вызываться метод Btn_Click    
                    btn.Click += Btn_Click;

                    SimpleButton(btn);

                    //добавление кнопки в сетку
                    grid.Children.Add(btn);
                }
            }
        }


        /// <summary>
        /// Устанавливаем свойства для обычной кнопки
        /// </summary>
        /// <param name="button">Кнопка</param>
        void SimpleButton(Button button)
        {
            button.BorderBrush = Brushes.Blue;
            button.BorderThickness = new Thickness(1);
        }

        /// <summary>
        /// Устанавливаем свойства для Выделения кнопки.
        /// т.е. для выделения кнопки, на которую нажали, приоткрыли, но ещё не окрыли с её парой
        /// </summary>
        /// <param name="button">Кнопка</param>
        void SelectedButton(Button button)
        {
            button.BorderBrush = Brushes.Red;
            button.BorderThickness = new Thickness(3);

        }

        /// <summary>
        /// Обработчкин нажатия кнопок
        /// </summary>
        /// <param name="sender">Объект(кнопка), в котором произошло событие "Нажатие на кнопку"</param>
        /// <param name="e"></param>
        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            //получение значения лежащего в Tag - это кортеж, поэтому (int i, int j)
            var coordinates = ((int i, int j))((Button)sender).Tag;

            if (((Button)sender).Content != null) //если содержимое не пустое, т.е. уже показана картинка
            {  //у нас повторное нажатие на картинку
              
                SystemSounds.Hand.Play(); //Звук
                return; //сразу выходим
            }

            game.KeyPressed(coordinates);
            RedrawGameBord(UniformGridGameBord);
        }


        /// <summary>
        /// Перерисовать все кнопки
        /// </summary>
        /// <param name="grid">UniformGrid на котором надо перерисовать все кнопки</param>
        void RedrawGameBord(UniformGrid grid)
        {
            int n = grid.Rows;
            int m = grid.Columns;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    string name = firstCharInButtonName + i.ToString() + firstCharInButtonName + j.ToString();
                    var obj = grid.FindName(name);
                    
                    Button button = obj as  Button; //пробуем преобразовать объект в Button

                    if (button != null) //если преобразование прошло успешно
                    {
                        //если надо спрятать картинку на текущей кнопке [i,j] и есть что-то на кнопке
                        if (game.GameBordCellsState[i, j] == GemeCellState.closed && button.Content != null)
                        {
                            SimpleButton(button);
                            button.Content = null; //очистить содержимое кнопки
                            continue;  //сразу переходим к следующей итерации цикла
                        }

                        //если надо приоткрыть кнопку=картинку
                        if (game.GameBordCellsState[i, j] == GemeCellState.visible && button.Content == null)
                        {
                            //создание контейнера для хранения изображения
                            Image img = new Image();

                            int imageNumber = game.GameBord[i, j];

                            //запись картинки с миной в контейнер
                            img.Source = imagesList[imageNumber];

                            //создание компонента для отображения изображения
                            StackPanel stackPnl = new StackPanel();
                            //установка толщины границ компонента
                            stackPnl.Margin = new Thickness(1);
                            //добавление контейнера с картинкой в компонент
                            stackPnl.Children.Add(img);
                            //запись компонента в кнопку
                            button.Content = stackPnl;

                            SelectedButton(button);

                            continue;//сразу переходим к следующей итерации цикла
                        }


                        //если надо открыть кнопку навсегда, но картинка уже нарисована
                        if (game.GameBordCellsState[i, j] == GemeCellState.opened && button.Content != null)
                        {
                            //тогда нам не надо перерисовывать картинку,
                            //а надо только установить свойства этой кнопки как обычной кнопки
                            // чтобы убрать "выделение" приоткрытых кнопок
                            SimpleButton(button);
                        }    
                        
                        //если надо открыть кнопку навсегда,т.е. нарисовать с картинкой, а картинки ещё на кнопке нет 
                            if (game.GameBordCellsState[i, j] == GemeCellState.opened && button.Content == null)
                        {
                            //создание контейнера для хранения изображения
                            Image img = new Image();

                            int imageNumber = game.GameBord[i, j];

                            //запись картинки с миной в контейнер
                            img.Source = imagesList[imageNumber];

                            //создание компонента для отображения изображения
                            StackPanel stackPnl = new StackPanel();
                            //установка толщины границ компонента
                            stackPnl.Margin = new Thickness(1);
                            //добавление контейнера с картинкой в компонент
                            stackPnl.Children.Add(img);
                            //запись компонента в кнопку
                            button.Content = stackPnl;

                            SimpleButton(button);

                            continue;
                        }
                    }
                }
            }
        }

        //Помощь
        private void MenuItem_Click_Help(object sender, RoutedEventArgs e)
        {
            WindowHelp help = new WindowHelp();
            help.ShowDialog();
        }

        //О программе
        private void MenuItem_Click_About(object sender, RoutedEventArgs e)
        {
            WindowAbout about = new WindowAbout();
            about.ShowDialog();
        }

        //Выход
        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        //Старт
        private void Button_Click_StartGame(object sender, RoutedEventArgs e)
        {
            StartGame();
            game.StartGame(imagesList.Count);
        }

        //Стоп
        private void Button_Click_StopGame(object sender, RoutedEventArgs e)
        {
            game.StopGame();
            StopGame();
        }

        /// <summary>
        /// Настройка элементов окна при нажатии на кнопку Старт игры
        /// </summary>
        void StartGame()
        {
            GroupBoxSelectGameLevel.IsEnabled = false;
            UniformGridGameBord.IsEnabled = true;
            ButtonStart.IsEnabled = false;
            ButtonStop.IsEnabled = true;

            GameStepCounter.Clear();
            GameTime.Clear();
        }

        /// <summary>
        /// Настройка элементов окна при Остановке игры
        /// </summary>
        void StopGame()
        {
            GroupBoxSelectGameLevel.IsEnabled = true;
            UniformGridGameBord.IsEnabled = false;
            ButtonStart.IsEnabled = true;
            ButtonStop.IsEnabled = false;
        }
    }
}
