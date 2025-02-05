using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Интерфейс
{
    public partial class Form1 : Form
    {
        private DataTable dataTable;    // Таблица данных для dataGridView

        int liveCycle = 0;          // Кол-во прошедших циклов
        int liverCount=0;           // Текущее кол-во особей
        bool isRun = false;         // Флаг статуса симуляции (запущена или нет)
        bool manualInput = false;   // Флаг ручного ввода
        bool isMatrixChanched;      // Изменилась ли матрица
        
        // Матрицы
        bool[,] matrix = new bool[25, 25];          // Матрица особей
        int[,] matrixNeighbor = new int[25, 25];    // Матрица с кол-вом соседей каждой клетки

        // Метод инициализация формы
        public Form1()
        {   
            InitializeComponent();
        }


        // Метод начальной настройки формы
        private void Form1_Load(object sender, EventArgs e)
        {
            matrixDataload();
            Updater();
            radioButton1.Checked = true;
        }

        // Метод изменения кол-во строк и столбцов в dataGridView
        private void matrixDataload()
        {
            dataTable = new DataTable();    // Переопределяем таблицу

            // Добавляем столбцы
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                dataTable.Columns.Add("Column " + (i + 1));
            }

            // Добавляем строки
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                DataRow row = dataTable.NewRow();
                dataTable.Rows.Add();
            }

            dataGridView1.DataSource = dataTable;   // Привязываем DataTable к DataGridView
        }


        //  Метод подсчёта соседей для каждой клетки
        private int neighbourCount(int i, int j)
        {
            int x, y;
            int countNeighbor = 0;
            for (x = i - 1; x <= i + 1; x++)
                for (y = j - 1; y <= j + 1; y++)
                    if (x >= 0 && x < matrix.GetLength(0) && y >= 0 && y < matrix.GetLength(1))
                        if (x != i || y != j)
                            if (matrix[x, y] == true)
                                countNeighbor++;
            return countNeighbor;
        }

        // Метод обновления данных в dataGridView
        private void matrixDataUpdate(int i, int j)
        {
            isMatrixChanched = true;
            if (matrix[i, j] == true)
                dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Black;
            else dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Empty;
        }


        // Метод обновления состояния клеток
        private async void Updater()
        {
            while (true)
            {
                while (isRun)   // Если симуляция идёт
                {
                    if (isMatrixChanched == false)  // Если матрица не изменилась, то останавливаем игру
                    {
                        isRun = false;
                    }
                    isMatrixChanched = false;
                    if (liverCount != 0)    // Если есть ещё живые особи
                    {
                        for (int i = 0; i < matrix.GetLength(0); i++)
                            for (int j = 0; j < matrix.GetLength(1); j++)
                            {
                                matrixNeighbor[i, j] = neighbourCount(i, j);    // Подсчитываем соседей для каждой клетки и записываем в neighboourCount
                            }
                        for (int i = 0; i < matrix.GetLength(0); i++)
                            for (int j = 0; j < matrix.GetLength(1); j++)
                            {
                                if (matrix[i, j] == false && matrixNeighbor[i, j] == 3) // Условия рождения новой особи
                                {
                                    matrix[i, j] = true;
                                    matrixDataUpdate(i, j);
                                    liverCount++;
                                }
                                else if (matrix[i, j] == true)
                                    if (matrixNeighbor[i, j] < 2 || matrixNeighbor[i, j] > 3)   // Условия смерти особи
                                    {
                                        matrix[i, j] = false;
                                        matrixDataUpdate(i, j);
                                        liverCount--;
                                    }
                            }
                        dataGridView1.Refresh();
                        liveCycle++;
                        label1.Text = $"цикл жизни: {liveCycle}";
                        label2.Text = $"количество особей: {liverCount}";
                    }
                    else isRun = false; // Если особей не осталось, останавливаем симуляцию
                    await Task.Delay(1);
                }
                // Задержка для имитации времени обновления
                await Task.Delay(1);
            }
        }


        // Метод нажатия на кнопку "старт"
        private void startButton_click(object sender, EventArgs e)
        {
                isRun = true;
        }


        // Метод нажатия на кнопку "стоп"
        private void stop_Click(object sender, EventArgs e)
        {
            isRun = false;
        }


        // Метод нажатия на кнопку "сброс"
        private void reset_Click(object sender, EventArgs e)
        {
            isRun = false; // Останавливаем симуляцию
            // Освобождаем матрицу
            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(0); j++)
                {
                    matrix[i, j] = false;
                    matrixNeighbor[i, j] = 0;
                }
            // Обнуляем счётчики
            liveCycle = 0;
            liverCount = 0;
            label1.Text = $"цикл жизни: {liveCycle}";
            label2.Text = $"количество особей: {liverCount}";
            matrixDataload();
        }


        // Метод нажатие на кнопку "создать поле"
        private void сreate_click(object sender, EventArgs e)
        {
            if (fieldWight.Text != "" && fieldHeight.Text != "")    // Проверяем, пустые ли поля ширины и высоты поля
            {
                isRun = false;  // Останавливаем игру

                // Берём данные из полей для ввода
                int width = int.Parse(fieldWight.Text);
                int height = int.Parse(fieldHeight.Text);
                int requiredLivers = 0;
                if (radioButton2.Checked)
                    requiredLivers = int.Parse(liverBox.Text);

                int totalCells = width * height;    // Колличество всех клеток

                if (!radioButton1.Checked && requiredLivers > totalCells)   // Если количество особей для генераций больше чем кол-во всех клеток
                {
                    MessageBox.Show("Количество клеток для генерации превышает размер поля.");
                    return;
                }

                if(int.Parse(fieldWight.Text)>25 || int.Parse(fieldHeight.Text) > 25)
                {
                    MessageBox.Show("Ширина и высота поля не должны больше 25");
                    return;
                }

                // Переинициализируем матрицы
                matrix = new bool[width, height];
                matrixNeighbor = new int[width, height];

                // Сбрасываем источник данных
                dataGridView1.DataSource = null;
                matrixDataload();

                // Обнуляем счётчики
                liveCycle = 0;
                liverCount = 0;
                label1.Text = $"цикл жизни: {liveCycle}";
                label2.Text = $"количество особей: {liverCount}";

                if (radioButton2.Checked)   // Если выбран режим генерации
                {
                    // Выводим кол-во особей
                    liverCount = requiredLivers;
                    label2.Text = $"количество особей: {liverCount}";

                    // Заполняем поле рандомно
                    Random rnd = new Random();
                    int placedCells = 0;    // Заполненные клетки
                    HashSet<(int, int)> usedPositions = new HashSet<(int, int)>(); // Кортеж с уникальными позициями особей
                    while (placedCells < requiredLivers)    // Пока заполенные клетки меньше нужного кол-ва особей
                    {
                        int x = rnd.Next(0, width);
                        int y = rnd.Next(0, height);
                        if (!usedPositions.Contains((x, y)))    // Если в данной позиции нет особи
                        {
                            matrix[x, y] = true;
                            usedPositions.Add((x, y));  // Добавляем новую запись в кортеж, чтобы в эту же клетку нельзя было 2 раз поставить клетку
                            placedCells++;
                        }
                    }
                    // Обновляем визуализацию
                    for (int i = 0; i < width; i++)
                        for (int j = 0; j < height; j++)
                            matrixDataUpdate(i, j);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните все поля (ширина, высота, количество клеток).");
            }
        }


        // Метод включения radioButton для ручного ввода
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            label7.Visible = false;
            liverBox.Visible = false;
            manualInput = true;
        }


        // Метод включения radioButton для генерации 
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            label7.Visible = true;
            liverBox.Visible = true;
            manualInput = false;
        }



        // Метод нажатия по клетке элемента dataGridView
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (isRun == false && manualInput) // Если симуляция остановлена и стоит режим ручного ввода
            {
                if (matrix[e.RowIndex, e.ColumnIndex] == false) // Если текущая клетка пустая, создаём в ней особь
                {
                    matrix[e.RowIndex, e.ColumnIndex] = true;
                    liverCount++;
                }
                else
                {
                    matrix[e.RowIndex, e.ColumnIndex] = false;  // Если в текущей клетке есть особь, освобождаем клетку
                    liverCount--;
                }
                label2.Text = $"количество соседей: {liverCount}";
                matrixDataUpdate(e.RowIndex, e.ColumnIndex);
            }
            dataGridView1.ClearSelection(); // Убираем выделение ячейки
        }


        // Метод защиты от неккоректного ввода
        private void inputProtection(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = sender as TextBox;    // Получаем текст поля, который вызвал функцию

            if (!(e.KeyChar >= '0' && e.KeyChar <= '9' || (int)e.KeyChar == 8))    // Если вводимый символ не цифра
                e.KeyChar = (char)0;

            if (textBox.Text.Length == 0 && e.KeyChar == '0')   // Если поле пустое, не разрешаем ввод '0' как первую цифру
            {
                e.KeyChar = (char)0;
                return;
            }
        }
    }
}
