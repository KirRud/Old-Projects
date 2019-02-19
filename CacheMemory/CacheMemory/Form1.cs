using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace CacheMemory
{
    struct Cash
    {
        public int index, attribute, priority;//индекс, признак, приоритет
        public double hoarder;// накопитель
        public Cash SetCashBegin(int i)
        {
            Cash value;
            value.index = i;
            value.attribute = -1;
            value.hoarder = 0;
            value.priority = 0;
            return value;
        }
    }
    struct RAM
    {
        public int address;//адресс, слово
        public double  word;
        static public RAM SetRAMBegin(int i)
        {
            RAM value;
            value.address = i;
            value.word = 0;
            return value;
        }

    }

    public partial class Form1 : Form
    {

        void VectRAM(out RAM[] _RAM)
        {
            RAM[] value = new RAM[sizeOfRAM];
            for (int i=0; i <sizeOfRAM; i++)
            {
                value[i] = RAM.SetRAMBegin(i);
            }
            _RAM = value;
        }
        void VectCash(out Cash[] _Cash)
        {
            Cash[] value = new Cash[sizeOfCash];
            for (int i = 0; i < sizeOfCash; i++)
            {
                value[i] = value[i].SetCashBegin(i);
            }
            _Cash = value;
        }
        void NullMatr(out int[,] _matr, int _sizeOfCash)
        {
            int[,] matr = new int[_sizeOfCash, _sizeOfCash];
            for (int i=0; i <_sizeOfCash; i++)
            {
                for (int j=0; j<_sizeOfCash; j++)
                {
                    matr[i, j] = 0;
                }
            }
            _matr = matr;
        }
        void PriorityOfCash(ref Cash[] _cash, int[,] _matr, int _sizeOfCash)
        {
            int k = 0;
            for (int i = 0; i < _sizeOfCash; i++)
            {
                k = 0;
                for (int j = 0; j < _sizeOfCash; j++)
                {
                    if (_matr[i, j] == 1) { k++; }
                }
                _cash[i].priority = k;
            }
        }
        void ChangePriority(ref int[,] _matr, int index, int _sizeOfCash)
        {
            for (int j=0; j <_sizeOfCash; j++)
            {
                _matr[index, j] = 1;
            }
            for (int i=0; i <_sizeOfCash; i++)
            {
                _matr[i, index] = 0;
            }
        }
        bool CheckFullCash(Cash[] _cash, out int index, int _sizeOfCash)
        {
            int k = -1;
            bool flg = true;
            for(int i=0; i <_sizeOfCash; i++) {
                if (_cash[i].attribute == -1)
                {
                    flg = false;
                    k = i;
                    break;
                }
            }
            index = k;
            return flg;
        }
        bool InCash(Cash[] _cash, int address, out int index, int _sizeOfCash)
        {
            for (int i=0; i <_sizeOfCash; i++)
            {
                if (_cash[i].attribute == address) { index = i; return true; }
            }
            index = -1;
            return false;
        }
        int MinPriority(Cash[] _cash, int _sizeOfCash)
        {
            int min = 1024;
            int index = 1023;
            for(int i=0; i< _sizeOfCash; i++)
            {
                if(min>_cash[i].priority)
                {
                    min = _cash[i].priority;
                    index = i;
                }
            }
            return index;
        }

        const int sizeOfRAM = 32 * 1024/4;
        const int sizeOfCash = 1024/4;
        const int disharge = 32;
        Cash[] vectCash = new Cash[sizeOfCash];
        RAM[] vectRAM = new RAM[sizeOfRAM];
        int[,] matrOfPriority = new int[sizeOfCash, sizeOfCash];
        Random rand = new Random();
        bool filledRAM = false;

        public Form1()
        {
            InitializeComponent();

            dataGridView2.Columns.Add(Convert.ToString(0), "Адрес");
            dataGridView2.Columns.Add(Convert.ToString(0), "Слово");

            for (int i=0; i<sizeOfRAM; i++)
            {
                dataGridView2.Rows.Add();
            }

            dataGridView1.Columns.Add(Convert.ToString(0), "Индекс");
            dataGridView1.Columns.Add(Convert.ToString(1), "Признак");
            dataGridView1.Columns.Add(Convert.ToString(2), "Накопитель");
            dataGridView1.Columns.Add(Convert.ToString(3), "Приоритет");

            for (int i=0; i < sizeOfCash; i++)
            {
                dataGridView1.Rows.Add();
            }

            VectCash(out vectCash);
            VectRAM(out vectRAM);

            for (int i=0; i <sizeOfCash; i ++)
            {
                this.dataGridView1.Rows[i].Cells[0].Value = vectCash[i].index;
                this.dataGridView1.Rows[i].Cells[1].Value = vectCash[i].attribute;
                this.dataGridView1.Rows[i].Cells[2].Value = vectCash[i].hoarder;
                this.dataGridView1.Rows[i].Cells[3].Value = vectCash[i].priority;
            }

            for (int i=0; i<sizeOfRAM; i++)
            {
                this.dataGridView2.Rows[i].Cells[0].Value = vectRAM[i].address;
                this.dataGridView2.Rows[i].Cells[1].Value = vectRAM[i].word;
            }
            NullMatr(out matrOfPriority,sizeOfCash);

            dataGridView3.Columns.Add(Convert.ToString(0), "Число обращений к ОЗУ");
            dataGridView3.Columns.Add(Convert.ToString(1), "Попадание");
            for (int j = 0; j < sizeOfRAM / 32; j++)
            {
                dataGridView3.Rows.Add();
            }

            dataGridView4.Columns.Add(Convert.ToString(0), "Объем кэш");
            dataGridView4.Columns.Add(Convert.ToString(1), "Число промахов");
            for (int j = 0; j < sizeOfRAM/32 ; j++)
            {
                dataGridView4.Rows.Add();
            }

            textBox3.Text = "" + rand.Next(sizeOfRAM - sizeOfRAM / 32 + 1);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex == 0)
            {
                textBox1.Enabled = true;
                textBox2.Enabled = false;
                button1.Enabled = true;
                button2.Enabled = false;
                button3.Enabled = true;
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            VectRAM(out vectRAM);
            filledRAM = true;
            int value = 0;
            for (int i=0; i <sizeOfRAM; i++)
            {
                for(int j=0; j <disharge; j++)
                {
                    value = rand.Next(2);
                    vectRAM[i].word += Convert.ToDouble(Math.Pow(10, j) * value);                   
                }
                this.dataGridView2.Rows[i].Cells[1].Value = vectRAM[i].word;

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int value = rand.Next(sizeOfRAM);
            this.textBox1.Text = Convert.ToString(value);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int value = 0;
            double wordForWrite = 0;
            for (int j = 0; j < disharge; j++)
            {
                value = rand.Next(2);
                wordForWrite += Convert.ToDouble(Math.Pow(10, j) * value);
            }
           
            this.textBox2.Text = Convert.ToString(wordForWrite);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!filledRAM)
            {
                MessageBox.Show("Заполните опреативную память");
                return;
            }
            int index;
            //Reading
            if (comboBox1.SelectedIndex == 0)
            {
               int address = Convert.ToInt32(textBox1.Text);
               if (address > 32767 || address < 0)
                {
                    MessageBox.Show("Нет такого адреса");
                    return;
                }
               if (InCash(vectCash, address, out index,sizeOfCash))
                {
                    ChangePriority(ref matrOfPriority, index, sizeOfCash);
                    PriorityOfCash(ref vectCash, matrOfPriority, sizeOfCash);
                    for (int i=0; i <sizeOfCash; i++)
                    {
                        this.dataGridView1.Rows[i].Cells[3].Value = vectCash[i].priority;
                    }
                    
                }
               else
                {
                    if (!CheckFullCash(vectCash, out index, sizeOfCash))
                    {
                        vectCash[index].attribute = address;
                        vectCash[index].hoarder = vectRAM[address].word;
                        ChangePriority(ref matrOfPriority, index, sizeOfCash);
                        PriorityOfCash(ref vectCash, matrOfPriority, sizeOfCash);
                        this.dataGridView1.Rows[index].Cells[1].Value = vectCash[index].attribute;
                        this.dataGridView1.Rows[index].Cells[2].Value = vectCash[index].hoarder;
                        for (int i=0; i<sizeOfCash; i++)
                        {
                            this.dataGridView1.Rows[i].Cells[3].Value = vectCash[i].priority;
                        }                        
                    }
                    else
                    {
                        index = MinPriority(vectCash, sizeOfCash);
                        vectCash[index].attribute = address;
                        vectCash[index].hoarder = vectRAM[address].word;
                        ChangePriority(ref matrOfPriority, index, sizeOfCash);
                        PriorityOfCash(ref vectCash, matrOfPriority, sizeOfCash);
                        this.dataGridView1.Rows[index].Cells[1].Value = vectCash[index].attribute;
                        this.dataGridView1.Rows[index].Cells[2].Value = vectCash[index].hoarder;
                        for (int i = 0; i < sizeOfCash; i++)
                        {
                            this.dataGridView1.Rows[i].Cells[3].Value = vectCash[i].priority;
                        }
                    }
                }
            }
            //Writing
            else if(comboBox1.SelectedIndex == 1) {
                int address = Convert.ToInt32(textBox1.Text);
                double word = Convert.ToDouble(textBox2.Text);
                if (address > 32767 || address < 0)
                {
                    MessageBox.Show("Нет такого адреса");
                    return;
                }
                if (word > 11111111111111111111111111111111.0 || word < 0)
                {
                    MessageBox.Show("Не корректное слово");
                    return;
                }
                if (InCash(vectCash,address,out index, sizeOfCash))
                {
                    ChangePriority(ref matrOfPriority, index, sizeOfCash);
                    PriorityOfCash(ref vectCash, matrOfPriority, sizeOfCash);
                    vectCash[index].hoarder = word;
                    this.dataGridView1.Rows[index].Cells[2].Value = vectCash[index].hoarder;
                    for (int i = 0; i < sizeOfCash; i++)
                    {
                        this.dataGridView1.Rows[i].Cells[3].Value = vectCash[i].priority;
                    }
                    vectRAM[address].word = word;
                    this.dataGridView2.Rows[address].Cells[1].Value = vectRAM[address].word;
                }
                else
                {
                    if (!CheckFullCash(vectCash, out index, sizeOfCash))
                    {
                        vectRAM[address].word = word;
                        vectCash[index].attribute = address;
                        vectCash[index].hoarder = vectRAM[address].word;
                        ChangePriority(ref matrOfPriority, index, sizeOfCash);
                        PriorityOfCash(ref vectCash, matrOfPriority, sizeOfCash);
                        this.dataGridView2.Rows[address].Cells[1].Value = vectRAM[address].word;
                        this.dataGridView1.Rows[index].Cells[1].Value = vectCash[index].attribute;
                        this.dataGridView1.Rows[index].Cells[2].Value = vectCash[index].hoarder;
                        for (int i = 0; i < sizeOfCash; i++)
                        {
                            this.dataGridView1.Rows[i].Cells[3].Value = vectCash[i].priority;
                        }
                    }
                    else
                    {

                        index = MinPriority(vectCash, sizeOfCash);
                        vectRAM[address].word = word;
                        vectCash[index].attribute = address;
                        vectCash[index].hoarder = vectRAM[address].word;
                        ChangePriority(ref matrOfPriority, index, sizeOfCash);
                        PriorityOfCash(ref vectCash, matrOfPriority, sizeOfCash);
                        this.dataGridView2.Rows[address].Cells[1].Value = vectRAM[address].word;
                        this.dataGridView1.Rows[index].Cells[1].Value = vectCash[index].attribute;
                        this.dataGridView1.Rows[index].Cells[2].Value = vectCash[index].hoarder;
                        for (int i = 0; i < sizeOfCash; i++)
                        {
                            this.dataGridView1.Rows[i].Cells[3].Value = vectCash[i].priority;
                        }
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            VectCash(out vectCash);
            NullMatr(out matrOfPriority, sizeOfCash);
            for (int i = 0; i < sizeOfCash; i++)
            {
                this.dataGridView1.Rows[i].Cells[0].Value = vectCash[i].index;
                this.dataGridView1.Rows[i].Cells[1].Value = vectCash[i].attribute;
                this.dataGridView1.Rows[i].Cells[2].Value = vectCash[i].hoarder;
                this.dataGridView1.Rows[i].Cells[3].Value = vectCash[i].priority;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (!filledRAM)
            {
                MessageBox.Show("Заполните оперативную память");
                return;
            }

            
            chart1.Series[0].Points.Clear();
            int addressOFChecking, numOfHitting = 0;
            //int checkAtributte;
            bool flgOfHitting;
            int index;
            //int shift = rand.Next(sizeOfRAM - sizeOfRAM / 32 + 1);
            int shift = Convert.ToInt32( this.textBox3.Text);
            if ((shift + sizeOfCash) >= sizeOfRAM) { shift = sizeOfRAM - sizeOfCash - 1; }
            for (int i=0+shift; i <sizeOfRAM/32+shift; i++)
            {
                flgOfHitting = false;
                addressOFChecking = i;
                
                    if (InCash(vectCash,addressOFChecking,out index, sizeOfCash))
                    {
                        flgOfHitting = true;
                        ChangePriority(ref matrOfPriority, index, sizeOfCash);
                        PriorityOfCash(ref vectCash, matrOfPriority, sizeOfCash);
                        for (int l = 0; l < sizeOfCash; l++)
                        {
                            this.dataGridView1.Rows[l].Cells[3].Value = vectCash[l].priority;
                        }
                        numOfHitting++;
                    }
                    if (!flgOfHitting)
                    {
                        index = MinPriority(vectCash, sizeOfCash);
                        vectCash[index].attribute = addressOFChecking;
                        vectCash[index].hoarder = vectRAM[addressOFChecking].word;
                        ChangePriority(ref matrOfPriority, index, sizeOfCash);
                        PriorityOfCash(ref vectCash, matrOfPriority, sizeOfCash);
                        this.dataGridView1.Rows[index].Cells[1].Value = vectCash[index].attribute;
                        this.dataGridView1.Rows[index].Cells[2].Value = vectCash[index].hoarder;
                        for (int l = 0; l < sizeOfCash; l++)
                        {
                            this.dataGridView1.Rows[l].Cells[3].Value = vectCash[l].priority;
                        }

                    }

                    this.dataGridView3.Rows[i-shift].Cells[0].Value = Convert.ToString(i-shift);
                    this.dataGridView3.Rows[i-shift].Cells[1].Value = Convert.ToString(numOfHitting);
                    chart1.Series[0].Points.AddXY(i-shift, numOfHitting);
               

            }

            this.textBox3.Text = "" + rand.Next(sizeOfRAM - sizeOfRAM / 32 + 1);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!filledRAM)
            {
                MessageBox.Show("Заполните оперативную память");
                return;
            }
            chart1.Series[0].Points.Clear();
            int addressOFChecking, numOfHitting = 0;
            //int checkAtributte;
            bool flgOfHitting;
            int index;
            for (int i = 0; i < sizeOfRAM / 32; i++)
            {
                flgOfHitting = false;

                addressOFChecking = rand.Next(sizeOfRAM);
                    if (InCash(vectCash, addressOFChecking, out index, sizeOfCash))
                    {
                        flgOfHitting = true;
                        ChangePriority(ref matrOfPriority, index, sizeOfCash);
                        PriorityOfCash(ref vectCash, matrOfPriority, sizeOfCash);
                        for (int l = 0; l < sizeOfCash; l++)
                        {
                            this.dataGridView1.Rows[l].Cells[3].Value = vectCash[l].priority;
                        }
                        numOfHitting++;
                    }
                    if (!flgOfHitting)
                    {
                        index = MinPriority(vectCash, sizeOfCash);
                        vectCash[index].attribute = addressOFChecking;
                        vectCash[index].hoarder = vectRAM[addressOFChecking].word;
                        ChangePriority(ref matrOfPriority, index, sizeOfCash);
                        PriorityOfCash(ref vectCash, matrOfPriority, sizeOfCash);
                        this.dataGridView1.Rows[index].Cells[1].Value = vectCash[index].attribute;
                        this.dataGridView1.Rows[index].Cells[2].Value = vectCash[index].hoarder;
                        for (int l = 0; l < sizeOfCash; l++)
                        {
                            this.dataGridView1.Rows[l].Cells[3].Value = vectCash[l].priority;
                        }

                    }
                this.dataGridView3.Rows[i].Cells[0].Value = Convert.ToString(i);
                this.dataGridView3.Rows[i].Cells[1].Value = Convert.ToString(numOfHitting);
                chart1.Series[0].Points.AddXY(i, numOfHitting);

            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (!filledRAM)
            {
                MessageBox.Show("Заполните оперативную память");
                return;
            }
            int index;
            int address;
            for (int j = 0; j < sizeOfCash; j++)
            {
                address = rand.Next(sizeOfRAM);
                if (InCash(vectCash, address, out index, sizeOfCash))
                {
                    ChangePriority(ref matrOfPriority, index, sizeOfCash);
                    PriorityOfCash(ref vectCash, matrOfPriority, sizeOfCash);
                    for (int i = 0; i < sizeOfCash; i++)
                    {
                        this.dataGridView1.Rows[i].Cells[3].Value = vectCash[index].priority;
                    }

                }
                else
                {
                    if (!CheckFullCash(vectCash, out index, sizeOfCash))
                    {
                        vectCash[index].attribute = address;
                        vectCash[index].hoarder = vectRAM[address].word;
                        ChangePriority(ref matrOfPriority, index, sizeOfCash);
                        PriorityOfCash(ref vectCash, matrOfPriority, sizeOfCash);
                        this.dataGridView1.Rows[index].Cells[1].Value = vectCash[index].attribute;
                        this.dataGridView1.Rows[index].Cells[2].Value = vectCash[index].hoarder;
                        for (int i = 0; i < sizeOfCash; i++)
                        {
                            this.dataGridView1.Rows[i].Cells[3].Value = vectCash[i].priority;
                        }
                    }
                    else
                    {
                        index = MinPriority(vectCash, sizeOfCash);
                        vectCash[index].attribute = address;
                        vectCash[index].hoarder = vectRAM[address].word;
                        ChangePriority(ref matrOfPriority, index, sizeOfCash);
                        PriorityOfCash(ref vectCash, matrOfPriority, sizeOfCash);
                        this.dataGridView1.Rows[index].Cells[1].Value = vectCash[index].attribute;
                        this.dataGridView1.Rows[index].Cells[2].Value = vectCash[index].hoarder;
                        for (int i = 0; i < sizeOfCash; i++)
                        {
                            this.dataGridView1.Rows[i].Cells[3].Value = vectCash[i].priority;
                        }
                    }
                }
            }


        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (!filledRAM)
            {
                MessageBox.Show("Заполните оперативную память");
                return;
            }
            chart1.Series[0].Points.Clear();
            for (int k=1; k <sizeOfRAM/32; k++)
            {
                //MessageBox.Show("" + k);
                Cash[] newCashForStat = new Cash[k];
                int[,] newMatrForStat = new int[k, k];
                NullMatr(out newMatrForStat,k);
                int address;
                int index;
                for (int j = 0; j < k+1; j++)
                {
                    address = rand.Next(sizeOfRAM);
                    if (InCash(newCashForStat, address, out index,k))
                    {
                        ChangePriority(ref newMatrForStat, index,k);
                        PriorityOfCash(ref newCashForStat, newMatrForStat,k);

                    }
                    else
                    {
                        if (!CheckFullCash(vectCash, out index,k))
                        {
                            newCashForStat[index].attribute = address;
                            newCashForStat[index].hoarder = vectRAM[address].word;
                            ChangePriority(ref newMatrForStat, index,k);
                            PriorityOfCash(ref newCashForStat, newMatrForStat,k);
                        }
                        else
                        {
                            index = MinPriority(newCashForStat,k);
                            newCashForStat[index].attribute = address;
                            newCashForStat[index].hoarder = vectRAM[address].word;
                            ChangePriority(ref newMatrForStat, index,k);
                            PriorityOfCash(ref newCashForStat, newMatrForStat,k);
                        }
                    }
                }

                //////////////////////////////////////////////
                /////////////////////////////////////////////
                int addressOFChecking, numOfHitting = 0;
                bool flgOfHitting;
                for (int i = 0; i < sizeOfRAM / 32; i++)
                {
                    flgOfHitting = false;

                    addressOFChecking = rand.Next(sizeOfRAM);
                    if (InCash(newCashForStat, addressOFChecking, out index,k))
                    {
                        flgOfHitting = true;
                        ChangePriority(ref newMatrForStat, index,k);
                        PriorityOfCash(ref newCashForStat, newMatrForStat,k);
                        numOfHitting++;
                    }
                    if (!flgOfHitting)
                    {
                        index = MinPriority(newCashForStat,k);
                        newCashForStat[index].attribute = addressOFChecking;
                        newCashForStat[index].hoarder = vectRAM[addressOFChecking].word;
                        ChangePriority(ref newMatrForStat, index,k);
                        PriorityOfCash(ref newCashForStat, newMatrForStat,k);
                    }
                }

                chart1.Series[0].Points.AddXY(k, sizeOfCash- numOfHitting);
                this.dataGridView4.Rows[k].Cells[0].Value = Convert.ToString(k);
                this.dataGridView4.Rows[k].Cells[1].Value = Convert.ToString(sizeOfCash - numOfHitting);

            }


        }
    }
}
