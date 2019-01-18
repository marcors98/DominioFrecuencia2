using System;
using System.Windows.Forms;
using System.Windows;

namespace GraficadorSeñales
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double amplitudMaxima = 1;
        Señal señal;
        Señal señalResultado;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnGraficar_Click(object sender, RoutedEventArgs e)
        {
           
            double tiempoInicial = double.Parse(txtTiempoInicial.Text);
            double tiempoFinal = double.Parse(txtTiempoFinal.Text);
            double frecuenciaMuestreo = double.Parse(txtFrecuenciaMuestreo.Text);


            double umbral = double.Parse(txtUmbral.Text);
            

            //PRIMERA SEÑAL
            switch (cbTipoSeñal.SelectedIndex)
            {
                //Señal Senoidal
                case 0:
                    double amplitud = double.Parse(((ConfiguracionSeñalSenoidal)
                        panelConfiguracion.Children[0]).txtAmplitud.Text);

                    double fase = double.Parse(((ConfiguracionSeñalSenoidal)
                        panelConfiguracion.Children[0]).txtFase.Text);

                    double frecuencia = double.Parse(((ConfiguracionSeñalSenoidal)
                        panelConfiguracion.Children[0]).txtFrecuencia.Text);

                    señal = new SeñalSenoidal(amplitud, fase, frecuencia, umbral); //constructor

                    break;

                //Rampa
                case 1: señal = new SeñalRampa();

                    break;

                //Exponencial
                case 2:
                    double alpha = double.Parse(((ConfiguracionSeñalExponencial)
                        panelConfiguracion.Children[0]).txtAlpha.Text);

                    señal = new SeñalExponencial(alpha, umbral);
                    break;

                    //Rectangular
                case 3:
                    señal = new SeñalRectangular();
                    break;
                default:

                    señal = null;

                    break;

            }
            
            //---------------------------------PRIMERA SEÑAL------------------------------------------------------//
            señal.TiempoInicial = tiempoInicial;
            señal.TiempoFinal = tiempoFinal;
            señal.FrecuenciaMuestreo = frecuenciaMuestreo;
            señal.construirSeñalDigital();

            //Escalar
            double factorEscala = double.Parse(txtFactorEscalaAmplitud.Text);
            señal.escalar(factorEscala);
            
            //Desplazamiento 
            double desplazar = double.Parse(txtDesplazamientoY.Text);
            señal.desplazarY(desplazar);

            //Truncar
            //señal.truncar(umbral);
            
            señal.actualizarAmplitudMaxima();
            
            amplitudMaxima = señal.AmplitudMaxima;
           
            plnGrafica.Points.Clear();
            
            lblAmplitudMaximaY.Text = amplitudMaxima.ToString("F");
            lblAmplitudMaximaNegativaY.Text = "-" + amplitudMaxima.ToString("F");

            //PRIMERA SEÑAL
            if (señal != null)
            {
                //Recorre todos los elementos de una coleccion o arreglo
                foreach (Muestra muestra in señal.Muestras)
                {
                    plnGrafica.Points.Add(new Point((muestra.X - tiempoInicial) * scrContenedor.Width, (muestra.Y /
                        amplitudMaxima * ((scrContenedor.Height / 2.0) - 30) * -1) + 
                        (scrContenedor.Height / 2)));

                }
                
            }
            
            plnEjeX.Points.Clear();
            //Punto del principio
            plnEjeX.Points.Add(new Point(0, (scrContenedor.Height / 2)));
            //Punto del final
            plnEjeX.Points.Add(new Point((tiempoFinal - tiempoInicial) * scrContenedor.Width, 
                (scrContenedor.Height / 2)));

            plnEjeY.Points.Clear();
            //Punto del principio
            plnEjeY.Points.Add(new Point((0 - tiempoInicial) * scrContenedor.Width , (señal.AmplitudMaxima * 
                ((scrContenedor.Height / 2.0) - 30) * -1) + (scrContenedor.Height / 2)));
            //Punto del final
            plnEjeY.Points.Add(new Point((0 - tiempoInicial) * scrContenedor.Width, (-señal.AmplitudMaxima * 
                ((scrContenedor.Height / 2.0) - 30) * -1) + (scrContenedor.Height / 2)));
                        
        }
        
        private void cbTipoSeñal_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (panelConfiguracion != null)
            {
                panelConfiguracion.Children.Clear();

                switch (cbTipoSeñal.SelectedIndex)
                {
                    case 0:  //Senoidal
                        panelConfiguracion.Children.Add(new ConfiguracionSeñalSenoidal());
                        break;

                    case 1: //Rampa

                        break;

                    case 2://Exponencial
                        panelConfiguracion.Children.Add(new ConfiguracionSeñalExponencial());
                        break;

                    case 3: //Rectangular

                        break;
                    default:
                        break;

                }

            }
           
        }
        


        //CHECKBOX'S
        private void cbEscalaAmplitud_Checked(object sender, RoutedEventArgs e)
        {
            if (cbEscalaAmplitud.IsChecked == true)
            {
                txtFactorEscalaAmplitud.IsEnabled = true;
            }
            else
            {
                txtFactorEscalaAmplitud.IsEnabled = false;
            }
        }

        private void cbDesplazamientoY_Checked(object sender, RoutedEventArgs e)
        {
            if (cbDesplazamientoY.IsChecked == true)
            {
                txtDesplazamientoY.IsEnabled = true;
            }
            else
            {
                txtDesplazamientoY.IsEnabled = false;
            }
        }
        
        private void cbUmbral_Checked(object sender, RoutedEventArgs e)
        {
            if (cbUmbral.IsChecked == true)
            {
                txtUmbral.IsEnabled = true;

            } 
            else
            {
                txtUmbral.IsEnabled = false;
            }
        }

        private void btnTransformadaFourier_Click(object sender, RoutedEventArgs e)
        {
           
            Señal transformada = Señal.transformar(señal);
            transformada.actualizarAmplitudMaxima();

            plnGraficaResultado.Points.Clear();

            lblAmplitudMaximaY_Resultado.Text = transformada.AmplitudMaxima.ToString("F");
            lblAmplitudMaximaNegativaY_Resultado.Text = "-" + transformada.AmplitudMaxima.ToString("F");

            //PRIMERA SEÑAL
            if (transformada != null)
            {
                //Recorre todos los elementos de una coleccion o arreglo
                foreach (Muestra muestra in transformada.Muestras)
                {
                    plnGraficaResultado.Points.Add(new Point((muestra.X - transformada.TiempoInicial) * scrContenedor_Resultado.Width, (muestra.Y /
                        transformada.AmplitudMaxima * ((scrContenedor_Resultado.Height / 2.0) - 30) * -1) +
                        (scrContenedor_Resultado.Height / 2)));

                }
                double valorMaximo = 0;
                int indiceMaximo = 0;
                int indiceActual = 0;

                foreach(Muestra muestra in transformada.Muestras)
                {
                    if (muestra.Y > valorMaximo)
                        {
                        valorMaximo = muestra.Y;
                        indiceMaximo = indiceActual;
                    }
                    indiceActual++;
                    if (indiceActual > (double)transformada.Muestras.Count / 2.0)
                    {
                        break;
                    }
                }
            }

            plnEjeXResultado.Points.Clear();
            //Punto del principio
            plnEjeXResultado.Points.Add(new Point(0, (scrContenedor_Resultado.Height / 2)));
            //Punto del final
            plnEjeXResultado.Points.Add(new Point((transformada.TiempoFinal - transformada.TiempoInicial) * scrContenedor_Resultado.Width,
                (scrContenedor_Resultado.Height / 2)));

            plnEjeYResultado.Points.Clear();
            //Punto del principio
            plnEjeYResultado.Points.Add(new Point((0 - transformada.TiempoInicial) * scrContenedor_Resultado.Width, (transformada.AmplitudMaxima *
                ((scrContenedor_Resultado.Height / 2.0) - 30) * -1) + (scrContenedor_Resultado.Height / 2)));
            //Punto del final
            plnEjeYResultado.Points.Add(new Point((0 - transformada.TiempoInicial) * scrContenedor_Resultado.Width, (-transformada.AmplitudMaxima *
                ((scrContenedor_Resultado.Height / 2.0) - 30) * -1) + (scrContenedor_Resultado.Height / 2)));

       
        }
    }

}
