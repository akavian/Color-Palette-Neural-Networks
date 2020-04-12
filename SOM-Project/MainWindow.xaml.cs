using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
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

namespace SOM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static uint randomDataSize = 1000;
        byte[,] randomNumberArray = new byte[3, randomDataSize];
        byte[,,] palleteNumberArray = new byte[3, 50, 50];
        byte[,,] palleteNumberArrayHolder = new byte[3, 50, 50];
        Random randomeNumber = new Random();
        Boolean errorFlag = false;
        double minValue=10000;
        double value=0;
        uint[] winnerNeuron = new uint[2];
        double learningRate;
        uint vicinity;
        uint vicinityCounter;
        double minLearningRate;
        double learningRateStep;
        uint loopCounter;
        WriteableBitmap writeableBitmap;
        Image image;
        byte[] tempArray;
        Thread thread;
        public MainWindow()
        {
            tempArray = new byte[3];
            writeableBitmap = new WriteableBitmap(50 , 50 , 96 , 96, PixelFormats.Rgb24, null);
            image = new Image();
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetEdgeMode(image, EdgeMode.Aliased);
       
       
            InitializeComponent();
            
            thread = new Thread(process);
            for (int n = 0; n < 3; n++)
            {
                for (int i = 0; i < randomDataSize; i++)
                {
                     randomNumberArray[n,i] = (byte)randomeNumber.Next(0, 256);
                }
                
            }

            for (int n = 0; n < 3; n++)
            {
                for (int i = 0; i < 50; i++)
                {
                    for (int j = 0; j < 50; j++)
                    {
                        palleteNumberArrayHolder[n, i, j] = (byte)randomeNumber.Next(0, 256);
                    }
                }
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            process();
        }
        private void validation()
        {
            errLine.Content = "";
            errorFlag = false;
            try
            {
                loopCounter = Convert.ToUInt32(loopCount.Text);
                vicinity = Convert.ToUInt32(D.Text);
                vicinityCounter=Convert.ToUInt32(dStep.Text);

                learningRate=Convert.ToDouble(nStartValue.Text);
                if (learningRate < 0 || learningRate > 1)
                    errorFlag = true;

                minLearningRate=Convert.ToDouble(nMinValue.Text);
                if (minLearningRate < 0 || minLearningRate > 1)
                    errorFlag = true;

                learningRateStep=Convert.ToDouble(nStep.Text);
                if (learningRateStep < 0 || learningRateStep > 1)
                    errorFlag = true;
            }
            catch (Exception)
            {
                errorFlag = true;
            }
        }

        private void update(uint Neuron, uint neighborSize)
        {
            uint i;
            uint j;
            if (winnerNeuron[0] - neighborSize > 0)
                i = winnerNeuron[0] - neighborSize;
            else
            {
                i = 0;
            }
            if (winnerNeuron[1] - neighborSize > 0)
                j = winnerNeuron[1] - neighborSize;
            else
            {
                j = 0;
            }
            for (; i <= (winnerNeuron[0] + neighborSize) && i < 50; i++)
            {
                for (; j <= (winnerNeuron[1] + neighborSize) && j < 50; j++)
                {
                    for (uint n = 0; n < 3; n++)
                    {
                        palleteNumberArray[n, i, j] = (byte)(palleteNumberArray[n, i, j] + learningRate * (randomNumberArray[n, Neuron] - palleteNumberArray[n, i, j]));
                    }
                }
            }
            
        }
         
      

        private void findWinnerNeuron1(uint j)
        {
            value = 0;
            byte rowX = 0;// (byte)randomeNumber.Next(0, 50);
            byte colY = 0; // (byte)randomeNumber.Next(0, 50);
            for (uint k = rowX; k < 50; k++)
            {
                for (uint m = colY; m < 50; m++)
                {
                    for (uint n = 0; n < 3; n++)
                    {
                        value += Math.Pow(randomNumberArray[n, j] - palleteNumberArray[n, k, m], 2);
                    }
                    value = Math.Sqrt(value);
                    if (value < minValue)
                    {
                        minValue = value;
                        winnerNeuron[0] = k;
                        winnerNeuron[1] = m;
                       
                    }
                    value = 0;
                }
            }
      /*      for (int k = rowX -1 ; k  > -1 ; --k)
            {
                for (int m = colY -1; m > -1 ; --m)
                {
                    for (uint n = 0; n < 3; n++)
                    {
                        value += Math.Pow(randomNumberArray[n, j] - palleteNumberArray[n, k, m], 2);
                    }
                    value = Math.Sqrt(value);
                    if (value < minValue)
                    {
                        minValue = value;
                        winnerNeuron[0] = (uint)k;
                        winnerNeuron[1] = (uint)m;
                    }
                    value = 0;
                }
            }*/
        }

        private void initialPalette()
        {
            for (int n = 0; n < 3; n++)
            {
                for (int i = 0; i < 50; i++)
                {
                    for (int j = 0; j < 50; j++)
                    {
                        palleteNumberArray[n, i, j] = palleteNumberArrayHolder[n, i, j];
                    }
                }
            }
        }

        private void display()
        {
            for (int k = 0; k < 50; k++)
            {
                for (int m = 0; m < 50; m++)
                {
                    tempArray[0] = palleteNumberArray[0, k, m];
                    tempArray[1] = palleteNumberArray[1, k, m];
                    tempArray[2] = palleteNumberArray[2, k, m];
                    writeableBitmap.WritePixels(new Int32Rect(m, k, 1, 1), tempArray, 3, 0);
                }
                   
                paletteHolder.Navigate(image);
                image.Source = writeableBitmap;
          
            }
        }

            private void process() {
            validation();
            initialPalette();
            if (errorFlag == false)
            {

                for (uint i = 1; i <= loopCounter; i++)
                {

                    for (uint j = 0; j < randomDataSize; j++)
                    {
                        findWinnerNeuron1(j);
                        // here we have to update D and n
                        minValue = 10000;
                        update(j, vicinity);

                     /*   if (vicinityCounter == 0 && vicinity > 0)
                        {
                            vicinity--;
                            vicinityCounter = Convert.ToUInt32(dStep.Text);
                        }
                        if (learningRate - learningRateStep > minLearningRate)
                            learningRate = learningRate - learningRateStep;
                            */
                    }
                    vicinityCounter--;
                    if (vicinityCounter == 0 && vicinity > 0)
                    {
                        vicinity--;
                        vicinityCounter = Convert.ToUInt32(dStep.Text);
                    }
                    if (learningRate - learningRateStep > minLearningRate)
                        learningRate = learningRate - learningRateStep;
                    // display();           

                }
                //load image into frame;

                display();
            }
            else
            {
                errLine.Content = "Invalid input in textboxes please Enter a number";
            }
        }
        
        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
         
        }
    }
}
