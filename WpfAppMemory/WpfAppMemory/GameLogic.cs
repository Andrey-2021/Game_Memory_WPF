using System;
using System.Windows.Threading;

namespace WpfAppMemory
{
    /// <summary>
    /// Класс, содержит игровое поле и всю логику игры
    /// </summary>
    public class GameLogic
    {
        /// <summary>
        /// Матрица игрового поля
        /// </summary>
        private int[,] gameBord;

        /// <summary>
        /// Публичное свойство. Матрица игрового поля
        /// </summary>
        public int[,] GameBord
        {
            get => gameBord;
        }

        /// <summary>
        /// Матрица состояний ячеек игрового поля. 
        /// (Указывает в каком из трёх состояний находится каждая ячейка)
        /// </summary>
        GemeCellState[,] gameBordCellsState;

        /// <summary>
        /// Свойство. Матрица состояний ячеек игрового поля. 
        /// </summary>
        public GemeCellState[,] GameBordCellsState
        {
            get => gameBordCellsState; //открыто только для чтения
        }

        /// <summary>
        /// Счётчик сделаных ходов
        /// </summary>
        int stepCounter;

        /// <summary>
        /// Свойство. Счётчик сделаных ходов
        /// </summary>
        public int StepCounter
        {
            get => stepCounter;
            private set
            {
                if (value < 0) value = 0; //если вдруг попытка записать число меньше 0, тогда 0
                stepCounter = value;
                infoChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Таймер игры
        /// </summary>
        DispatcherTimer gameTimer;

        /// <summary>
        /// Время от начала игры
        /// </summary>
        DateTime timeFromStart;

        /// <summary>
        /// Свойство. Время от начала игры
        /// </summary>
        public DateTime TimeFromStart
        {
            get =>timeFromStart;
            private set
            {
                timeFromStart = value;
                infoChanged?.Invoke(this, EventArgs.Empty); //информируем подписчиков на событие, что информация в объекте изменилась
            }
        }

        /// <summary>
        /// Состояние игры
        /// </summary>
        string gameStatus;
        
        /// <summary>
        /// Свойство. Информация о состоянии игры
        /// </summary>
        public string GameStatus
        {
            get => gameStatus;
            private set
            {
                gameStatus = value;
            }
        }

        /// <summary>
        /// Свойство. Возвращает размер доски- количество строк
        /// </summary>
        public int NumberRows
        {
            get
            {
                return gameBord.GetLength(0);
            }
        }

        /// <summary>
        /// Свойство. Возвращает размер доски в виде строки
        /// </summary>
        public string BordSizeInfo
        {
            get
            {
                int n = gameBord.GetLength(0);
                return n + "x" + n;
            }
        }


        /// <summary>
        /// Событие. Информация о игровой доске изменилась
        /// </summary>
        public EventHandler infoChanged;

        /// <summary>
        /// Событие. Надо перерисовать новую чистую доску
        /// </summary>
        public EventHandler newBordCreated;

        /// <summary>
        /// Событие. Конц игре. Победа
        /// </summary>
        public EventHandler gameOver;


        /// <summary>
        /// конструктор
        /// </summary>
        public GameLogic()
        {
            gameBord = new int[0, 0];
            gameBordCellsState = new GemeCellState[0, 0];

            TimeFromStart = new DateTime();
            //настройка таймера
            gameTimer =new DispatcherTimer();
            gameTimer.Interval = new TimeSpan(0, 0, 1); //интервал 1 секунда
            gameTimer.Tick += new EventHandler(GameTimerick); //метод, который будет вызываться при срабатывании таймера

            GameStatus = "Поехали!!!";
        }

        /// <summary>
        /// метод, который вызывается при срабатывании таймера
        /// </summary>
        private void GameTimerick(object sender, EventArgs e)
        {
            TimeFromStart=TimeFromStart.AddSeconds(1); //прибавляем секунду
        }


        /// <summary>
        /// Задать размер игрового поля
        /// </summary>
        /// <param name="n"></param>
        public void SetSize(int n)
        {
            gameBord = new int[n, n];
            gameBordCellsState = new GemeCellState[n, n];
            infoChanged?.Invoke(this, EventArgs.Empty);
            newBordCreated?.Invoke(this, EventArgs.Empty);
        }


        /// <summary>
        /// Проверка победы или поражения
        /// </summary>
        public void CheckEndGame()
        {
            for (int i = 0; i < gameBord.GetLength(0); i++)
            {
                for (int j = 0; j < gameBord.GetLength(1); j++)
                {
                    //если есть закрытая ячейка
                    if (gameBordCellsState[i, j] == GemeCellState.closed)
                    { //сразу выходим из метода, значит игра ещё не окончена
                        return;
                    }
                }
            }
            //все ячейки открыты

            gameTimer.Stop(); //останавливаем таймер
            GameStatus = "Ураааааа!!!! Победа!!!!";
            gameOver?.Invoke(this, EventArgs.Empty);//информируем подписчиков на событие, что игра окончена
            infoChanged?.Invoke(this, EventArgs.Empty); //информируем подписчиков на событие, что информация в объекте изменилась
        }


        /// <summary>
        /// Начало игры.
        /// </summary>
        /// <param name="maxNumber">Число, ограничивает максимально возможное случайное число для ячейки.
        /// (Равно количеству доступных картинок)
        /// </param>
        public void StartGame(int maxNumber)
        {
            newBordCreated?.Invoke(this, EventArgs.Empty);
            StepCounter = 0;
            ClearGameBords();
            CreateRandomBord(maxNumber);

            GameStatus = "Вперёд!!!";

            TimeFromStart = DateTime.MinValue;
            gameTimer.Start(); //запускаем таймер
        }


        /// <summary>
        /// Остановить игру
        /// </summary>
        public void StopGame()
        {
            gameTimer.Stop(); //остановили таймер
            GameStatus = "Остановчка...";
            infoChanged?.Invoke(this, EventArgs.Empty); //информируем подписчиков на событие, что информация в объекте изменилась
            
        }





        /// <summary>
        /// Очистить матрицы
        /// </summary>
        void ClearGameBords()
        {
            //Очистили поле
            for (int i = 0; i < gameBord.GetLength(0); i++)
            {
                for (int j = 0; j < gameBord.GetLength(1); j++)
                {
                    gameBord[i, j] = -1;                         // в ячеке "нет числа"
                    gameBordCellsState[i, j] = GemeCellState.closed; // ячейка закрытая
                }
            }
        }




        /// <summary>
        /// Заполнить доску случайными числами
        /// </summary>
        /// <param name="maxNumber">Максимально допустимое число</param>
        void CreateRandomBord(int maxNumber)
        {
            Random rnd = new Random();

            //количество пар картинок
            int k = gameBord.GetLength(0) * gameBord.GetLength(1) / 2;

            //заполняем матрицу случайными ПАРАМИ чисел=номер картинки
            for (int count = 0; count < k; count++)
            {
                int randomNumberImage = rnd.Next(0, maxNumber); //получаем случайную Цифру (=номер картиники)

                //должна быть пара цифр (картинок)
                SaveToRandomCell(randomNumberImage, gameBord); // Цифру (НОМЕР картинки) записываем в случайную ячейку
                SaveToRandomCell(randomNumberImage, gameBord); // эту же Цифру (этот же Номер картинки) записываем в другую случайную ячейку
            }



            //Запись числа в случайную ячейку матрицы
            // Если ячейка уже занята, переходим к следующей по порядку ячейке
            //пока не найдём свободную ячейку
            void SaveToRandomCell(int imageNumber, int[,] matr)
            {
                int i = rnd.Next(0, matr.GetLength(0)); //случайный номер строки
                int j = rnd.Next(0, matr.GetLength(1)); //случайный номер столбца

                do
                {
                    // если ячейка свободна
                    if (matr[i, j] == -1)
                    {
                        matr[i, j] = imageNumber; //записываем туда число
                        break; //выходим из цикла
                    }
                    else //если ячека занята
                    {

                        i++; //идём к следующей по порядку ячейке

                        //если достигли конца строки
                        if (i == matr.GetLength(0))
                        {
                            j++; //переходим на следующую строку
                            i = 0;
                        }
                        //если это была последняя строка в матрице
                        if (j == matr.GetLength(1))
                        {
                            //переходим на первую строку
                            j = 0;
                            i = 0;
                        }
                    }
                }
                while (true);
            }
        }


        /// <summary>
        /// Кортеж. Координаты первой временно приоткрытой ячейки
        /// </summary>
        (int i, int j) firstSelectedCell = (-1, -1);

        /// <summary>
        /// Кортеж. Координаты второй временно приоткрытой ячейки
        /// </summary>
        (int i, int j) secondSelectedCell = (-1, -1);


        //это по условию задания -  <<Каждое действие пользователя приводит к изменению состояния поля.>>

        /// <summary>
        /// Кликнули на ячейку игрового поля с координатами (i,j)
        /// </summary>
        /// <param name="selectedCell">Кортеж. Координаты текущей выбранной ячейки</param>
        public void KeyPressed((int i, int j) selectedCell)
        {
            //если нажали по уже открытой ячейке (картинке)
            if (gameBordCellsState[selectedCell.i, selectedCell.j] != GemeCellState.closed)
            {  //ничего не делаем, сразу выходим
                return;
            }

            StepCounter++;


            //если первая ячейка открыта и вторая открыта
            if (firstSelectedCell.i != -1 && secondSelectedCell.i != -1)
            {
                //закрываем обе приоткрытые ячейки
                gameBordCellsState[firstSelectedCell.i, firstSelectedCell.j] = GemeCellState.closed;
                gameBordCellsState[secondSelectedCell.i, secondSelectedCell.j] = GemeCellState.closed;

                //текущую (на которую кликнули) ячейку приоткрываем
                gameBordCellsState[selectedCell.i, selectedCell.j] = GemeCellState.visible;

                // запоминаем текущюу выбранную ячейку как ПЕРВУЮ приоткрытую
                firstSelectedCell = selectedCell; //запомнили 
                //второй открытой ячейки нет
                secondSelectedCell = (-1, -1);

                return;
            }

            //если есть первая открятая ячейка и нет второй открытой
            // И цифры в ячейках (открытой и текущей нажатой) игрового поля совпадают
            if (firstSelectedCell.i != -1 && secondSelectedCell.i == -1
                && GameBord[firstSelectedCell.i, firstSelectedCell.j] == GameBord[selectedCell.i, selectedCell.j])
            { //т.е. Угадали две картинки

                //оставляем открытыми обе картинки
                gameBordCellsState[firstSelectedCell.i, firstSelectedCell.j] = GemeCellState.opened;
                gameBordCellsState[selectedCell.i, selectedCell.j] = GemeCellState.opened;
                //нет открытых ячеек
                firstSelectedCell = (-1, -1);

                //это по условию задания -
                // <<После каждого действия, совершаемого над полем, должны проверяться условия победы и поражения.>>
                // но нам достаточно проверять здесь
                // и у нас нет Поражения
                CheckEndGame();
                return;
            }

            //если есть первая открятая ячейка и нет второй открытой
            if (firstSelectedCell.i != -1 && secondSelectedCell.i == -1)
            {
                //текущую (на которую нажали) ячейку приоткрываем
                gameBordCellsState[selectedCell.i, selectedCell.j] = GemeCellState.visible;

                // запоминаем текущюу выбранную ячейку как ВТОРУЮ приоткрытую
                secondSelectedCell = selectedCell; //запомнили 

                return;
            }

            //если нет ни первой открытой ячейки И ни второй
            if (firstSelectedCell.i == -1 && secondSelectedCell.i == -1)
            {
                //текущую (на которую нажали) ячейку приоткрываем
                gameBordCellsState[selectedCell.i, selectedCell.j] = GemeCellState.visible;

                // запоминаем текущюу выбранную ячейку как ПЕРВУЮ приоткрытую
                firstSelectedCell = selectedCell;

                return;
            }
        }
    }
}
